# Module 02: Basic Migration - Hello World

**Duration**: 15 minutes  
**Level**: Beginner  
**Prerequisites**: .NET 9 SDK installed, API keys configured

---

## Learning Objectives

By the end of this module, you will:

- Perform your first simple agent migration
- Understand core concept changes between SK and AF
- Recognize code simplification opportunities
- Successfully run both versions side-by-side

---

## Overview

This module demonstrates the most basic migration: a simple "Hello World" chatbot. We'll:

1. Review the Semantic Kernel implementation
2. Show the equivalent Agent Framework code
3. Highlight key differences
4. Run both versions

---

## The Scenario

**Requirement**: Create a simple agent that responds to user messages using GPT-4o.

**Features**:
- Single agent
- Basic chat interaction
- Console output

---

## Semantic Kernel Version

### Code Structure

```
before-sk/
├── Program.cs
├── BasicAgent_SK.csproj
├── appsettings.json
└── README.md
```

### Key Code (Program.cs)

```csharp
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.Extensions.Configuration;

// Load configuration
var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddUserSecrets<Program>()
    .Build();

var apiKey = configuration["OpenAI:ApiKey"] 
    ?? throw new InvalidOperationException("API key not configured");
var model = configuration["OpenAI:ChatDeploymentName"] ?? "gpt-4o";

// Build Kernel
var kernel = Kernel.CreateBuilder()
    .AddOpenAIChatCompletion(model, apiKey)
    .Build();

// Create Agent
var agent = new ChatCompletionAgent
{
    Kernel = kernel,
    Name = "Assistant",
    Instructions = "You are a helpful AI assistant."
};

// Get chat completion service for thread
var chatService = kernel.GetRequiredService<IChatCompletionService>();

// Chat loop
Console.WriteLine("Semantic Kernel Basic Agent");
Console.WriteLine("Type 'exit' to quit\n");

while (true)
{
    Console.Write("You: ");
    var input = Console.ReadLine();
    
    if (string.IsNullOrEmpty(input) || input.ToLower() == "exit")
        break;
    
    // Invoke agent
    var response = await agent.InvokeAsync(input);
    
    Console.WriteLine($"Agent: {response}\n");
}
```

**Lines of Code**: ~45  
**Key Objects**: Kernel, ChatCompletionAgent, IChatCompletionService  
**Complexity**: Medium (multiple abstractions)

---

## Agent Framework Version

### Code Structure

```
after-af/
├── Program.cs
├── BasicAgent_AF.csproj
├── appsettings.json
└── README.md
```

### Key Code (Program.cs)

```csharp
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;

// Load configuration
var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddUserSecrets<Program>()
    .Build();

var apiKey = configuration["OpenAI:ApiKey"] 
    ?? throw new InvalidOperationException("API key not configured");
var model = configuration["OpenAI:ChatDeploymentName"] ?? "gpt-4o";

// Create client and agent
var client = new OpenAIChatClient(model, apiKey);
var agent = client.CreateAIAgent(
    name: "Assistant",
    instructions: "You are a helpful AI assistant."
);

// Create thread
var thread = await agent.GetNewThread();

// Chat loop
Console.WriteLine("Agent Framework Basic Agent");
Console.WriteLine("Type 'exit' to quit\n");

while (true)
{
    Console.Write("You: ");
    var input = Console.ReadLine();
    
    if (string.IsNullOrEmpty(input) || input.ToLower() == "exit")
        break;
    
    // Run agent
    var response = await agent.RunAsync(thread, input);
    
    Console.WriteLine($"Agent: {response.Text}\n");
}

// Cleanup
await thread.DeleteAsync();
```

**Lines of Code**: ~35 (22% reduction)  
**Key Objects**: IChatClient, AIAgent, Thread  
**Complexity**: Low (direct abstractions)

---

## Side-by-Side Comparison

### Agent Creation

| Semantic Kernel | Agent Framework |
|-----------------|-----------------|
| Create Kernel first | Create Client directly |
| Configure Kernel builder | Configure Client constructor |
| Create Agent separately | Create Agent from Client |
| 3 steps | 2 steps |

### Invocation

| Semantic Kernel | Agent Framework |
|-----------------|-----------------|
| `agent.InvokeAsync(message)` | `agent.RunAsync(thread, message)` |
| Returns `IAsyncEnumerable` | Returns `AgentRunResponse` |
| Iterate to get response | Direct `.Text` property |

### Thread Management

| Semantic Kernel | Agent Framework |
|-----------------|-----------------|
| Implicit or provider-specific | Explicit unified thread |
| `new OpenAIAssistantAgentThread()` | `agent.GetNewThread()` |
| Manual cleanup | `await thread.DeleteAsync()` |

---

## Key Migration Changes

### 1. Package References

**Remove**:
```xml
<PackageReference Include="Microsoft.SemanticKernel" Version="1.61.0" />
```

**Add**:
```xml
<PackageReference Include="Microsoft.Extensions.AI" Version="9.0.0" />
<PackageReference Include="Microsoft.Extensions.AI.OpenAI" Version="9.0.0" />
<PackageReference Include="Microsoft.Agents.AI" Version="1.0.0" />
```

### 2. Namespace Imports

**Before**:
```csharp
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
```

**After**:
```csharp
using Microsoft.Extensions.AI;
using Microsoft.Agents.AI;
```

### 3. Core Objects

**Before**:
```csharp
var kernel = Kernel.CreateBuilder()...;
var agent = new ChatCompletionAgent { Kernel = kernel };
```

**After**:
```csharp
var client = new OpenAIChatClient(...);
var agent = client.CreateAIAgent();
```

### 4. Invocation

**Before**:
```csharp
var response = await agent.InvokeAsync(input);
```

**After**:
```csharp
var response = await agent.RunAsync(thread, input);
Console.WriteLine(response.Text);
```

---

## Running the Samples

### Prerequisites

1. .NET 9 SDK installed
2. User Secrets configured

### Configure User Secrets

For both projects:

```bash
# Navigate to project directory
cd modules/02-Basic-Migration/code-samples/before-sk
dotnet user-secrets init
dotnet user-secrets set "OpenAI:ApiKey" "sk-your-key-here"
dotnet user-secrets set "OpenAI:ChatDeploymentName" "gpt-4o"

# Repeat for after-af project
cd ../after-af
dotnet user-secrets init
dotnet user-secrets set "OpenAI:ApiKey" "sk-your-key-here"
dotnet user-secrets set "OpenAI:ChatDeploymentName" "gpt-4o"
```

### Run Semantic Kernel Version

```bash
cd code-samples/before-sk
dotnet run
```

Expected output:
```
Semantic Kernel Basic Agent
Type 'exit' to quit

You: Hello
Agent: Hello! How can I assist you today?

You: exit
```

### Run Agent Framework Version

```bash
cd code-samples/after-af
dotnet run
```

Expected output:
```
Agent Framework Basic Agent
Type 'exit' to quit

You: Hello
Agent: Hello! How can I assist you today?

You: exit
```

---

## Benefits Demonstrated

### Code Simplification

- **22% fewer lines of code**
- **Clearer intent** - Direct client → agent pattern
- **Less boilerplate** - No Kernel builder ceremony

### Better API

- **Unified thread management** - `agent.GetNewThread()`
- **Direct response access** - `.Text` property
- **Explicit cleanup** - `thread.DeleteAsync()`

### Improved Performance

While this simple example doesn't showcase it, AF provides:
- Faster agent initialization
- Lower memory footprint
- Better throughput for multiple requests

---

## Migration Steps Recap

To migrate this example:

1. ✅ Update package references
2. ✅ Update namespace imports
3. ✅ Replace `Kernel` with `IChatClient`
4. ✅ Replace `ChatCompletionAgent` with `AIAgent`
5. ✅ Add thread management
6. ✅ Update invocation method
7. ✅ Test functionality

**Time Invested**: ~10-15 minutes  
**Lines Changed**: ~15 lines  
**Complexity**: Low

---

## Common Issues and Solutions

### Issue 1: "OpenAI namespace not found"

**Solution**: Add NuGet package
```bash
dotnet add package Microsoft.Extensions.AI.OpenAI
```

### Issue 2: "API key is null"

**Solution**: Configure User Secrets
```bash
dotnet user-secrets set "OpenAI:ApiKey" "your-key"
```

### Issue 3: "Thread is null"

**Solution**: Create thread before calling RunAsync
```csharp
var thread = await agent.GetNewThread();
```

---

## What's Next?

Now that you've seen a basic migration:

1. [Module 03: Namespace and Package Updates](../03-Namespace-And-Packages/) - Detailed package migration
2. [Module 04: Agent Creation](../04-Agent-Creation/) - Advanced agent patterns
3. [Module 06: Tool Registration](../06-Tool-Registration/) - Migrating plugins to functions

---

## Try It Yourself

### Exercise 1: Modify Instructions

Change the agent instructions in both versions and compare behavior.

### Exercise 2: Add Error Handling

Add try-catch blocks to handle API errors gracefully.

### Exercise 3: Customize Output

Format the responses with color or timestamps.

---

## Additional Resources

- [Demo Script](demo-script.md) - Step-by-step presentation guide
- [Quick Reference](../../docs/QUICK-REFERENCE.md) - API cheat sheet
- [Code Samples](code-samples/) - Full working examples

---

**Ready to dive deeper?** Continue to [Module 03: Namespace and Packages](../03-Namespace-And-Packages/)! 🎯
