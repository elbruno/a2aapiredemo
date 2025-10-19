using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.Extensions.Configuration;

/// <summary>
/// Basic Semantic Kernel agent demonstrating simple chat interaction using GitHub Models.
/// This is the "before" version showing SK patterns.
/// </summary>
class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("=== Semantic Kernel Basic Agent (GitHub Models) ===");
        Console.WriteLine("Type 'exit' to quit\n");

        try
        {
            // Get GitHub token from environment variable or User Secrets
            var githubToken = Environment.GetEnvironmentVariable("GITHUB_TOKEN");
            if (string.IsNullOrEmpty(githubToken))
            {
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false)
                    .AddUserSecrets<Program>()
                    .Build();

                githubToken = configuration["GITHUB_TOKEN"]
                    ?? throw new InvalidOperationException(
                        "GITHUB_TOKEN not found. Run: dotnet user-secrets set \"GITHUB_TOKEN\" \"your-token\"");
            }

            var model = "gpt-4o-mini";

            Console.WriteLine($"Using GitHub Models with model: {model}");
            Console.WriteLine($"Creating Semantic Kernel agent...\n");

            // SK: Build Kernel with GitHub Models endpoint
            var kernel = Kernel.CreateBuilder()
                .AddOpenAIChatCompletion(
                    modelId: model,
                    apiKey: githubToken,
                    endpoint: new Uri("https://models.github.ai/inference"))
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
            Console.WriteLine("\nMake sure you've configured your GitHub token:");
            Console.WriteLine("  dotnet user-secrets set \"GITHUB_TOKEN\" \"your-github-token\"");
            Console.WriteLine("\nGet a token at: https://github.com/settings/tokens");
        }
    }
}
