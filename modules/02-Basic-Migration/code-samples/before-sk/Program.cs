using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.Extensions.Configuration;

/// <summary>
/// Basic Semantic Kernel agent demonstrating simple chat interaction.
/// This is the "before" version showing SK patterns.
/// </summary>
class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("=== Semantic Kernel Basic Agent ===");
        Console.WriteLine("Type 'exit' to quit\n");

        try
        {
            // Load configuration from appsettings.json and User Secrets
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .AddUserSecrets<Program>()
                .Build();

            // Get API key from User Secrets (never hardcode!)
            var apiKey = configuration["OpenAI:ApiKey"] 
                ?? throw new InvalidOperationException(
                    "OpenAI:ApiKey not found. Run: dotnet user-secrets set \"OpenAI:ApiKey\" \"your-key\"");
            
            var model = configuration["OpenAI:ChatDeploymentName"] ?? "gpt-4o";

            Console.WriteLine($"Using model: {model}");
            Console.WriteLine($"Creating Semantic Kernel agent...\n");

            // SK: Build Kernel with OpenAI chat completion
            var kernel = Kernel.CreateBuilder()
                .AddOpenAIChatCompletion(
                    modelId: model,
                    apiKey: apiKey)
                .Build();

            // SK: Get chat completion service
            var chatService = kernel.GetRequiredService<IChatCompletionService>();
            
            // SK: Create chat history
            var history = new ChatHistory();
            history.AddSystemMessage("You are a helpful AI assistant.");

            // Chat loop
            while (true)
            {
                Console.Write("You: ");
                var input = Console.ReadLine();
                
                if (string.IsNullOrWhiteSpace(input) || input.ToLower() == "exit")
                {
                    Console.WriteLine("Goodbye!");
                    break;
                }

                // SK: Add user message to history
                history.AddUserMessage(input);

                // SK: Get chat completion
                var response = await chatService.GetChatMessageContentAsync(
                    history,
                    kernel: kernel);

                // SK: Add assistant response to history
                history.AddAssistantMessage(response.Content ?? string.Empty);

                Console.WriteLine($"Agent: {response.Content}\n");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nError: {ex.Message}");
            Console.WriteLine("\nMake sure you've configured User Secrets:");
            Console.WriteLine("  dotnet user-secrets set \"OpenAI:ApiKey\" \"your-key\"");
            Console.WriteLine("  dotnet user-secrets set \"OpenAI:ChatDeploymentName\" \"gpt-4o\"");
        }
    }
}
