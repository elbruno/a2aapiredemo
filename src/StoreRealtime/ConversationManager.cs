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

            var outputTranscription = new StringBuilder();
            var functionCallOutputs = new ConcurrentQueue<object>();

            await foreach (RealtimeUpdate update in _session.ReceiveUpdatesAsync(cancellationToken))
            {
                switch (update)
                {
                    case ConversationSessionStartedUpdate:
                        await addMessageAsync("Conversation started");
                        _ = Task.Run(async () => await _session.SendInputAudioAsync(audioInput, cancellationToken));
                        break;

                    // INPUT AUDIO UPDATES

                    case InputAudioSpeechStartedUpdate:
                        await addMessageAsync("Speech started");
                        await audioOutput.ClearPlaybackAsync();
                        break;

                    case InputAudioSpeechFinishedUpdate:
                        await addMessageAsync("Speech finished");
                        break;

                    case InputAudioTranscriptionFinishedUpdate:
                        var transcript = update as InputAudioTranscriptionFinishedUpdate;
                        await addMessageAsync($"User: {transcript.Transcript}");
                        await addChatMessageAsync(transcript.Transcript, true);
                        break;

                    case InputAudioTranscriptionDeltaUpdate: //ConversationItemStreamingPartDeltaUpdate outputDelta:

                        var deltaUpdate = update as InputAudioTranscriptionDeltaUpdate;
                        // Append the delta text to the output transcription
                        outputTranscription.Append(deltaUpdate.Delta);
                        await addMessageAsync($"Assistant: {outputTranscription}");
                        break;

                    // OUTPUT AUDIO UPDATES

                    case OutputStreamingStartedUpdate:
                        var outputUpdate = update as OutputStreamingStartedUpdate;

                        if (!string.IsNullOrEmpty(outputUpdate.FunctionName))
                        {
                            await addMessageAsync($"Calling function: {outputUpdate.FunctionName}({outputUpdate.FunctionCallArguments})");

                        }
                        break;

                    case OutputDeltaUpdate outputDelta:

                        outputTranscription.Clear();
                        await addMessageAsync($"Assistant: {outputDelta.AudioTranscript}");
                        await addChatMessageAsync($"{outputDelta.AudioTranscript}", false);
                        outputTranscription.Clear();
                        break;

                    case OutputStreamingFinishedUpdate outputStreamingFinishedUpdate:

                        addMessageAsync($"  -- Item streaming finished, item_id={outputStreamingFinishedUpdate.ItemId}");

                        if (outputStreamingFinishedUpdate.FunctionCallId is not null)
                        {
                            addMessageAsync($"    + Responding to tool invoked by item: {outputStreamingFinishedUpdate.FunctionName}");
                            RealtimeItem functionOutputItem = RealtimeItem.CreateFunctionCallOutput(
                                callId: outputStreamingFinishedUpdate.FunctionCallId,
                                output: "70 degrees Fahrenheit and sunny");
                            await _session.AddItemAsync(functionOutputItem);
                        }
                        else if (outputStreamingFinishedUpdate.MessageContentParts?.Count > 0)
                        {
                            addMessageAsync($"+ [{outputStreamingFinishedUpdate.MessageRole}]: ");
                            foreach (ConversationContentPart contentPart in outputStreamingFinishedUpdate.MessageContentParts)
                            {
                                addMessageAsync(contentPart.AudioTranscript);
                            }
                        }
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
