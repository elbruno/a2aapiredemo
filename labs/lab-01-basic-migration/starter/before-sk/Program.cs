using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using System.ComponentModel;

Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.WriteLine("=== Semantic Kernel Weather Bot ===");
Console.WriteLine("Type 'exit' to quit.\n");

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true)
    .AddUserSecrets<Program>()
    .Build();

var githubToken = configuration["GITHUB_TOKEN"]
    ?? throw new InvalidOperationException("Configure GITHUB_TOKEN using dotnet user-secrets.");
var deploymentName = configuration["OpenAI:ChatDeploymentName"] ?? "gpt-4o-mini";
var endpoint = configuration["OpenAI:Endpoint"] ?? "https://models.github.ai/inference";

var kernelBuilder = Kernel.CreateBuilder();
kernelBuilder.AddOpenAIChatCompletion(
    modelId: deploymentName,
    apiKey: githubToken,
    endpoint: new Uri(endpoint));

var kernel = kernelBuilder.Build();

kernel.ImportPluginFromType<WeatherPlugin>();

var agent = new ChatCompletionAgent
{
    Kernel = kernel,
    Name = "WeatherBot",
    Instructions = "You are a helpful weather assistant. Use tools when appropriate to answer questions."
};

var thread = new ChatHistory();
thread.AddSystemMessage("You are a weather assistant that can convert temperatures and report conditions.");

while (true)
{
    Console.Write("You: ");
    var input = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(input) || input.Equals("exit", StringComparison.OrdinalIgnoreCase))
    {
        Console.WriteLine("WeatherBot: Goodbye!");
        break;
    }

    thread.AddUserMessage(input);

    await foreach (var update in agent.InvokeAsync(thread))
    {
        dynamic dynamicUpdate = update; // Use dynamic to avoid tight coupling to AgentResponseItem internals
        if (dynamicUpdate.Content is IEnumerable<ChatMessageContent> messages)
        {
            foreach (var message in messages)
            {
                if (message.Role == AuthorRole.Assistant && !string.IsNullOrWhiteSpace(message.Content))
                {
                    Console.WriteLine($"WeatherBot: {message.Content}\n");
                    thread.AddAssistantMessage(message.Content);
                }
            }
        }
        else if (dynamicUpdate.Message is ChatMessageContent singleMessage)
        {
            if (singleMessage.Role == AuthorRole.Assistant && !string.IsNullOrWhiteSpace(singleMessage.Content))
            {
                Console.WriteLine($"WeatherBot: {singleMessage.Content}\n");
                thread.AddAssistantMessage(singleMessage.Content);
            }
        }
    }
}

/// <summary>
/// Semantic Kernel plugin providing weather related utilities.
/// </summary>
public sealed class WeatherPlugin
{
    [KernelFunction]
    [Description("Get weather for a city")]
    public string GetWeather(string city)
    {
        return $"Weather in {city}: Sunny, 72°F";
    }

    [KernelFunction]
    [Description("Convert Celsius to Fahrenheit")]
    public string ConvertToFahrenheit(double celsius)
    {
        var fahrenheit = (celsius * 9 / 5) + 32;
        return $"{celsius:F1}°C = {fahrenheit:F1}°F";
    }
}
