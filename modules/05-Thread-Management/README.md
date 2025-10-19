# Module 05: Thread Management

**Duration**: 15 minutes  
**Level**: Intermediate  
**Prerequisites**: Module 04 completed

---

## Learning Objectives

- Understand thread management in Agent Framework
- Learn to maintain conversation context
- Work with agent run responses
- Manage conversation lifecycle

---

## Overview

In Agent Framework, conversations are managed through agent interactions. Unlike Semantic Kernel's provider-specific thread types, Agent Framework uses a simplified approach where the agent manages the conversation context.

---

## Semantic Kernel Approach

### Provider-Specific Threads

```csharp
// SK: Provider-specific thread types
var thread = new OpenAIAssistantAgentThread("thread-id");
// or
var thread = new AzureAIAgentThread("thread-id");

// Requires managing thread lifecycle manually
```

---

## Agent Framework Approach

### Simplified Conversation Management

```csharp
// AF: Agent manages conversation through RunAsync
var response1 = await agent.RunAsync("Hello!");
var response2 = await agent.RunAsync("How are you?");

// Context is maintained automatically
```

**Key Point**: The agent maintains conversation context internally. You don't need to manage thread objects explicitly for simple scenarios.

---

## Multi-Turn Conversations

### Basic Pattern

```csharp
using Microsoft.Agents.AI;

// Create agent (from Module 04)
var agent = new ChatClientAgent(chatClient, options);

// First message
var response1 = await agent.RunAsync("What is machine learning?");
Console.WriteLine($"Agent: {response1.Text}");

// Follow-up (context maintained)
var response2 = await agent.RunAsync("Give me an example");
Console.WriteLine($"Agent: {response2.Text}");

// Another follow-up
var response3 = await agent.RunAsync("Explain that further");
Console.WriteLine($"Agent: {response3.Text}");
```

---

## Working with Responses

### AgentRunResponse Properties

```csharp
var response = await agent.RunAsync("Tell me a joke");

// Access response text
Console.WriteLine(response.Text);

// Check if response is complete
if (response.IsComplete)
{
    Console.WriteLine("Response finished");
}

// Access raw message data if needed
var message = response.Message;
```

---

## Complete Example with GitHub Models

```csharp
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using OpenAI.Chat;
using System.ClientModel;

class Program
{
    static async Task Main(string[] args)
    {
        // Configuration
        var config = new ConfigurationBuilder()
            .AddUserSecrets<Program>()
            .Build();

        var githubToken = config["GITHUB_TOKEN"] 
            ?? throw new InvalidOperationException("GITHUB_TOKEN not set");

        // Create agent with GitHub Models
        var chatClient = new ChatClient(
            "gpt-4o-mini",
            new ApiKeyCredential(githubToken),
            new OpenAIClientOptions { 
                Endpoint = new Uri("https://models.github.ai/inference") 
            });

        var agent = new ChatClientAgent(
            chatClient.AsIChatClient(),
            new ChatClientAgentOptions
            {
                Name = "ConversationBot",
                Instructions = "You are a helpful assistant. Maintain conversation context."
            });

        // Multi-turn conversation
        Console.WriteLine("=== Multi-Turn Conversation Demo ===\n");

        var response1 = await agent.RunAsync("My name is Alice");
        Console.WriteLine($"Agent: {response1.Text}\n");

        var response2 = await agent.RunAsync("What did I just tell you?");
        Console.WriteLine($"Agent: {response2.Text}\n");

        var response3 = await agent.RunAsync("Tell me a fact about my name");
        Console.WriteLine($"Agent: {response3.Text}\n");
    }
}
```

**Output Example:**
```
=== Multi-Turn Conversation Demo ===

Agent: Nice to meet you, Alice! How can I help you today?

Agent: You told me that your name is Alice.

Agent: Alice is a classic name with roots in Old German, meaning "noble" or "of noble kind." It became popular through Lewis Carroll's famous book "Alice's Adventures in Wonderland."
```

---

## Conversation Patterns

### Pattern 1: Interactive Chat Loop

```csharp
while (true)
{
    Console.Write("You: ");
    var input = Console.ReadLine();
    
    if (string.IsNullOrEmpty(input) || input.ToLower() == "exit")
        break;
    
    var response = await agent.RunAsync(input);
    Console.WriteLine($"Agent: {response.Text}\n");
}
```

### Pattern 2: Contextual Follow-ups

```csharp
// Initial query
var response1 = await agent.RunAsync("List three planets");
Console.WriteLine(response1.Text);

// Follow-up using context
var response2 = await agent.RunAsync("Tell me more about the second one");
Console.WriteLine(response2.Text);
```

---

## Context Management

### How Context Works

Agent Framework automatically maintains conversation history for you:

1. Each `RunAsync` call includes previous conversation context
2. The agent remembers what was discussed
3. You can reference previous messages naturally

### Resetting Context

To start a fresh conversation, create a new agent instance:

```csharp
// New agent = new conversation
var newAgent = new ChatClientAgent(chatClient, options);
```

---

## Comparison with Semantic Kernel

| Feature | Semantic Kernel | Agent Framework |
|---------|----------------|-----------------|
| **Thread Type** | Provider-specific | Agent-managed |
| **Context** | Manual tracking | Automatic |
| **Initialization** | Create thread object | Just use agent |
| **Cleanup** | Thread disposal | Automatic |

---

## Best Practices

1. **One Agent Per Conversation** - Create separate agents for independent conversations
2. **Let Agent Manage Context** - Don't manually track message history
3. **Use Clear Instructions** - Set system instructions to guide behavior
4. **Handle Long Conversations** - For very long chats, consider context window limits

---

## Common Patterns

### Customer Support Bot

```csharp
var supportAgent = new ChatClientAgent(
    chatClient,
    new ChatClientAgentOptions
    {
        Name = "SupportBot",
        Instructions = "You are a customer support agent. Be helpful and remember the conversation context."
    });

await supportAgent.RunAsync("I have a problem with my order");
await supportAgent.RunAsync("It was supposed to arrive yesterday");
await supportAgent.RunAsync("Order number is 12345");
```

---

## Next Steps

- [Module 06: Tool Registration](../06-Tool-Registration/) - Add function calling
- [Module 07: Invocation Patterns](../07-Invocation-Patterns/) - Advanced usage
- [Module 08: Streaming Responses](../08-Streaming-Responses/) - Real-time responses

---

**Ready for function calling?** Continue to [Module 06: Tool Registration](../06-Tool-Registration/)! 🛠️
