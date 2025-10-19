# Module 08: Streaming Responses

**Duration**: 15 minutes  
**Level**: Intermediate  

---

## Learning Objectives

- Stream responses in real-time with Agent Framework
- Use `RunStreamingAsync` method
- Display progressive output
- Handle streaming with GitHub Models

---

## Streaming Pattern

### Agent Framework Streaming

```csharp
using Microsoft.Agents.AI;

await foreach (var update in agent.RunStreamingAsync("Write a long story"))
{
    Console.Write(update.Text);
}
```

**That's it!** Text appears progressively as the AI generates it.

---

## Complete Example with GitHub Models

```csharp
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI.Chat;
using System.ClientModel;

// Setup agent (from previous modules)
var githubToken = Environment.GetEnvironmentVariable("GITHUB_TOKEN");
var chatClient = new ChatClient(
    "gpt-4o-mini",
    new ApiKeyCredential(githubToken!),
    new OpenAIClientOptions { Endpoint = new Uri("https://models.github.ai/inference") });

var agent = new ChatClientAgent(
    chatClient.AsIChatClient(),
    new ChatClientAgentOptions 
    { 
        Name = "StoryTeller",
        Instructions = "You are a creative storyteller."
    });

// Stream a response
Console.WriteLine("Streaming response:");
var fullText = new System.Text.StringBuilder();

await foreach (var update in agent.RunStreamingAsync("Tell me a short story about AI"))
{
    Console.Write(update.Text);
    fullText.Append(update.Text);
}

Console.WriteLine($"\n\nComplete text length: {fullText.Length} characters");
```

---

## Streaming vs Non-Streaming

### Non-Streaming (Wait for complete response)
```csharp
var response = await agent.RunAsync("Write a story");
Console.WriteLine(response.Text); // All at once
```

### Streaming (Progressive display)
```csharp
await foreach (var update in agent.RunStreamingAsync("Write a story"))
{
    Console.Write(update.Text); // Word by word
}
```

---

## Use Cases for Streaming

- **Long responses** - Show progress to user
- **Interactive UX** - More engaging experience
- **Real-time applications** - Chat interfaces
- **Perceived speed** - Feels faster

---

## Building Full Response

```csharp
var fullResponse = new StringBuilder();

await foreach (var update in agent.RunStreamingAsync("Explain machine learning"))
{
    var text = update.Text;
    Console.Write(text);
    fullResponse.Append(text);
}

// Use complete response
string complete = fullResponse.ToString();
```

---

## Best Practices

1. **Use for long outputs** - Stories, explanations, code generation
2. **Show progress** - Better UX for users
3. **Collect if needed** - Build full response while streaming
4. **Handle errors** - Wrap in try-catch

---

## Next: [Module 09: Dependency Injection](../09-Dependency-Injection/)
