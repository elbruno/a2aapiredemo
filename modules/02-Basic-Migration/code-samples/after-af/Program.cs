using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using OpenAI;

/// <summary>
/// Basic Agent Framework example demonstrating simple chat interaction.
/// This is the "after" version showing modern Microsoft.Extensions.AI patterns.
/// </summary>
class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("=== Agent Framework Basic Agent ===");
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
            Console.WriteLine($"Creating Agent Framework chat client...\n");

            // AF: Create IChatClient directly using OpenAI
            IChatClient chatClient = new OpenAIClient(apiKey)
                .AsChatClient(model);

            // AF: Create conversation messages (thread management)
            var messages = new List<ChatMessage>
            {
                new(ChatRole.System, "You are a helpful AI assistant.")
            };

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

                // AF: Add user message
                messages.Add(new ChatMessage(ChatRole.User, input));

                // AF: Get chat completion using CompleteAsync
                var response = await chatClient.CompleteAsync(messages);

                // AF: Add assistant response to conversation
                var assistantMessage = response.Message.Text ?? string.Empty;
                messages.Add(response.Message);

                Console.WriteLine($"Agent: {assistantMessage}\n");
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
