using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using OpenAI;
using OpenAI.Chat;
using System.ClientModel;

/// <summary>
/// Basic Agent Framework example demonstrating simple chat interaction using GitHub Models.
/// This is the "after" version showing modern Microsoft.Extensions.AI patterns.
/// </summary>
class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("=== Agent Framework Basic Agent (GitHub Models) ===");
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
            Console.WriteLine($"Creating Agent Framework chat client...\n");

            // AF: Create IChatClient using GitHub Models endpoint
            IChatClient chatClient =
                new ChatClient(
                        model,
                        new ApiKeyCredential(githubToken),
                        new OpenAIClientOptions { Endpoint = new Uri("https://models.github.ai/inference") })
                    .AsIChatClient();

            // AF: Create AI agent
            AIAgent writer = new ChatClientAgent(
                chatClient,
                new ChatClientAgentOptions
                {
                    Name = "Assistant",
                    Instructions = "You are a helpful AI assistant."
                });

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

                // AF: Run agent and get response
                AgentRunResponse response = await writer.RunAsync(input);

                Console.WriteLine($"Agent: {response.Text}\n");
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
