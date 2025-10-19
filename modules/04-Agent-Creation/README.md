# Module 04: Agent Creation Simplification

**Duration**: 15 minutes  
**Level**: Beginner  
**Prerequisites**: Module 03 completed, packages updated

---

## Learning Objectives

- Understand the simplified agent creation pattern in Agent Framework
- Learn to use `ChatClientAgent` with GitHub Models
- Replace Kernel-based patterns with direct client instantiation
- Create agents without the Kernel orchestrator

---

## Overview

Agent Framework eliminates the need for the Kernel orchestrator, allowing you to create agents directly from a chat client. This simplification reduces boilerplate and makes the code more intuitive.

---

## Semantic Kernel Approach (Before)

### With Kernel Orchestrator

```csharp
// SK: Multi-step process with Kernel
var kernel = Kernel.CreateBuilder()
    .AddOpenAIChatCompletion("gpt-4o-mini", apiKey)
    .Build();

var agent = new ChatCompletionAgent
{
    Kernel = kernel,
    Name = "Assistant",
    Instructions = "You are a helpful AI assistant."
};
```

**Steps**: Create Kernel → Configure → Create Agent → Assign Kernel

---

## Agent Framework Approach (After)

### Direct Agent Creation with GitHub Models

```csharp
// AF: Direct creation with GitHub Models
var githubToken = configuration["GITHUB_TOKEN"];

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
        Name = "Assistant",
        Instructions = "You are a helpful AI assistant."
    });
```

**Steps**: Create ChatClient → Create Agent

---

## Key Differences

| Aspect | Semantic Kernel | Agent Framework |
|--------|----------------|-----------------|
| **Orchestrator** | Kernel required | No Kernel needed |
| **Steps** | 3-4 steps | 2 steps |
| **Agent Type** | `ChatCompletionAgent` | `ChatClientAgent` |
| **Configuration** | Kernel builder | Direct client options |
| **Endpoint** | Provider-specific | Unified with options |

---

## GitHub Models Integration

### Configuration Setup

```bash
# Set GitHub token in User Secrets
dotnet user-secrets set "GITHUB_TOKEN" "your-github-token"
```

Get your token at: https://github.com/settings/tokens

### Complete Example

```csharp
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using OpenAI.Chat;
using System.ClientModel;

// Load configuration
var configuration = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

var githubToken = configuration["GITHUB_TOKEN"] 
    ?? throw new InvalidOperationException("GITHUB_TOKEN not configured");

// Create ChatClient for GitHub Models
var chatClient = new ChatClient(
    "gpt-4o-mini",
    new ApiKeyCredential(githubToken),
    new OpenAIClientOptions { 
        Endpoint = new Uri("https://models.github.ai/inference") 
    });

// Create AI Agent
var agent = new ChatClientAgent(
    chatClient.AsIChatClient(),
    new ChatClientAgentOptions
    {
        Name = "MyAssistant",
        Instructions = "You are a helpful AI assistant that uses GitHub Models."
    });

// Use the agent
var response = await agent.RunAsync("What is GitHub Models?");
Console.WriteLine(response.Text);
```

---

## Benefits of Simplified Creation

1. **Less Boilerplate** - No Kernel setup required
2. **Clearer Intent** - Direct agent creation
3. **Easier Testing** - Interface-based design
4. **GitHub Models Ready** - Easy free AI model integration

---

## Agent Options

### Basic Options

```csharp
new ChatClientAgentOptions
{
    Name = "Assistant",
    Instructions = "System message/instructions"
}
```

### Advanced Options

```csharp
new ChatClientAgentOptions
{
    Name = "SpecializedAgent",
    Instructions = "You are a specialized assistant.",
    Temperature = 0.7f,
    MaxTokens = 1000
}
```

---

## Pattern Comparison

### Creating Multiple Agents

**Semantic Kernel:**
```csharp
var kernel = Kernel.CreateBuilder()...Build();

var agent1 = new ChatCompletionAgent { Kernel = kernel, Name = "Agent1" };
var agent2 = new ChatCompletionAgent { Kernel = kernel, Name = "Agent2" };
```

**Agent Framework:**
```csharp
var client = new ChatClient(...).AsIChatClient();

var agent1 = new ChatClientAgent(client, new() { Name = "Agent1" });
var agent2 = new ChatClientAgent(client, new() { Name = "Agent2" });
```

---

## Migration Steps

1. ✅ Remove Kernel.CreateBuilder() code
2. ✅ Create ChatClient with GitHub Models endpoint
3. ✅ Replace ChatCompletionAgent with ChatClientAgent
4. ✅ Pass IChatClient to agent constructor
5. ✅ Update agent options structure

---

## Common Issues

### Issue: "Kernel not found"

**Before (SK):**
```csharp
var agent = new ChatCompletionAgent { Kernel = kernel };
```

**After (AF):**
```csharp
var agent = new ChatClientAgent(chatClient, options);
```

### Issue: GitHub token not working

**Solution**: Verify token has correct permissions and is not expired.

---

## Next Steps

- [Module 05: Thread Management](../05-Thread-Management/)
- [Module 06: Tool Registration](../06-Tool-Registration/)
- [Module 07: Invocation Patterns](../07-Invocation-Patterns/)

---

**Ready to manage conversations?** Continue to [Module 05: Thread Management](../05-Thread-Management/)! 💬
