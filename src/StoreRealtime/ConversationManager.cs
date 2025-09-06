using DataEntities;
using Microsoft.Extensions.AI; // For AIFunction
using OpenAI.Realtime;
using StoreRealtime.ContextManagers; // Added for ContosoProductContext
using StoreRealtime.Support;
using System.Collections.Concurrent;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace StoreRealtime;

// Transitional implementation while migrating from obsolete RealtimeConversation* API to new RealtimeClient API.
// This class emulates the prior sample behavior (prompt setup, tool registration, audio streaming, update loop,
// function call handling, transcript + audio output) using reflective calls. The new OpenAI Realtime SDK surface
// is still evolving; we avoid direct compile-time dependencies on the (previous) Conversation* update types so
// the project continues to build even if names shift. Replace reflection with strongly-typed usage once the
// final API for RealtimeSession updates is stable.
public class ConversationManager : IDisposable
{
    private readonly RealtimeClient _client;
    private readonly ContosoProductContext _contosoProductContext;
    private readonly ILogger _logger;

    private object? _session; // RealtimeSession (held as object for reflection-based adaptation)
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
        await SafeInvoke(addMessageAsync, "Connecting...");
        await SafeInvoke(addChatMessageAsync, ("Hello, how can I help?", false));

        // Build tool functions (reflection based invocation later when/if the model calls them via function calling).
        var semanticSearchTool = AIFunctionFactory.Create(_contosoProductContext.SemanticSearchOutdoorProductsAsync).ToConversationFunctionTool();
        var searchByNameTool = AIFunctionFactory.Create(_contosoProductContext.SearchOutdoorProductsByNameAsync).ToConversationFunctionTool();
        List<AIFunction> tools = [semanticSearchTool, searchByNameTool];

        // Create session options if the runtime type exists.
        object? sessionOptions = CreateSessionOptions(prompt, tools);

        // Determine model name from configuration / environment (fallback to gpt-realtime)
        var modelName = Environment.GetEnvironmentVariable("AI_RealtimeDeploymentName")
                        ?? Environment.GetEnvironmentVariable("OPENAI_REALTIME_MODEL")
                        ?? "gpt-realtime"; // sensible default

        _logger.LogInformation("Starting realtime session with model {Model}", modelName);

        // Start session: RealtimeClient.StartConversationSessionAsync(string model, RealtimeSessionOptions options = null, CancellationToken)
        _session = await StartSessionAsync(modelName, sessionOptions, cancellationToken)
                   ?? throw new InvalidOperationException("Failed to start realtime session (null session returned).");

        // Configure session if an explicit configure method exists (older sample pattern)
        await ConfigureSessionIfSupportedAsync(_session, sessionOptions, cancellationToken);

        var outputTranscription = new StringBuilder();
        var functionCallOutputs = new ConcurrentQueue<object>();

        // Kick off audio send in background if API exists
        _ = Task.Run(async () => await SendMicrophoneAudioIfSupportedAsync(_session, audioInput, addMessageAsync, cancellationToken), cancellationToken);

        // Consume updates if streaming API exists; else we cannot emulate incremental updates.
        var receiveUpdatesAsyncMethod = _session.GetType().GetMethod("ReceiveUpdatesAsync", BindingFlags.Public | BindingFlags.Instance);
        if (receiveUpdatesAsyncMethod is null)
        {
            await SafeInvoke(addMessageAsync, "Realtime session started (no update stream method exposed).");
            return; // Nothing else we can do.
        }

        // Expect IAsyncEnumerable<T>; iterate via dynamic.
        var updatesAsyncEnumerable = receiveUpdatesAsyncMethod.Invoke(_session, new object?[] { cancellationToken });
        if (updatesAsyncEnumerable is null)
        {
            await SafeInvoke(addMessageAsync, "Realtime session produced no updates enumerable.");
            return;
        }

        // Use generic helper to iterate without compile-time type.
        await foreach (var update in ReflectiveAsyncEnumerable(updatesAsyncEnumerable, cancellationToken))
        {
            if (update is null) continue;
            cancellationToken.ThrowIfCancellationRequested();
            var updateTypeName = update.GetType().Name;

            try
            {
                switch (updateTypeName)
                {
                    case "ConversationSessionStartedUpdate":
                        await SafeInvoke(addMessageAsync, "Conversation started");
                        break;

                    case "ConversationInputSpeechStartedUpdate":
                        await SafeInvoke(addMessageAsync, "Speech started");
                        await audioOutput.ClearPlaybackAsync();
                        break;

                    case "ConversationInputTranscriptionFinishedUpdate":
                        if (TryGetProperty(update, "Transcript", out string? transcript) && !string.IsNullOrWhiteSpace(transcript))
                        {
                            await SafeInvoke(addMessageAsync, $"User: {transcript}");
                            await SafeInvoke(addChatMessageAsync, (transcript, true));
                        }
                        break;

                    case "ConversationInputSpeechFinishedUpdate":
                        await SafeInvoke(addMessageAsync, "Speech finished");
                        break;

                    case "ConversationItemStreamingPartDeltaUpdate":
                        // Accumulate audio + text deltas
                        if (TryGetProperty(update, "AudioBytes", out byte[]? audioBytes) && audioBytes?.Length > 0)
                        {
                            await audioOutput.EnqueueAsync(audioBytes);
                        }
                        if (TryGetProperty(update, "Text", out string? textDelta) && !string.IsNullOrEmpty(textDelta))
                        {
                            outputTranscription.Append(textDelta);
                        }
                        else if (TryGetProperty(update, "AudioTranscript", out string? audioTranscript) && !string.IsNullOrEmpty(audioTranscript))
                        {
                            outputTranscription.Append(audioTranscript);
                        }
                        break;

                    case "ConversationItemStreamingAudioTranscriptionFinishedUpdate":
                    case "ConversationItemStreamingTextFinishedUpdate":
                        if (outputTranscription.Length > 0)
                        {
                            var finalText = outputTranscription.ToString();
                            await SafeInvoke(addMessageAsync, $"Assistant: {finalText}");
                            await SafeInvoke(addChatMessageAsync, (finalText, false));
                            outputTranscription.Clear();
                        }
                        break;

                    case "ConversationItemStreamingFinishedUpdate":
                        // Possible function call completion
                        if (TryGetProperty(update, "FunctionName", out string? fnName) && !string.IsNullOrWhiteSpace(fnName))
                        {
                            string argsJson = TryGetProperty(update, "FunctionCallArguments", out string? fnArgs) ? fnArgs ?? "{}" : "{}";
                            await SafeInvoke(addMessageAsync, $"Calling function: {fnName}({argsJson})");
                            var output = await GetFunctionCallOutputAsync(update, tools);
                            if (output is not null)
                            {
                                functionCallOutputs.Enqueue(output);
                                await SafeInvoke(addMessageAsync, $"Call function finished: {output}");
                                await AddItemAsyncIfSupported(_session, output, cancellationToken);

                                // Attempt to extract underlying output property for product list
                                if (TryExtractSearchProducts(output, out var products))
                                {
                                    await SafeInvoke(addChatProductMessageAsync, products);
                                }
                            }
                        }
                        break;

                    case "ConversationResponseFinishedUpdate":
                        // If any created items had function calls, instruct model to respond again.
                        bool hadFunctionItems = false;
                        if (TryGetProperty(update, "CreatedItems", out System.Collections.IEnumerable? createdItems) && createdItems is not null)
                        {
                            foreach (var item in createdItems)
                            {
                                if (TryGetProperty(item, "FunctionName", out string? createdFn) && !string.IsNullOrWhiteSpace(createdFn))
                                {
                                    hadFunctionItems = true; break;
                                }
                            }
                        }
                        if (hadFunctionItems)
                        {
                            // Try to log token usage if present
                            if (TryGetProperty(update, "Usage", out object? usage) && usage is not null)
                            {
                                var tokensSb = new StringBuilder();
                                if (TryGetProperty(usage, "TotalTokens", out int total)) tokensSb.AppendLine($"Total Tokens: {total}");
                                if (TryGetProperty(usage, "InputTokens", out int input)) tokensSb.AppendLine($"Input Tokens: {input}");
                                if (TryGetProperty(usage, "OutputTokens", out int outputTokens)) tokensSb.AppendLine($"Output Tokens: {outputTokens}");
                                if (tokensSb.Length > 0)
                                {
                                    await SafeInvoke(addMessageAsync, tokensSb.ToString().TrimEnd());
                                }
                            }
                            await StartResponseIfSupportedAsync(_session, cancellationToken);
                        }
                        break;
                }
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogWarning(ex, "Error processing realtime update type {Type}", updateTypeName);
            }
        }
    }

    #region Reflection helpers

    private object? CreateSessionOptions(string instructions, List<AIFunction> tools)
    {
        try
        {
            var optionsType = Type.GetType("OpenAI.Realtime.RealtimeSessionOptions, OpenAI");
            if (optionsType is null) return null;
            var options = Activator.CreateInstance(optionsType);
            // Set Instructions
            TrySetProperty(options, "Instructions", instructions);

            // Attempt to set Voice (string or enum); optional.
            if (optionsType.GetProperty("Voice") is { } voiceProp)
            {
                // Known sample voice name
                object? value = null;
                if (voiceProp.PropertyType.IsEnum)
                {
                    value = Enum.GetValues(voiceProp.PropertyType).Cast<object?>().FirstOrDefault(v => string.Equals(v?.ToString(), "Shimmer", StringComparison.OrdinalIgnoreCase));
                }
                else if (voiceProp.PropertyType == typeof(string))
                {
                    value = "Shimmer";
                }
                if (value is not null) voiceProp.SetValue(options, value);
            }

            // InputTranscriptionOptions with Model = whisper-1 if available
            var inputTxProp = optionsType.GetProperty("InputTranscriptionOptions");
            if (inputTxProp is not null)
            {
                var txType = inputTxProp.PropertyType;
                var txObj = Activator.CreateInstance(txType);
                TrySetProperty(txObj, "Model", "whisper-1");
                inputTxProp.SetValue(options, txObj);
            }

            // Tools property maybe a collection we can add to
            if (optionsType.GetProperty("Tools")?.GetValue(options) is System.Collections.IList toolList)
            {
                foreach (var tool in tools)
                {
                    // For now, we add the AIFunction directly (extension earlier returns same instance).
                    toolList.Add(tool);
                }
            }
            return options;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed creating session options reflectively.");
            return null;
        }
    }

    private async Task<object?> StartSessionAsync(string model, object? options, CancellationToken ct)
    {
        // Prefer StartConversationSessionAsync(model, options, ct) if available; fall back to StartSessionAsync(model, intent, options, ct)
        var type = _client.GetType();
        var method = type.GetMethod("StartConversationSessionAsync", new[] { typeof(string), options?.GetType() ?? typeof(object), typeof(CancellationToken) })
                     ?? type.GetMethods().FirstOrDefault(m => m.Name == "StartConversationSessionAsync" && m.GetParameters().Length >= 1);
        if (method is not null)
        {
            var task = method.Invoke(_client, BuildArgumentArray(method, model, options, ct));
            return await AwaitResultAsync(task);
        }

        // Fallback generic StartSessionAsync(model, intent, options, ct)
        var startSession = type.GetMethods().FirstOrDefault(m => m.Name == "StartSessionAsync");
        if (startSession is not null)
        {
            var args = BuildArgumentArray(startSession, model, options, ct);
            var task = startSession.Invoke(_client, args);
            return await AwaitResultAsync(task);
        }

        throw new MissingMethodException("RealtimeClient does not expose a recognized async start session method.");
    }

    private async Task ConfigureSessionIfSupportedAsync(object session, object? options, CancellationToken ct)
    {
        if (options is null) return;
        var m = session.GetType().GetMethod("ConfigureSessionAsync", BindingFlags.Public | BindingFlags.Instance);
        if (m is null) return; // optional
        var task = m.Invoke(session, new[] { options, ct });
        await AwaitResultAsync(task);
    }

    private async Task SendMicrophoneAudioIfSupportedAsync(object session, Stream micStream, Func<string, Task> addMessageAsync, CancellationToken ct)
    {
        try
        {
            var method = session.GetType().GetMethod("SendInputAudioAsync", BindingFlags.Public | BindingFlags.Instance);
            if (method is null)
            {
                await SafeInvoke(addMessageAsync, "Audio streaming not supported by current RealtimeSession implementation.");
                return;
            }
            var task = method.Invoke(session, new object?[] { micStream, ct });
            await AwaitResultAsync(task);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogWarning(ex, "Error streaming microphone audio.");
        }
    }

    private async Task AddItemAsyncIfSupported(object session, object item, CancellationToken ct)
    {
        var m = session.GetType().GetMethod("AddItemAsync", BindingFlags.Public | BindingFlags.Instance);
        if (m is null) return;
        var task = m.Invoke(session, new[] { item, ct });
        await AwaitResultAsync(task);
    }

    private async Task StartResponseIfSupportedAsync(object session, CancellationToken ct)
    {
        var m = session.GetType().GetMethod("StartResponseAsync", BindingFlags.Public | BindingFlags.Instance);
        if (m is null) return;
        var task = m.Invoke(session, new object?[] { ct });
        await AwaitResultAsync(task);
    }

    private async Task<object?> GetFunctionCallOutputAsync(object update, List<AIFunction> tools)
    {
        try
        {
            var method = update.GetType().GetMethod("GetFunctionCallOutputAsync", BindingFlags.Public | BindingFlags.Instance);
            if (method is null) return null;
            var task = method.Invoke(update, new object?[] { tools });
            return await AwaitResultAsync(task);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed retrieving function call output.");
            return null;
        }
    }

    private static bool TryExtractSearchProducts(object functionOutput, out List<Product> products)
    {
        products = new();
        try
        {
            // Attempt to find an "Output" property (might be internal / non-public)
            var outputProp = functionOutput.GetType().GetProperty("Output", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var value = outputProp?.GetValue(functionOutput);
            if (value is null) return false;
            // Expect JSON convertible to SearchResponse (in SearchEntities). We use Product list only.
            var json = value.ToString();
            if (string.IsNullOrWhiteSpace(json)) return false;
            using var doc = JsonDocument.Parse(json);
            if (!doc.RootElement.TryGetProperty("products", out var productsElement) || productsElement.ValueKind != JsonValueKind.Array)
                return false;
            foreach (var p in productsElement.EnumerateArray())
            {
                products.Add(new Product
                {
                    Id = p.TryGetProperty("id", out var idEl) && idEl.TryGetInt32(out var idVal) ? idVal : 0,
                    Name = p.TryGetProperty("name", out var nameEl) ? nameEl.GetString() ?? string.Empty : string.Empty,
                    Description = p.TryGetProperty("description", out var dEl) ? dEl.GetString() ?? string.Empty : string.Empty,
                    Price = p.TryGetProperty("price", out var priceEl) && priceEl.TryGetDecimal(out var priceVal) ? priceVal : 0m,
                    ImageUrl = p.TryGetProperty("imageUrl", out var imgEl) ? imgEl.GetString() ?? string.Empty : string.Empty
                });
            }
            return products.Count > 0;
        }
        catch
        {
            return false;
        }
    }

    private static async Task<object?> AwaitResultAsync(object? maybeTask)
    {
        if (maybeTask is null) return null;
        switch (maybeTask)
        {
            case Task t when t.GetType().IsGenericType:
                await t.ConfigureAwait(false);
                return t.GetType().GetProperty("Result")?.GetValue(t);
            case Task t2:
                await t2.ConfigureAwait(false);
                return null;
            case ValueTask vt:
                await vt.ConfigureAwait(false);
                return null;
        }
        return maybeTask; // Not a task
    }

    private static object?[] BuildArgumentArray(MethodInfo method, string model, object? options, CancellationToken ct)
    {
        var parameters = method.GetParameters();
        var args = new object?[parameters.Length];
        for (int i = 0; i < parameters.Length; i++)
        {
            var p = parameters[i];
            if (i == 0 && p.ParameterType == typeof(string)) args[i] = model;
            else if (p.ParameterType == options?.GetType()) args[i] = options;
            else if (p.ParameterType == typeof(CancellationToken)) args[i] = ct;
            else if (p.Name?.Equals("intent", StringComparison.OrdinalIgnoreCase) == true) args[i] = "chat"; // fallback intent
            else if (p.HasDefaultValue) args[i] = p.DefaultValue;
            else args[i] = null;
        }
        return args;
    }

    private static bool TryGetProperty<T>(object source, string name, out T? value)
    {
        value = default;
        if (source is null) return false;
        var prop = source.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (prop is null) return false;
        var raw = prop.GetValue(source);
        if (raw is null) return false;
        if (raw is T tv)
        {
            value = tv; return true;
        }
        try
        {
            value = (T?)Convert.ChangeType(raw, typeof(T));
            return true;
        }
        catch { return false; }
    }

    private static void TrySetProperty(object? target, string name, object? value)
    {
        if (target is null) return;
        var prop = target.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (prop is null || !prop.CanWrite) return;
        try { prop.SetValue(target, value); } catch { /* ignore */ }
    }

    private static async Task SafeInvoke(Func<string, Task> action, string arg)
    {
        try { await action(arg); } catch { /* UI update exceptions ignored */ }
    }
    private static async Task SafeInvoke(Func<string, bool, Task> action, (string text, bool flag) args)
    {
        try { await action(args.text, args.flag); } catch { }
    }
    private static async Task SafeInvoke(Func<List<Product>, Task> action, List<Product> arg)
    {
        try { await action(arg); } catch { }
    }

    private static async IAsyncEnumerable<object?> ReflectiveAsyncEnumerable(object asyncEnumerable, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken ct)
    {
        // Expect GetAsyncEnumerator(CancellationToken)
        var getAsyncEnum = asyncEnumerable.GetType().GetMethod("GetAsyncEnumerator");
        if (getAsyncEnum is null)
        {
            yield break;
        }
        var enumerator = getAsyncEnum.GetParameters().Length == 1
            ? getAsyncEnum.Invoke(asyncEnumerable, new object?[] { ct })
            : getAsyncEnum.Invoke(asyncEnumerable, Array.Empty<object?>());
        if (enumerator is null) yield break;

        var moveNextAsync = enumerator.GetType().GetMethod("MoveNextAsync");
        var currentProp = enumerator.GetType().GetProperty("Current");
        if (moveNextAsync is null || currentProp is null) yield break;

        while (true)
        {
            ct.ThrowIfCancellationRequested();
            var mnaResult = moveNextAsync.Invoke(enumerator, Array.Empty<object?>());
            var hasNext = false;
            if (mnaResult is ValueTask<bool> vtBool)
            {
                hasNext = await vtBool.ConfigureAwait(false);
            }
            else if (mnaResult is Task<bool> tBool)
            {
                hasNext = await tBool.ConfigureAwait(false);
            }
            else if (mnaResult is ValueTask vtGeneric)
            {
                await vtGeneric.ConfigureAwait(false);
                // Unknown result type; break
                yield break;
            }
            else if (mnaResult is Task t)
            {
                await t.ConfigureAwait(false); yield break; // assume non-generic ended
            }
            else
            {
                yield break;
            }

            if (!hasNext) yield break;
            yield return currentProp.GetValue(enumerator);
        }
    }

    #endregion

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
