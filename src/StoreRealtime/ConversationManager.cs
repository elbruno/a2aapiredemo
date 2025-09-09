using DataEntities;
using Microsoft.Extensions.AI; // For AIFunction
using OpenAI.Realtime;
using SearchEntities;
using StoreRealtime.ContextManagers; // Added for ContosoProductContext
using StoreRealtime.Support;
using System.Collections.Concurrent;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Linq; // Added for LINQ Select used in tool result serialization

namespace StoreRealtime;

public class ConversationManager : IDisposable
{
    private readonly RealtimeClient _client;
    private readonly ContosoProductContext _contosoProductContext;
    private readonly ILogger _logger;

    private RealtimeSession _session; // RealtimeSession (held as object for reflection-based adaptation)
    private CancellationTokenRegistration _cancellationRegistration;
    private bool _disposed;

    public ConversationManager(RealtimeClient client, ContosoProductContext contosoProductContext, ILogger logger)
    {
        _client = client;
        _contosoProductContext = contosoProductContext;
        _logger = logger;
    }

    public async Task RunAsync(Stream audioInput, Components.Speaker audioOutput,
        Func<string, Task> addMessageAsync,
        Func<string, bool, Task> addChatMessageAsync,
        Func<List<Product>, Task> addChatProductMessageAsync,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(audioInput);
        ArgumentNullException.ThrowIfNull(audioOutput);

        // Prepare system instructions similar to original sample.
        var prompt = $"""
            You are a useful assistant.
            Respond as succinctly as possible, in just a few words.
            Your main field of expertise is outdoor products.
            You are able to answer questions about outdoor products, including their features, specifications, and availability.
            Check the product database and external sources for information.
            The current date is {DateTime.Now.ToLongDateString()}
            """;
        await addMessageAsync("Connecting...");
        await addChatMessageAsync("Hello, how can I help?", false);

        try
        {


            // Build tool functions (reflection based invocation later when/if the model calls them via function calling).
            var semanticSearchTool = AIFunctionFactory.Create(_contosoProductContext.SemanticSearchOutdoorProductsAsync).ToConversationFunctionTool();
            var searchByNameTool = AIFunctionFactory.Create(_contosoProductContext.SearchOutdoorProductsByNameAsync).ToConversationFunctionTool();
            List<AIFunction> tools = [semanticSearchTool, searchByNameTool];

            var modelName = Environment.GetEnvironmentVariable("AI_RealtimeDeploymentName")
                            ?? Environment.GetEnvironmentVariable("OPENAI_REALTIME_MODEL")
                            ?? "gpt-realtime"; // sensible default

            _logger.LogInformation("Starting realtime session with model {Model}", modelName);

            _session = await _client.StartConversationSessionAsync(model: modelName);
            await addMessageAsync("Session started...");

            ConversationSessionOptions conversationSessionOptions = CreateConversationSessionOptions(prompt, tools);
            await _session.ConfigureConversationSessionAsync(conversationSessionOptions);

            var userPartialTranscript = new StringBuilder();      // Collects streaming user speech until finished
            var assistantPartialTranscript = new StringBuilder(); // Collects streaming assistant speech until finished

            // Local function to invoke model requested tool by name and JSON args
            async Task<(string toolResultText, List<Product>? products)> InvokeToolAsync(string functionName, string? rawArgs)
            {
                try
                {
                    // Parse single string argument for both existing context functions which expect a single string parameter
                    string? extracted = null;
                    if (!string.IsNullOrWhiteSpace(rawArgs))
                    {
                        try
                        {
                            using var doc = JsonDocument.Parse(string.IsNullOrWhiteSpace(rawArgs) ? "{}" : rawArgs);
                            // Accept common keys
                            if (doc.RootElement.ValueKind == JsonValueKind.Object)
                            {
                                if (doc.RootElement.TryGetProperty("searchCriteria", out var sc)) extracted = sc.GetString();
                                else if (doc.RootElement.TryGetProperty("name", out var nm)) extracted = nm.GetString();
                                else if (doc.RootElement.TryGetProperty("query", out var q)) extracted = q.GetString();
                                else if (doc.RootElement.EnumerateObject().FirstOrDefault().Value.ValueKind == JsonValueKind.String)
                                {
                                    extracted = doc.RootElement.EnumerateObject().First().Value.GetString();
                                }
                            }
                        }
                        catch { /* swallow parse errors; fallback below */ }
                    }
                    extracted ??= rawArgs?.Trim('"'); // fallback if rawArgs was already a simple JSON string

                    // Provide a safe default
                    if (string.IsNullOrWhiteSpace(extracted)) extracted = "outdoor"; // generic fallback

                    if (functionName.Equals("SemanticSearchOutdoorProducts", StringComparison.OrdinalIgnoreCase))
                    {
                        var json = await _contosoProductContext.SemanticSearchOutdoorProductsAsync(extracted);
                        // Try to parse products from json (List<Product>)
                        List<Product>? prods = null;
                        try
                        {
                            prods = JsonSerializer.Deserialize<SearchResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })?.Products;
                        }
                        catch { /* ignore */ }
                        return ($"Semantic search results for '{extracted}'", prods ?? new List<Product>());
                    }
                    if (functionName.Equals("SearchOutdoorProductsByName", StringComparison.OrdinalIgnoreCase))
                    {
                        var text = await _contosoProductContext.SearchOutdoorProductsByNameAsync(extracted);
                        // name search returns plain text list (?) – attempt parse first
                        List<Product>? prods = null;
                        try
                        {
                            var sr = JsonSerializer.Deserialize<SearchResponse>(text, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                            prods = sr?.Products;
                        }
                        catch { /* ignore, treat as plain text */ }
                        return (text, prods ?? new List<Product>());
                    }
                    return ($"Tool '{functionName}' not recognized", new List<Product>());
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Tool invocation error for {Function}", functionName);
                    return ($"Error invoking {functionName}: {ex.Message}", new List<Product>());
                }
            }

            // NOTE: Previous implementation pushed a chat bubble on every OutputDeltaUpdate which caused
            // multiple fragmented bubbles for a single assistant response. We now:
            //  1. Buffer user transcription deltas until InputAudioTranscriptionFinishedUpdate, then send ONE user bubble
            //  2. Buffer assistant output deltas until OutputStreamingFinishedUpdate (non function-call), then send ONE assistant bubble
            //  3. Keep function calling path; when a function call completes we inject a tool result item (still simple placeholder)
            //  4. Provide log updates (addMessageAsync) for granular progress without polluting chat UI

            await foreach (RealtimeUpdate update in _session.ReceiveUpdatesAsync(cancellationToken))
            {
                switch (update)
                {
                    // SESSION LIFECYCLE -----------------------------------------------------------
                    case ConversationSessionStartedUpdate:
                        await addMessageAsync("Conversation started");
                        // Fire-and-forget streaming of input audio
                        _ = Task.Run(async () => await _session.SendInputAudioAsync(audioInput, cancellationToken));
                        break;

                    // USER INPUT (MIC) -----------------------------------------------------------
                    case InputAudioSpeechStartedUpdate:
                        await addMessageAsync("Speech started");
                        await audioOutput.ClearPlaybackAsync(); // make sure no overlapping playback
                        userPartialTranscript.Clear();
                        break;

                    case InputAudioTranscriptionDeltaUpdate userDelta:
                        // Buffer only (no logging of partials to avoid noisy UI)
                        userPartialTranscript.Append(userDelta.Delta);
                        break;

                    case InputAudioTranscriptionFinishedUpdate userDone:
                        // Final user transcript -> single chat bubble
                        var finalUser = userDone.Transcript?.Trim();
                        if (!string.IsNullOrEmpty(finalUser))
                        {
                            await addMessageAsync($"User: {finalUser}");
                            await addChatMessageAsync(finalUser, true);
                        }
                        userPartialTranscript.Clear();
                        break;

                    case InputAudioSpeechFinishedUpdate:
                        await addMessageAsync("Speech finished");
                        break;

                    // ASSISTANT OUTPUT STREAMING -------------------------------------------------
                    case OutputStreamingStartedUpdate started:
                        assistantPartialTranscript.Clear();
                        if (!string.IsNullOrWhiteSpace(started.FunctionName))
                        {
                            // Model announced it's about to call a function
                            await addMessageAsync($"Calling function: {started.FunctionName}({started.FunctionCallArguments})");
                        }
                        else
                        {
                            await addMessageAsync("Assistant answering...");
                        }
                        break;

                    case OutputDeltaUpdate delta:
                        // Accumulate assistant streaming transcript only (no partial logging)
                        if (!string.IsNullOrEmpty(delta.AudioTranscript))
                        {
                            assistantPartialTranscript.Append(delta.AudioTranscript);
                        }
                        break;

                    case OutputStreamingFinishedUpdate finished:
                        // Record final outcome only
                        if (finished.FunctionCallId is not null)
                        {
                            // Real tool invocation
                            var (toolText, products) = await InvokeToolAsync(finished.FunctionName ?? string.Empty, finished.FunctionCallArguments);

                            // Build tool output item for the model with textual summary (keep concise)
                            var toolOutput = toolText;
                            if (products?.Count > 0)
                            {
                                // Provide a terse JSON with minimal fields to keep token usage low
                                var compact = JsonSerializer.Serialize(products.Select(p => new { p.Name, p.Description, p.Price }));
                                toolOutput = compact;
                            }
                            RealtimeItem functionOutputItem = RealtimeItem.CreateFunctionCallOutput(
                                callId: finished.FunctionCallId,
                                output: toolOutput);
                            await _session.AddItemAsync(functionOutputItem);

                            await addMessageAsync($"Tool executed: {finished.FunctionName}");
                            if (products?.Count > 0)
                            {
                                await addMessageAsync($"Products returned: {products.Count}");
                                await addChatProductMessageAsync(products); // show product cards/bubble
                            }
                            else
                            {
                                await addChatMessageAsync(toolText, false);
                            }
                        }
                        else
                        {
                            string assistantText;
                            if (finished.MessageContentParts?.Count > 0)
                            {
                                var assembled = new StringBuilder();
                                foreach (ConversationContentPart part in finished.MessageContentParts)
                                {
                                    if (!string.IsNullOrEmpty(part.AudioTranscript)) assembled.Append(part.AudioTranscript);
                                    else if (!string.IsNullOrEmpty(part.Text)) assembled.Append(part.Text);
                                }
                                assistantText = assembled.Length > 0 ? assembled.ToString() : assistantPartialTranscript.ToString();
                            }
                            else
                            {
                                assistantText = assistantPartialTranscript.ToString();
                            }
                            assistantText = assistantText.Trim();
                            if (!string.IsNullOrEmpty(assistantText))
                            {
                                await addMessageAsync($"Assistant: {assistantText}");
                                await addChatMessageAsync(assistantText, false);
                            }
                        }
                        assistantPartialTranscript.Clear();
                        break;

                    // DEFAULT / UNHANDLED --------------------------------------------------------
                    default:
                        // Suppress noisy low-signal updates (ResponseStartedUpdate, ItemCreatedUpdate, etc.)
                        // If troubleshooting, temporarily re-enable logging here.
                        break;
                }
            }

        }
        catch (Exception exc)
        {
            _logger.LogError(exc, "Error occurred while processing conversation updates");
            await addMessageAsync($"Error: {exc.Message}");
        }
    }


    private ConversationSessionOptions CreateConversationSessionOptions(string instructions, List<AIFunction> tools)
    {
        ConversationSessionOptions sessionOptions = new()
        {
            Instructions = instructions,
            Voice = ConversationVoice.Alloy,
            InputAudioFormat = RealtimeAudioFormat.Pcm16,
            OutputAudioFormat = RealtimeAudioFormat.Pcm16,
            // Input transcription options must be provided to enable transcribed feedback for input audio
            InputTranscriptionOptions = new()
            {
                Model = "whisper-1",
            },
        };

        // iterate through tools and add them to the session options tools
        foreach (var tool in tools)
        {
            sessionOptions.Tools.Add(tool.AsOpenAIConversationFunctionTool());
        }
        return sessionOptions;
    }


    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _cancellationRegistration.Dispose();
        try
        {
            if (_session is IDisposable d) d.Dispose();
            else if (_session is IAsyncDisposable ad) ad.DisposeAsync().AsTask().GetAwaiter().GetResult();
        }
        catch { /* ignore */ }
    }
}
