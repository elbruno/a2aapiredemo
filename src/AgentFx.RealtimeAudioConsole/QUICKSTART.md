# Quick Start Guide

This guide will help you get the Microsoft Agent Framework Realtime Audio Console Demo up and running in just a few minutes.

## Step 1: Prerequisites

Ensure you have:
- ✅ .NET 9 SDK installed ([download here](https://dotnet.microsoft.com/download/dotnet/9.0))
- ✅ A working microphone and speakers
- ✅ One of the following:
  - OpenAI API key with access to `gpt-4o-realtime-preview`
  - Azure OpenAI deployment with real-time audio model

## Step 2: Navigate to Project Directory

```bash
cd src/AgentFx.RealtimeAudioConsole
```

## Step 3: Configure API Credentials

### Option A: Using OpenAI

```bash
# Initialize user secrets
dotnet user-secrets init

# Set your OpenAI API key
dotnet user-secrets set "AUDIO_MODEL" "gpt-4o-realtime-preview"
dotnet user-secrets set "OPENAI_API_KEY" "sk-your-api-key-here"
```

### Option B: Using Azure OpenAI

```bash
# Initialize user secrets
dotnet user-secrets init

# Set your Azure OpenAI credentials
dotnet user-secrets set "AUDIO_MODEL" "gpt-4o-realtime-preview"
dotnet user-secrets set "AZURE_OPENAI_API_KEY" "your-azure-api-key"
dotnet user-secrets set "AZURE_OPENAI_ENDPOINT" "https://your-resource.openai.azure.com"
dotnet user-secrets set "AZURE_OPENAI_DEPLOYMENT" "your-deployment-name"
```

## Step 4: Build and Run

```bash
dotnet build
dotnet run
```

## Step 5: Start Talking!

1. The application will list your available audio devices
2. It will connect to the AI service
3. When you see "🎙️ Listening... (Press Ctrl+C to stop)", start speaking
4. The AI will respond with both voice and text
5. Press `Ctrl+C` to stop the conversation

## Example Session

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

🎙️  Listening... (Press Ctrl+C to stop)

🗣️  You: What is the capital of France?
    (transcript: What is the capital of France?)

🤖  Assistant: The capital of France is Paris.

🗣️  You: Tell me a fun fact about it.
    (transcript: Tell me a fun fact about it.)

🤖  Assistant: The Eiffel Tower was only meant to stand for 20 years!
```

## Troubleshooting

### "Configuration Error"
- Make sure you ran `dotnet user-secrets init`
- Verify your secrets are set: `dotnet user-secrets list`
- Check that your API key is valid

### "No audio devices found"
- Ensure your microphone is connected and working
- Check your system audio settings
- Try running with elevated permissions

### Connection fails
- Verify your endpoint URL (Azure OpenAI) or API key (OpenAI)
- Check your network connection
- Ensure your deployment name matches your Azure resource

## Advanced Configuration

### Use a Specific Audio Device

First, run the app to see the device list, then:

```bash
dotnet user-secrets set "DEVICE_INDEX" "0"
```

### Change Audio Quality

```bash
# For lower latency (16kHz)
dotnet user-secrets set "SAMPLE_RATE" "16000"

# For higher quality (48kHz)
dotnet user-secrets set "SAMPLE_RATE" "48000"
```

## Next Steps

- Read the full [README.md](README.md) for detailed documentation
- Explore the source code in the `Agents/`, `Audio/`, and `Configuration/` folders
- Customize the system prompt in `RealtimeAudioAgent.cs`
- Extend with your own AI functions and tools

## Support

For issues or questions:
- Check the [README.md](README.md) troubleshooting section
- Review the repository documentation
- Open an issue on GitHub
