using AgentFx.RealtimeAudioConsole.Agents;
using AgentFx.RealtimeAudioConsole.Audio;
using AgentFx.RealtimeAudioConsole.Configuration;
using Microsoft.Extensions.Logging;

#pragma warning disable OPENAI002

Console.WriteLine("==============================================");
Console.WriteLine("   Microsoft Agent Framework Realtime Audio");
Console.WriteLine("==============================================\n");

// Set up logging
using var loggerFactory = LoggerFactory.Create(builder =>
{
    builder
        .AddConsole()
        .SetMinimumLevel(LogLevel.Information);
});

var logger = loggerFactory.CreateLogger<Program>();

try
{
    // Load configuration
    logger.LogInformation("Loading configuration...");
    var config = new ConfigurationHelper();

    // Validate configuration
    try
    {
        config.ValidateConfiguration();
        logger.LogInformation("Configuration validated successfully");
    }
    catch (InvalidOperationException ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("\n❌ Configuration Error:");
        Console.WriteLine(ex.Message);
        Console.ResetColor();
        Console.WriteLine("\nPlease configure the required settings using user secrets:");
        Console.WriteLine("\n  dotnet user-secrets init");
        Console.WriteLine("  dotnet user-secrets set \"AUDIO_MODEL\" \"gpt-4o-realtime-preview\"");
        Console.WriteLine("  dotnet user-secrets set \"OPENAI_API_KEY\" \"<your-key>\"");
        Console.WriteLine("\nOr for Azure OpenAI:");
        Console.WriteLine("  dotnet user-secrets set \"AUDIO_MODEL\" \"gpt-4o-realtime-preview\"");
        Console.WriteLine("  dotnet user-secrets set \"AZURE_OPENAI_API_KEY\" \"<your-key>\"");
        Console.WriteLine("  dotnet user-secrets set \"AZURE_OPENAI_ENDPOINT\" \"https://your-endpoint.openai.azure.com\"");
        Console.WriteLine("  dotnet user-secrets set \"AZURE_OPENAI_DEPLOYMENT\" \"<deployment-name>\"");
        Console.WriteLine();
        return 1;
    }

    // List available audio devices
    AudioCaptureService.ListAudioDevices();

    // Initialize and run the agent
    using var agent = new RealtimeAudioAgent(config, logger);
    agent.Initialize();

    // Set up cancellation for Ctrl+C
    using var cts = new CancellationTokenSource();
    Console.CancelKeyPress += (sender, e) =>
    {
        e.Cancel = true;
        cts.Cancel();
        Console.WriteLine("\n\n👋 Stopping conversation...");
    };

    // Run the conversation
    await agent.RunConversationAsync(cts.Token);

    Console.WriteLine("\n✅ Conversation ended successfully");
    return 0;
}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"\n❌ Fatal error: {ex.Message}");
    Console.ResetColor();
    logger.LogError(ex, "Fatal error occurred");
    return 1;
}
