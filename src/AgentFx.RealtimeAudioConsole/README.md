# Microsoft Agent Framework Realtime Audio Console Demo

A .NET 9 console application demonstrating real-time audio interaction using Microsoft Agent Framework with Azure OpenAI or OpenAI's real-time audio models.

## Description

This console application enables voice-based conversations with AI using real-time audio streaming. It captures audio from your microphone, sends it to the AI model, and plays back the AI's synthesized voice responses in real-time.

### Features

- 🎙️ Real-time microphone audio capture
- 🔊 Real-time audio playback of AI responses
- 📝 Speech-to-text transcription display
- 🤖 Integration with Azure OpenAI or OpenAI real-time models
- ⚙️ User Secrets configuration support
- 🔧 Configurable audio settings (sample rate, channels, device selection)

## Prerequisites

- **.NET 9 SDK** - [Download here](https://dotnet.microsoft.com/download/dotnet/9.0)
- **Audio Drivers** - Native audio input/output devices
- **API Access** - Either:
  - OpenAI API key with access to `gpt-4o-realtime-preview` model
  - Azure OpenAI service with real-time audio deployment

## Configuration

The application uses the following configuration priority:

1. **User Secrets** (recommended for development)
2. **Environment Variables**
3. **appsettings.json** (default fallback values)

### Required Configuration Values

#### For OpenAI:
- `AUDIO_MODEL` - Model name (e.g., "gpt-4o-realtime-preview")
- `OPENAI_API_KEY` - Your OpenAI API key

#### For Azure OpenAI:
- `AUDIO_MODEL` - Model name (e.g., "gpt-4o-realtime-preview")
- `AZURE_OPENAI_API_KEY` - Your Azure OpenAI API key (or use Managed Identity)
- `AZURE_OPENAI_ENDPOINT` - Your Azure OpenAI endpoint (e.g., "https://your-resource.openai.azure.com")
- `AZURE_OPENAI_DEPLOYMENT` - Your deployment name

#### Optional Configuration:
- `SAMPLE_RATE` - Audio sample rate in Hz (default: 24000)
- `CHANNELS` - Number of audio channels (default: 1)
- `DEVICE_INDEX` - Audio input device index (default: -1 for default device)

## Setup Instructions

### 1. Initialize User Secrets

```bash
cd src/AgentFx.RealtimeAudioConsole
dotnet user-secrets init
```

### 2. Configure for OpenAI

```bash
dotnet user-secrets set "AUDIO_MODEL" "gpt-4o-realtime-preview"
dotnet user-secrets set "OPENAI_API_KEY" "sk-your-openai-api-key-here"
```

### 3. Or Configure for Azure OpenAI

```bash
dotnet user-secrets set "AUDIO_MODEL" "gpt-4o-realtime-preview"
dotnet user-secrets set "AZURE_OPENAI_API_KEY" "your-azure-api-key-here"
dotnet user-secrets set "AZURE_OPENAI_ENDPOINT" "https://your-resource.openai.azure.com"
dotnet user-secrets set "AZURE_OPENAI_DEPLOYMENT" "your-deployment-name"
```

### 4. (Optional) Configure Audio Settings

```bash
# Set custom sample rate
dotnet user-secrets set "SAMPLE_RATE" "16000"

# Set specific audio device (run app first to see device list)
dotnet user-secrets set "DEVICE_INDEX" "0"
```

## Running the Application

### Build and Run

```bash
cd src/AgentFx.RealtimeAudioConsole
dotnet build
dotnet run
```

### Usage

1. The application will start and list available audio input devices
2. It will connect to the configured AI service
3. Start speaking into your microphone
4. The AI will respond in real-time with synthesized voice
5. Transcripts of your speech and the AI's text responses will be shown in the console
6. Press `Ctrl+C` to stop the conversation

### Example Output

```
==============================================
   Microsoft Agent Framework Realtime Audio
==============================================

=== Available Audio Input Devices ===
Device 0: Microphone (USB Audio Device) (2 channels)
Device 1: Built-in Microphone (2 channels)
=====================================

info: Loading configuration...
info: Configuration validated successfully
info: Initializing Realtime Audio Agent...
info: Connecting to Azure OpenAI at https://your-resource.openai.azure.com
info: RealtimeClient initialized successfully
info: Starting realtime conversation session...
info: Session started with model: gpt-4o-realtime-preview
info: Session configured successfully
info: Audio playback initialized
info: Audio capture started. Speak into your microphone...

🎙️  Listening... (Press Ctrl+C to stop)

🗣️  You: What is the weather like today?
🤖  Assistant: I don't have access to real-time weather data...
```

## Architecture

### Project Structure

```
AgentFx.RealtimeAudioConsole/
├── Program.cs                      # Main entry point
├── Agents/
│   └── RealtimeAudioAgent.cs      # Core agent implementation
├── Audio/
│   ├── AudioCaptureService.cs     # Microphone capture
│   └── AudioPlaybackService.cs    # Speaker playback
├── Configuration/
│   └── ConfigurationHelper.cs     # Configuration management
├── appsettings.json               # Default configuration
└── README.md                      # This file
```

### Key Components

- **RealtimeAudioAgent**: Orchestrates the real-time conversation, manages the RealtimeClient session, and coordinates audio I/O
- **AudioCaptureService**: Uses NAudio to capture PCM16 audio from the microphone at 24kHz
- **AudioPlaybackService**: Uses NAudio to play PCM16 audio through speakers
- **ConfigurationHelper**: Manages configuration from multiple sources with validation

### Audio Format

- **Format**: PCM16 (16-bit linear PCM)
- **Sample Rate**: 24kHz (configurable)
- **Channels**: 1 (mono, configurable)
- **Encoding**: Little-endian signed integer

## Troubleshooting

### No Audio Devices Found

If no microphone is detected:
- Ensure your microphone is properly connected and recognized by your OS
- Check system audio settings
- Try running the app as administrator (Windows) or with proper permissions (Linux/macOS)

### Configuration Errors

If you see configuration validation errors:
- Verify all required secrets are set: `dotnet user-secrets list`
- Check for typos in secret keys
- Ensure API keys are valid and not expired

### Connection Errors

If the app fails to connect to the AI service:
- Verify your API key is correct
- For Azure OpenAI, ensure the endpoint URL is correct
- Check your network connection and firewall settings
- Verify the deployment name matches your Azure resource

### Audio Quality Issues

If audio quality is poor:
- Try adjusting the `SAMPLE_RATE` setting (16000, 24000, or 48000)
- Check your microphone hardware and settings
- Reduce background noise
- Ensure sufficient network bandwidth

## Additional Resources

- [Azure OpenAI Service Documentation](https://learn.microsoft.com/en-us/azure/ai-services/openai/)
- [OpenAI Realtime API Documentation](https://platform.openai.com/docs/guides/realtime)
- [Microsoft Agent Framework Overview](https://learn.microsoft.com/en-us/agent-framework/overview/agent-framework-overview)
- [NAudio Documentation](https://github.com/naudio/NAudio)

## License

This project follows the same license as the parent repository.
