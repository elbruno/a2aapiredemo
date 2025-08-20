using Microsoft.SemanticKernel;

namespace ZavaSemanticKernelProvider;

/// <summary>
/// Simple provider that creates a Semantic Kernel instance configured for Azure OpenAI chat completion.
/// </summary>
public class SemanticKernelProvider
{
    private readonly Kernel _kernel;

    public SemanticKernelProvider(string openAIConnection, string chatDeploymentName = "gpt-5-mini")
    {
        // Parse the connection string into endpoint + apiKey.
        var (endpoint, apiKey) = ParseAzureOpenAIConnection(openAIConnection);

        IKernelBuilder kernelBuilder = Kernel.CreateBuilder();

        // Add Azure OpenAI Chat Completion service using parsed connection data.
        // Removed hard-coded placeholder values and now using provided configuration. (Change)
        kernelBuilder.AddAzureOpenAIChatCompletion(
            deploymentName: chatDeploymentName,
            endpoint: endpoint,
            apiKey: apiKey
        );

        _kernel = kernelBuilder.Build();
    }

    public Kernel? GetKernel() => _kernel;

    /// <summary>
    /// Parses an Azure OpenAI connection string into (endpoint, key).
    /// Accepts minimal form "https://...azure.com/;Key=xyz;" or explicit key/value pairs.
    /// </summary>
    private static (string endpoint, string key) ParseAzureOpenAIConnection(string? connection)
    {
        if (string.IsNullOrWhiteSpace(connection))
            throw new ArgumentException("Azure OpenAI connection string is null or empty.", nameof(connection));

        // Quick path: if it starts with https and contains ';Key=' we treat first segment as endpoint.
        if (connection.StartsWith("https://", StringComparison.OrdinalIgnoreCase) &&
            connection.Contains(";Key=", StringComparison.OrdinalIgnoreCase))
        {
            var parts = connection.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            string? endpointPart = parts.FirstOrDefault(p => p.StartsWith("https://", StringComparison.OrdinalIgnoreCase));
            string? keyPart = parts.FirstOrDefault(p => p.StartsWith("Key=", StringComparison.OrdinalIgnoreCase));

            if (endpointPart is not null && keyPart is not null)
            {
                string ep = endpointPart.Trim();
                string key = keyPart[4..].Trim(); // after "Key="
                return (NormalizeEndpoint(ep), key);
            }
        }

        // General key=value parsing.
        var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var segment in connection.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            int eq = segment.IndexOf('=');
            if (eq < 0)
            {
                // Allow a raw endpoint segment (e.g. just the URL at start).
                if (segment.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                    dict["Endpoint"] = segment;
                continue;
            }
            string key = segment[..eq].Trim();
            string value = segment[(eq + 1)..].Trim();
            if (!string.IsNullOrEmpty(key))
                dict[key] = value;
        }

        if (!dict.TryGetValue("Endpoint", out var endpointValue))
        {
            // Fallback: maybe stored under "Url"
            dict.TryGetValue("Url", out endpointValue);
        }

        if (string.IsNullOrWhiteSpace(endpointValue))
            throw new InvalidOperationException("Endpoint not found in Azure OpenAI connection string.");

        // Key synonyms.
        if (!dict.TryGetValue("Key", out var apiKey) &&
            !dict.TryGetValue("ApiKey", out apiKey) &&
            !dict.TryGetValue("Api-Key", out apiKey))
        {
            throw new InvalidOperationException("API Key not found in Azure OpenAI connection string (expected Key= or ApiKey=).");
        }

        return (NormalizeEndpoint(endpointValue), apiKey);
    }

    private static string NormalizeEndpoint(string endpoint)
    {
        endpoint = endpoint.Trim();
        // Ensure no trailing spaces or accidental query.
        if (!endpoint.EndsWith("/", StringComparison.Ordinal))
            endpoint += "/";
        return endpoint;
    }
}