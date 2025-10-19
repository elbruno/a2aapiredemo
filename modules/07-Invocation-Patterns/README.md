# Module 07: Invocation Patterns

**Duration**: 15 minutes  
**Level**: Intermediate  

---

## Learning Objectives

- Understand `RunAsync` vs Semantic Kernel's `InvokeAsync`
- Work with `AgentRunResponse`
- Handle agent execution patterns
- Use GitHub Models effectively

---

## Key Change: InvokeAsync → RunAsync

### Semantic Kernel

```csharp
await foreach (var item in agent.InvokeAsync(thread, "Hello"))
{
    var content = item.Content.GetValue<ChatMessageContent>();
    Console.WriteLine(content.Content);
}
```

### Agent Framework

```csharp
var response = await agent.RunAsync("Hello");
Console.WriteLine(response.Text);
```

**Simpler!** Direct response access, no iteration needed.

---

## Complete Example with GitHub Models

```csharp
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI.Chat;
using System.ClientModel;

// Setup
var githubToken = Environment.GetEnvironmentVariable("GITHUB_TOKEN");
var chatClient = new ChatClient(
    "gpt-4o-mini",
    new ApiKeyCredential(githubToken!),
    new OpenAIClientOptions { Endpoint = new Uri("https://models.github.ai/inference") });

var agent = new ChatClientAgent(
    chatClient.AsIChatClient(),
    new ChatClientAgentOptions { Name = "Assistant" });

// Simple invocation
var response = await agent.RunAsync("Explain quantum computing in one sentence");
Console.WriteLine(response.Text);

// With options
var response2 = await agent.RunAsync(
    "Write a haiku about coding",
    new ChatClientAgentRunOptions
    {
        MaxTokens = 100,
        Temperature = 0.7f
    });
Console.WriteLine(response2.Text);
```

---

## Response Object

```csharp
var response = await agent.RunAsync("Hello");

// Access properties
string text = response.Text;              // Response content
bool complete = response.IsComplete;       // Completion status
var message = response.Message;            // Raw ChatMessage
```

---

## With Run Options

```csharp
var options = new ChatClientAgentRunOptions
{
    MaxTokens = 500,
    Temperature = 0.8f,
    TopP = 0.9f
};

var response = await agent.RunAsync("Tell me a story", options);
```

---

## Comparison

| Feature | Semantic Kernel | Agent Framework |
|---------|----------------|-----------------|
| Method | `InvokeAsync` | `RunAsync` |
| Return | `IAsyncEnumerable` | `Task<AgentRunResponse>` |
| Access | Iterate + extract | Direct `.Text` |

---

## Next: [Module 08: Streaming](../08-Streaming-Responses/)
