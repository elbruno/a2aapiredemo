using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using OpenAI;
using OpenAI.Chat;
using Azure.AI.OpenAI;
using Azure.AI.Inference;
using Azure.Identity;
using System.ClientModel;

Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.WriteLine("=== Agent Framework Weather Bot ===");
Console.WriteLine("Type 'exit' to quit.\n");

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true)
    .AddUserSecrets<Program>()
    .Build();

var deploymentName = configuration["OpenAI:ChatDeploymentName"] ?? "gpt-4o-mini";

// Create IChatClient based on configuration
IChatClient chatClient = CreateChatClient(configuration, deploymentName);

// Local functions replace the old plugin class.
string GetWeather(string city) => $"Weather in {city}: Sunny, 72°F";
string ConvertToFahrenheit(double celsius)
{
    var fahrenheit = (celsius * 9 / 5) + 32;
    return $"{celsius:F1}°C = {fahrenheit:F1}°F";
}

var agent = new ChatClientAgent(
    chatClient,
    new ChatClientAgentOptions
    {
        Name = "WeatherBot",
        Instructions = "You are a helpful weather assistant. Use the provided tools to answer questions."
    });

while (true)
{
    Console.Write("You: ");
    var input = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(input) || input.Equals("exit", StringComparison.OrdinalIgnoreCase))
    {
        Console.WriteLine("WeatherBot: Goodbye!");
        break;
    }

    try
    {
        AgentRunResponse response = await agent.RunAsync(input);
        Console.WriteLine($"WeatherBot: {response.Text}\n");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}\n");
    }
}

static IChatClient CreateChatClient(IConfiguration configuration, string deploymentName)
{
    var provider = configuration["AI:Provider"] ?? "GitHubModels"; // Default to GitHub Models

    return provider.ToUpperInvariant() switch
    {
        "GITHUBMODELS" => CreateGitHubModelsClient(configuration, deploymentName),
        "OPENAI" => CreateOpenAIClient(configuration, deploymentName),
        "AZUREAIFOUNDRY" => CreateAzureAIFoundryClient(configuration, deploymentName),
        _ => throw new InvalidOperationException($"Unknown AI provider: {provider}. Valid options: GitHubModels, OpenAI, AzureAIFoundry")
    };
}

static IChatClient CreateGitHubModelsClient(IConfiguration configuration, string deploymentName)
{
    var githubToken = configuration["GITHUB_TOKEN"]
        ?? throw new InvalidOperationException("GITHUB_TOKEN not configured for GitHub Models");
    var endpoint = configuration["OpenAI:Endpoint"] ?? "https://models.github.ai/inference";

    return new ChatClient(
            deploymentName,
            new ApiKeyCredential(githubToken),
            new OpenAIClientOptions { Endpoint = new Uri(endpoint) })
        .AsIChatClient();
}

static IChatClient CreateOpenAIClient(IConfiguration configuration, string deploymentName)
{
    var apiKey = configuration["OpenAI:ApiKey"]
        ?? throw new InvalidOperationException("OpenAI:ApiKey not configured for OpenAI");

    return new ChatClient(deploymentName, new ApiKeyCredential(apiKey))
        .AsIChatClient();
}

static IChatClient CreateAzureAIFoundryClient(IConfiguration configuration, string deploymentName)
{
    var endpoint = configuration["AzureAIFoundry:Endpoint"]
        ?? throw new InvalidOperationException("AzureAIFoundry:Endpoint not configured");
    var apiKey = configuration["AzureAIFoundry:ApiKey"];

    // Use managed identity (DefaultAzureCredential) by default, or API key if provided
    if (!string.IsNullOrEmpty(apiKey))
    {
        return new ChatCompletionsClient(
                new Uri(endpoint),
                new Azure.AzureKeyCredential(apiKey))
            .AsIChatClient(deploymentName);
    }
    else
    {
        return new ChatCompletionsClient(
                new Uri(endpoint),
                new DefaultAzureCredential())
            .AsIChatClient(deploymentName);
    }
}
