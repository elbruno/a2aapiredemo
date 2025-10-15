using Microsoft.Extensions.Configuration;

namespace AgentFx.RealtimeAudioConsole.Configuration;

/// <summary>
/// Helper class for managing application configuration from user secrets, environment variables, and appsettings.json
/// </summary>
public class ConfigurationHelper
{
    private readonly IConfiguration _configuration;

    public ConfigurationHelper()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
            .AddUserSecrets<Program>(optional: true)
            .AddEnvironmentVariables();

        _configuration = builder.Build();
    }

    public string? GetAudioModel() => GetValue("AUDIO_MODEL");
    public string? GetOpenAIApiKey() => GetValue("OPENAI_API_KEY");
    public string? GetAzureOpenAIApiKey() => GetValue("AZURE_OPENAI_API_KEY");
    public string? GetAzureOpenAIEndpoint() => GetValue("AZURE_OPENAI_ENDPOINT");
    public string? GetAzureOpenAIDeployment() => GetValue("AZURE_OPENAI_DEPLOYMENT");
    public int GetSampleRate() => int.TryParse(GetValue("SAMPLE_RATE"), out var rate) ? rate : 24000;
    public int GetChannels() => int.TryParse(GetValue("CHANNELS"), out var channels) ? channels : 1;
    public int GetDeviceIndex() => int.TryParse(GetValue("DEVICE_INDEX"), out var index) ? index : -1;

    private string? GetValue(string key) => _configuration[key];

    public void ValidateConfiguration()
    {
        var errors = new List<string>();

        var audioModel = GetAudioModel();
        if (string.IsNullOrWhiteSpace(audioModel))
        {
            errors.Add("AUDIO_MODEL is required");
        }

        var hasOpenAI = !string.IsNullOrWhiteSpace(GetOpenAIApiKey());
        var hasAzureOpenAI = !string.IsNullOrWhiteSpace(GetAzureOpenAIApiKey()) 
                            && !string.IsNullOrWhiteSpace(GetAzureOpenAIEndpoint())
                            && !string.IsNullOrWhiteSpace(GetAzureOpenAIDeployment());

        if (!hasOpenAI && !hasAzureOpenAI)
        {
            errors.Add("Either OPENAI_API_KEY or (AZURE_OPENAI_API_KEY + AZURE_OPENAI_ENDPOINT + AZURE_OPENAI_DEPLOYMENT) is required");
        }

        if (errors.Any())
        {
            throw new InvalidOperationException(
                "Configuration validation failed:\n" + string.Join("\n", errors.Select(e => $"  - {e}")));
        }
    }

    public bool IsAzureOpenAI() => !string.IsNullOrWhiteSpace(GetAzureOpenAIApiKey());
}
