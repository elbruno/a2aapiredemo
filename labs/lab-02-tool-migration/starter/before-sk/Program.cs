using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;

Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.WriteLine("=== Semantic Kernel Customer Management Bot ===");
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

// Create database service and plugin
var database = new DatabaseService();
var plugin = new CustomerPlugin(database);
kernel.ImportPluginFromObject(plugin, "CustomerManagement");

var agent = new ChatCompletionAgent
{
    Kernel = kernel,
    Name = "CustomerBot",
    Instructions = "You are a helpful customer service assistant. Use the available tools to help with customer management tasks."
};

var thread = new ChatHistory();
thread.AddSystemMessage("You are a customer service assistant that can manage customer records.");

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

    thread.AddUserMessage(input);

    await foreach (var update in agent.InvokeAsync(thread))
    {
        dynamic dynamicUpdate = update;
        if (dynamicUpdate.Content is IEnumerable<ChatMessageContent> messages)
        {
            foreach (var message in messages)
            {
                if (message.Role == AuthorRole.Assistant && !string.IsNullOrWhiteSpace(message.Content))
                {
                    Console.WriteLine($"CustomerBot: {message.Content}\n");
                    thread.AddAssistantMessage(message.Content);
                }
            }
        }
        else if (dynamicUpdate.Message is ChatMessageContent singleMessage)
        {
            if (singleMessage.Role == AuthorRole.Assistant && !string.IsNullOrWhiteSpace(singleMessage.Content))
            {
                Console.WriteLine($"CustomerBot: {singleMessage.Content}\n");
                thread.AddAssistantMessage(singleMessage.Content);
            }
        }
    }
}
