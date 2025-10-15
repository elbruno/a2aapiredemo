using System.ClientModel;
using Azure.AI.OpenAI;
using Azure.Identity;
using OpenAI.Realtime;
using Microsoft.Extensions.Logging;
using AgentFx.RealtimeAudioConsole.Audio;
using AgentFx.RealtimeAudioConsole.Configuration;

namespace AgentFx.RealtimeAudioConsole.Agents;

/// <summary>
/// Agent that uses Microsoft Agent Framework with real-time audio model
/// </summary>
public class RealtimeAudioAgent : IDisposable
{
    private readonly ConfigurationHelper _config;
    private readonly ILogger _logger;
    private readonly AudioCaptureService _audioCapture;
    private readonly AudioPlaybackService _audioPlayback;
    private RealtimeClient? _realtimeClient;
    private RealtimeSession? _session;
    private CancellationTokenSource? _cts;
    private bool _disposed;

    public RealtimeAudioAgent(ConfigurationHelper config, ILogger logger)
    {
        _config = config;
        _logger = logger;
        
        var sampleRate = _config.GetSampleRate();
        var channels = _config.GetChannels();
        var deviceIndex = _config.GetDeviceIndex();

        _audioCapture = new AudioCaptureService(sampleRate, channels, deviceIndex);
        _audioPlayback = new AudioPlaybackService(sampleRate, channels);
    }

    public void Initialize()
    {
        _logger.LogInformation("Initializing Realtime Audio Agent...");

        // Create RealtimeClient based on configuration
        if (_config.IsAzureOpenAI())
        {
            var endpoint = _config.GetAzureOpenAIEndpoint()!;
            var apiKey = _config.GetAzureOpenAIApiKey();

            _logger.LogInformation("Connecting to Azure OpenAI at {Endpoint}", endpoint);

            if (!string.IsNullOrWhiteSpace(apiKey))
            {
                var azureClient = new AzureOpenAIClient(new Uri(endpoint), new ApiKeyCredential(apiKey));
                _realtimeClient = azureClient.GetRealtimeClient();
            }
            else
            {
                var azureClient = new AzureOpenAIClient(new Uri(endpoint), new DefaultAzureCredential());
                _realtimeClient = azureClient.GetRealtimeClient();
            }
        }
        else
        {
            var apiKey = _config.GetOpenAIApiKey()!;
            _logger.LogInformation("Connecting to OpenAI");
            
            // For OpenAI, the RealtimeClient is obtained through Azure.AI.OpenAI package
            // which doesn't have a direct OpenAI client. Use Azure format with api.openai.com
            var openAIClient = new AzureOpenAIClient(new Uri("https://api.openai.com/v1"), new ApiKeyCredential(apiKey));
            _realtimeClient = openAIClient.GetRealtimeClient();
        }

        _logger.LogInformation("RealtimeClient initialized successfully");
    }

    public async Task RunConversationAsync(CancellationToken cancellationToken = default)
    {
        if (_realtimeClient == null)
        {
            throw new InvalidOperationException("Agent not initialized. Call InitializeAsync() first.");
        }

        _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        try
        {
            _logger.LogInformation("Starting realtime conversation session...");

            // Determine model name
            var modelName = _config.IsAzureOpenAI() 
                ? _config.GetAzureOpenAIDeployment()! 
                : _config.GetAudioModel()!;

            _session = await _realtimeClient.StartConversationSessionAsync(model: modelName);
            _logger.LogInformation("Session started with model: {Model}", modelName);

            // Configure session options
            var sessionOptions = new ConversationSessionOptions
            {
                Instructions = "You are a helpful AI assistant. Respond to the user's questions in a friendly and informative manner.",
                Voice = ConversationVoice.Alloy,
                InputAudioFormat = RealtimeAudioFormat.Pcm16,
                OutputAudioFormat = RealtimeAudioFormat.Pcm16,
                InputTranscriptionOptions = new()
                {
                    Model = "whisper-1"
                }
            };

            await _session.ConfigureConversationSessionAsync(sessionOptions);
            _logger.LogInformation("Session configured successfully");

            // Initialize audio playback
            _audioPlayback.Initialize();
            _logger.LogInformation("Audio playback initialized");

            // Create memory stream to buffer audio
            var audioStream = new MemoryStream();
            
            // Set up audio capture callback
            _audioCapture.AudioDataAvailable += (sender, audioData) =>
            {
                try
                {
                    if (!_cts.Token.IsCancellationRequested)
                    {
                        audioStream.Write(audioData, 0, audioData.Length);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error buffering audio data");
                }
            };

            // Start audio capture
            _audioCapture.StartCapture();
            _logger.LogInformation("Audio capture started. Speak into your microphone...");

            // Start sending audio in background
            _ = Task.Run(async () =>
            {
                try
                {
                    audioStream.Position = 0;
                    await _session.SendInputAudioAsync(audioStream, _cts.Token);
                }
                catch (OperationCanceledException)
                {
                    // Expected on cancellation
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error sending audio stream");
                }
            }, _cts.Token);

            Console.WriteLine("\n🎙️  Listening... (Press Ctrl+C to stop)\n");

            // Process updates from the session
            await ProcessSessionUpdatesAsync(_cts.Token);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Conversation cancelled by user");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during conversation");
            throw;
        }
        finally
        {
            _audioCapture.StopCapture();
            _logger.LogInformation("Audio capture stopped");
        }
    }

    private async Task ProcessSessionUpdatesAsync(CancellationToken cancellationToken)
    {
        if (_session == null) return;

        try
        {
            await foreach (var update in _session.ReceiveUpdatesAsync(cancellationToken))
            {
                switch (update)
                {
                    case ConversationSessionStartedUpdate:
                        _logger.LogInformation("Conversation session started");
                        break;

                    case InputAudioSpeechStartedUpdate:
                        Console.Write("\n🗣️  You: ");
                        break;

                    case InputAudioTranscriptionDeltaUpdate userDelta:
                        Console.Write(userDelta.Delta);
                        break;

                    case InputAudioTranscriptionFinishedUpdate userDone:
                        Console.WriteLine($"\n    (transcript: {userDone.Transcript})");
                        break;

                    case InputAudioSpeechFinishedUpdate:
                        // Speech ended
                        break;

                    case OutputStreamingStartedUpdate:
                        Console.Write("\n🤖  Assistant: ");
                        break;

                    case OutputDeltaUpdate delta:
                        // Handle text delta
                        if (!string.IsNullOrWhiteSpace(delta.Text))
                        {
                            Console.Write(delta.Text);
                        }
                        
                        // Handle audio delta
                        if (delta.AudioBytes != null && delta.AudioBytes.Length > 0)
                        {
                            _audioPlayback.EnqueueAudio(delta.AudioBytes.ToArray());
                        }
                        break;

                    case OutputStreamingFinishedUpdate:
                        Console.WriteLine("\n");
                        break;

                    default:
                        // Handle other update types if needed
                        break;
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Expected when cancellation is requested
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        _cts?.Cancel();
        _cts?.Dispose();
        _audioCapture?.Dispose();
        _audioPlayback?.Dispose();

        if (_session is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
}
