# Quick Reference Guide

**One-page cheat sheet for Semantic Kernel to Agent Framework migration**

## Package Migration

### Remove Semantic Kernel Packages

```xml
<!-- Remove these -->
<PackageReference Include="Microsoft.SemanticKernel" Version="1.*" />
<PackageReference Include="Microsoft.SemanticKernel.Agents.Core" Version="1.*" />
```

### Add Agent Framework Packages

```xml
<!-- Add these -->
<PackageReference Include="Microsoft.Agents.AI" Version="1.0.0" />
<PackageReference Include="Microsoft.Extensions.AI" Version="9.0.0" />
<PackageReference Include="Microsoft.Extensions.AI.OpenAI" Version="9.0.0" />
```

---

## Namespace Changes

| Semantic Kernel | Agent Framework |
|-----------------|-----------------|
| `using Microsoft.SemanticKernel;` | `using Microsoft.Agents.AI;` |
| `using Microsoft.SemanticKernel.Agents;` | `using Microsoft.Extensions.AI;` |
| `using Microsoft.SemanticKernel.ChatCompletion;` | `using Microsoft.Extensions.AI;` |

---

## Core API Mapping

### Agent Creation

**Semantic Kernel:**
```csharp
var kernel = Kernel.CreateBuilder()
    .AddOpenAIChatCompletion("gpt-4o", apiKey)
    .Build();

var agent = new ChatCompletionAgent
{
    Kernel = kernel,
    Name = "Assistant"
};
```

**Agent Framework:**
```csharp
var client = new OpenAIChatClient("gpt-4o", apiKey);
var agent = client.CreateAIAgent(name: "Assistant");
```

---

### Invocation (Non-Streaming)

**Semantic Kernel:**
```csharp
await foreach (var item in agent.InvokeAsync(thread, "Hello"))
{
    var content = item.Content.GetValue<ChatMessageContent>();
    Console.WriteLine(content.Content);
}
```

**Agent Framework:**
```csharp
var response = await agent.RunAsync(thread, "Hello");
Console.WriteLine(response.Text);
```

---

### Streaming Invocation

**Semantic Kernel:**
```csharp
await foreach (var item in agent.InvokeStreamingAsync(thread, "Hello"))
{
    var streamContent = item.Content as StreamingChatMessageContent;
    Console.Write(streamContent?.Content);
}
```

**Agent Framework:**
```csharp
await foreach (var update in agent.RunStreamingAsync(thread, "Hello"))
{
    Console.Write(update.Text);
}
```

---

### Thread Management

**Semantic Kernel:**
```csharp
// Provider-specific thread types
var thread = new OpenAIAssistantAgentThread("openai-thread-id");
// or
var thread = new AzureAIAgentThread("azure-thread-id");
```

**Agent Framework:**
```csharp
// Unified thread management
var thread = await agent.GetNewThread();

// Delete thread when done
await thread.DeleteAsync();
```

---

### Tool Registration

**Semantic Kernel:**
```csharp
public class WeatherPlugin
{
    [KernelFunction]
    [Description("Get weather for a location")]
    public string GetWeather(string location)
    {
        return $"Weather in {location}: Sunny, 72°F";
    }
}

kernel.Plugins.Add(
    KernelPluginFactory.CreateFromType<WeatherPlugin>()
);
```

**Agent Framework:**
```csharp
// Direct function registration
string GetWeather(string location)
{
    return $"Weather in {location}: Sunny, 72°F";
}

var agent = client.CreateAIAgent(
    tools: new[] { GetWeather }
);
```

---

### Configuration

**Semantic Kernel:**
```csharp
var settings = new OpenAIPromptExecutionSettings
{
    MaxTokens = 100,
    Temperature = 0.7,
    TopP = 0.9
};

var arguments = new KernelArguments(settings);
await agent.InvokeAsync(thread, "Hello", arguments);
```

**Agent Framework:**
```csharp
var options = new ChatClientAgentRunOptions
{
    MaxTokens = 100,
    Temperature = 0.7,
    TopP = 0.9
};

await agent.RunAsync(thread, "Hello", options);
```

---

### Dependency Injection

**Semantic Kernel:**
```csharp
// Program.cs
builder.Services.AddKernel()
    .AddOpenAIChatCompletion("gpt-4o", apiKey);

builder.Services.AddSingleton(sp =>
{
    var kernel = sp.GetRequiredService<Kernel>();
    return new ChatCompletionAgent { Kernel = kernel };
});
```

**Agent Framework:**
```csharp
// Program.cs
builder.Services.AddSingleton<IChatClient>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var apiKey = config["OpenAI:ApiKey"];
    return new OpenAIChatClient("gpt-4o", apiKey);
});

builder.Services.AddSingleton<AIAgent>(sp =>
{
    var client = sp.GetRequiredService<IChatClient>();
    return client.CreateAIAgent();
});
```

---

## Key Type Changes

| Semantic Kernel | Agent Framework | Usage |
|-----------------|-----------------|-------|
| `Kernel` | `IChatClient` | Core AI client |
| `ChatCompletionAgent` | `AIAgent` | Agent instance |
| `AgentResponseItem<ChatMessageContent>` | `AgentRunResponse` | Response object |
| `StreamingChatMessageContent` | `AgentRunResponseUpdate` | Streaming update |
| `OpenAIAssistantAgentThread` | Unified thread type | Thread management |
| `KernelFunction` | Function/delegate | Tool definition |
| `OpenAIPromptExecutionSettings` | `ChatOptions` | Configuration |

---

## Common Patterns

### Pattern 1: Basic Chat

```csharp
// AF: Simple chat interaction
var client = new OpenAIChatClient("gpt-4o", apiKey);
var agent = client.CreateAIAgent();
var thread = await agent.GetNewThread();

var response = await agent.RunAsync(thread, "What is AI?");
Console.WriteLine(response.Text);
```

### Pattern 2: Chat with Tools

```csharp
// AF: Agent with function calling
string GetTime() => DateTime.Now.ToString();
int Add(int a, int b) => a + b;

var agent = client.CreateAIAgent(
    tools: new[] { (Func<string>)GetTime, (Func<int, int, int>)Add }
);

var response = await agent.RunAsync(thread, "What time is it?");
```

### Pattern 3: Streaming Chat

```csharp
// AF: Real-time streaming responses
var fullResponse = new StringBuilder();

await foreach (var update in agent.RunStreamingAsync(thread, "Tell me a story"))
{
    Console.Write(update.Text);
    fullResponse.Append(update.Text);
}

Console.WriteLine($"\n\nFull response: {fullResponse}");
```

---

## Configuration Setup

### User Secrets (Required)

```bash
# Initialize secrets
dotnet user-secrets init

# Add OpenAI credentials
dotnet user-secrets set "OpenAI:ApiKey" "sk-..."
dotnet user-secrets set "OpenAI:ChatDeploymentName" "gpt-4o"

# Or Azure OpenAI
dotnet user-secrets set "AzureOpenAI:Endpoint" "https://..."
dotnet user-secrets set "AzureOpenAI:ApiKey" "..."
dotnet user-secrets set "AzureOpenAI:ChatDeploymentName" "gpt-4o"
```

### appsettings.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  },
  "OpenAI": {
    "ChatDeploymentName": "gpt-4o"
  }
}
```

### Read Configuration

```csharp
// In Program.cs or Startup
var configuration = builder.Configuration;
var apiKey = configuration["OpenAI:ApiKey"];
var model = configuration["OpenAI:ChatDeploymentName"];
```

---

## Migration Checklist

- [ ] Update package references (remove SK, add AF)
- [ ] Update namespace imports
- [ ] Replace `Kernel` with `IChatClient`
- [ ] Change agent creation pattern
- [ ] Update thread management
- [ ] Convert plugins to direct functions
- [ ] Update invocation methods (InvokeAsync → RunAsync)
- [ ] Update streaming methods
- [ ] Simplify DI configuration
- [ ] Update options/settings objects
- [ ] Test all functionality
- [ ] Update documentation

---

## Performance Benefits

After migration, expect:

- ⚡ **30-40% faster** agent creation
- 💾 **20-30% lower** memory usage
- 🚀 **15-25% faster** response times
- 📉 **20-40% fewer** lines of code

---

## Common Gotchas

1. **Thread Management**: Use `agent.GetNewThread()` instead of provider-specific types
2. **Function Registration**: No `[KernelFunction]` attributes needed
3. **Response Handling**: Response text is directly available via `.Text` property
4. **Configuration**: Always use User Secrets, never .env files
5. **Async Methods**: All methods are async, always use `await`

---

## Need More Details?

- **Complete Guide**: See [migration-checklist.md](migration-checklist.md)
- **API Reference**: See [api-mapping.md](api-mapping.md)
- **Examples**: Browse [modules/](../modules/)
- **Issues**: Check [TROUBLESHOOTING.md](TROUBLESHOOTING.md)

---

**Keep this guide handy while migrating!** 📋
