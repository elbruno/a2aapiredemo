# Module 11: Complete Example - Full Chatbot

**Duration**: 20 minutes  
**Level**: Intermediate  

---

## Complete Chatbot with GitHub Models

This module provides a complete, production-ready chatbot example using Agent Framework and GitHub Models.

---

## Full Console Chatbot

```csharp
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using OpenAI.Chat;
using System.ClientModel;

class ChatBot
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("=== AI Chatbot with GitHub Models ===\n");

        // Configuration
        var config = new ConfigurationBuilder()
            .AddUserSecrets<ChatBot>()
            .Build();

        var githubToken = config["GITHUB_TOKEN"] 
            ?? throw new InvalidOperationException("GITHUB_TOKEN not configured");

        // Create chat client with GitHub Models
        var chatClient = new ChatClient(
            "gpt-4o-mini",
            new ApiKeyCredential(githubToken),
            new OpenAIClientOptions { 
                Endpoint = new Uri("https://models.github.ai/inference") 
            });

        // Define tools/functions
        string GetWeather(string location) => 
            $"Weather in {location}: Sunny, 72°F";

        int CalculateSum(int a, int b) => a + b;

        DateTime GetCurrentTime() => DateTime.Now;

        // Create agent with tools
        var agent = new ChatClientAgent(
            chatClient.AsIChatClient(),
            new ChatClientAgentOptions
            {
                Name = "ChatBot",
                Instructions = @"You are a helpful AI assistant powered by GitHub Models. 
                    You can check weather, perform calculations, and tell the time. 
                    Be friendly and helpful.",
                Temperature = 0.7f,
                MaxTokens = 1000,
                Tools = { GetWeather, CalculateSum, GetCurrentTime }
            });

        // Chat loop
        Console.WriteLine("Chat started! (Type 'exit' to quit, 'clear' to start new conversation)\n");

        while (true)
        {
            Console.Write("You: ");
            var input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
                continue;

            if (input.ToLower() == "exit")
            {
                Console.WriteLine("Goodbye!");
                break;
            }

            if (input.ToLower() == "clear")
            {
                // Create new agent for fresh conversation
                agent = new ChatClientAgent(
                    chatClient.AsIChatClient(),
                    new ChatClientAgentOptions
                    {
                        Name = "ChatBot",
                        Instructions = agent.Options.Instructions,
                        Temperature = 0.7f,
                        Tools = { GetWeather, CalculateSum, GetCurrentTime }
                    });
                Console.WriteLine("Conversation cleared.\n");
                continue;
            }

            try
            {
                // Get response
                var response = await agent.RunAsync(input);
                Console.WriteLine($"Bot: {response.Text}\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}\n");
            }
        }
    }
}
```

---

## Features Demonstrated

1. ✅ GitHub Models integration
2. ✅ Function calling (weather, calculator, time)
3. ✅ Conversation context management
4. ✅ Error handling
5. ✅ User secrets configuration
6. ✅ Interactive loop
7. ✅ Conversation reset capability

---

## Running the Example

```bash
# Set up user secrets
dotnet user-secrets init
dotnet user-secrets set "GITHUB_TOKEN" "your-github-token"

# Run
dotnet run
```

---

## Example Session

```
=== AI Chatbot with GitHub Models ===

Chat started! (Type 'exit' to quit, 'clear' to start new conversation)

You: What's the weather in Seattle?
Bot: The weather in Seattle is currently sunny with a temperature of 72°F.

You: What's 25 + 37?
Bot: The sum of 25 and 37 is 62.

You: What time is it?
Bot: The current time is 3:45 PM.

You: Thanks!
Bot: You're welcome! Feel free to ask if you need anything else.

You: exit
Goodbye!
```

---

## Enhancements

### Add Streaming

```csharp
var fullResponse = new StringBuilder();
await foreach (var update in agent.RunStreamingAsync(input))
{
    Console.Write(update.Text);
    fullResponse.Append(update.Text);
}
Console.WriteLine("\n");
```

### Add More Tools

```csharp
string SearchWeb(string query) => $"Search results for: {query}";
string TranslateText(string text, string targetLanguage) => $"Translated: {text}";
```

### Add Conversation History Display

```csharp
List<(string speaker, string message)> history = new();

// After each interaction
history.Add(("You", input));
history.Add(("Bot", response.Text));

// Display history command
if (input.ToLower() == "history")
{
    foreach (var (speaker, message) in history)
        Console.WriteLine($"{speaker}: {message}");
}
```

---

## Next Steps

- [Module 12: Real-World Migrations](../12-Real-World-Migrations/)
- [Module 13: Testing Strategies](../13-Testing-Strategies/)
- [Module 15: ASP.NET Core Integration](../15-ASPNetCore-Integration/)

---

**Congratulations!** You've built a complete chatbot with Agent Framework and GitHub Models! 🎉
