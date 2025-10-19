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
Console.WriteLine("=== Agent Framework Customer Management Bot ===");
Console.WriteLine("Type 'exit' to quit.\n");

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true)
    .AddUserSecrets<Program>()
    .Build();

var deploymentName = configuration["OpenAI:ChatDeploymentName"] ?? "gpt-4o-mini";

// Create IChatClient based on configuration
IChatClient chatClient = CreateChatClient(configuration, deploymentName);

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

// Create database service
var database = new DatabaseService();

// Local async functions replace the plugin class - using closures to access database
async Task<string> GetCustomer(int customerId) =>
    await database.GetCustomerAsync(customerId);

async Task<string> CreateCustomer(string name, string email, string city) =>
    await database.CreateCustomerAsync(name, email, city);

async Task<string> UpdateCustomer(int customerId, string? email = null, string? city = null) =>
    await database.UpdateCustomerAsync(customerId, email, city);

async Task<string> SearchCustomers(string? city = null, string? nameContains = null) =>
    await database.SearchCustomersAsync(city, nameContains);

async Task<string> DeleteCustomer(int customerId) =>
    await database.DeleteCustomerAsync(customerId);

// Note: Agent Framework currently has limited support for function registration
// This is a simplified implementation for demonstration purposes
var agent = new ChatClientAgent(
    chatClient,
    new ChatClientAgentOptions
    {
        Name = "CustomerBot",
        Instructions = "You are a helpful customer service assistant. Use the available tools to help with customer management tasks."
    });

Console.WriteLine("Try commands like:");
Console.WriteLine("- Get customer 123");
Console.WriteLine("- Create a new customer named David Lee with email david@example.com in Boston");
Console.WriteLine("- Search for customers in Seattle");
Console.WriteLine("- Update email for customer 456 to newemail@example.com\n");

while (true)
{
    Console.Write("You: ");
    var input = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(input) || input.Equals("exit", StringComparison.OrdinalIgnoreCase))
    {
        Console.WriteLine("CustomerBot: Goodbye!");
        break;
    }

    try
    {
        AgentRunResponse response = await agent.RunAsync(input);
        Console.WriteLine($"CustomerBot: {response.Text}\n");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}\n");
    }
}
