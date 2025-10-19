# Module 12: Real-World Migration Examples

**Duration**: 25 minutes  
**Level**: Advanced  

---

## Overview

This module presents real-world migration scenarios from Semantic Kernel to Agent Framework, showing practical examples with GitHub Models.

---

## Case Study 1: Customer Support Bot

### Before (Semantic Kernel)

```csharp
public class SupportPlugin
{
    [KernelFunction]
    public string GetOrderStatus(string orderId) => $"Order {orderId}: Shipped";
    
    [KernelFunction]
    public string GetRefundStatus(string orderId) => $"Refund {orderId}: Approved";
}

var kernel = Kernel.CreateBuilder()
    .AddOpenAIChatCompletion("gpt-4", apiKey)
    .Build();
kernel.Plugins.Add(KernelPluginFactory.CreateFromType<SupportPlugin>());
var agent = new ChatCompletionAgent { Kernel = kernel };
```

### After (Agent Framework with GitHub Models)

```csharp
string GetOrderStatus(string orderId) => $"Order {orderId}: Shipped";
string GetRefundStatus(string orderId) => $"Refund {orderId}: Approved";

var githubToken = configuration["GITHUB_TOKEN"];
var chatClient = new ChatClient(
    "gpt-4o-mini",
    new ApiKeyCredential(githubToken!),
    new OpenAIClientOptions { Endpoint = new Uri("https://models.github.ai/inference") });

var agent = new ChatClientAgent(
    chatClient.AsIChatClient(),
    new ChatClientAgentOptions
    {
        Name = "SupportAgent",
        Instructions = "You are a customer support agent. Be helpful and professional.",
        Tools = { GetOrderStatus, GetRefundStatus }
    });
```

**Benefits**: 40% less code, free AI models for development, simpler structure.

---

## Case Study 2: Document Analyzer

### Migration Summary

**Changes Made**:
1. Removed Kernel orchestrator
2. Converted document parsing plugin to direct functions
3. Switched to GitHub Models endpoint
4. Simplified error handling

**Result**: Faster development, easier testing, no API costs in development.

---

## Case Study 3: Multi-Agent System

### Before (SK): Complex orchestration with Kernel

### After (AF with GitHub Models):

```csharp
var client = new ChatClient(
    "gpt-4o-mini",
    new ApiKeyCredential(githubToken!),
    new OpenAIClientOptions { Endpoint = new Uri("https://models.github.ai/inference") })
    .AsIChatClient();

var researchAgent = new ChatClientAgent(client, new() 
{ 
    Name = "Researcher",
    Instructions = "You research topics thoroughly."
});

var writerAgent = new ChatClientAgent(client, new() 
{ 
    Name = "Writer",
    Instructions = "You write engaging content."
});

var reviewerAgent = new ChatClientAgent(client, new() 
{ 
    Name = "Reviewer",
    Instructions = "You review and improve content."
});

// Orchestration
var research = await researchAgent.RunAsync("Research AI trends");
var draft = await writerAgent.RunAsync($"Write an article based on: {research.Text}");
var final = await reviewerAgent.RunAsync($"Review and improve: {draft.Text}");
```

---

## Case Study 4: Web API Migration

### Before: Complex SK + ASP.NET Core setup

### After: Simplified DI with GitHub Models

```csharp
// Program.cs
builder.Services.AddSingleton<IChatClient>(sp =>
{
    var token = sp.GetRequiredService<IConfiguration>()["GITHUB_TOKEN"];
    return new ChatClient(
        "gpt-4o-mini",
        new ApiKeyCredential(token!),
        new OpenAIClientOptions { Endpoint = new Uri("https://models.github.ai/inference") })
        .AsIChatClient();
});

builder.Services.AddScoped<ChatClientAgent>(sp =>
{
    var client = sp.GetRequiredService<IChatClient>();
    return new ChatClientAgent(client, new() { Name = "WebAssistant" });
});
```

**Benefits**: Standard ASP.NET Core patterns, easier testing, free development models.

---

## Common Migration Patterns

### Pattern 1: Plugin → Function
```csharp
// Before: Plugin class
// After: Direct function
string MyFunction(string input) => ProcessInput(input);
```

### Pattern 2: Kernel → Client
```csharp
// Before: Kernel.CreateBuilder()
// After: new ChatClient()
```

### Pattern 3: Thread Management
```csharp
// Before: Manual thread objects
// After: Agent maintains context
```

---

## Migration Checklist

- [ ] Identify all SK plugins
- [ ] Extract function logic
- [ ] Remove plugin attributes
- [ ] Update to GitHub Models endpoint
- [ ] Replace Kernel with ChatClient
- [ ] Update agent creation
- [ ] Test all functionality
- [ ] Update configuration (User Secrets)

---

## Lessons Learned

1. **Start with packages** - Update NuGet first
2. **Test incrementally** - Migrate one feature at a time
3. **Use GitHub Models** - Free development and testing
4. **Simplify gradually** - Don't over-engineer
5. **Document changes** - Help team understand new patterns

---

## Next: [Module 13: Testing Strategies](../13-Testing-Strategies/)
