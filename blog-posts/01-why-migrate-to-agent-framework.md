---
title: "Why Migrate from Semantic Kernel to Microsoft Agent Framework"
date: 2024-01-15
author: "Bruno Capuano"
description: "Discover the benefits of migrating from Semantic Kernel to Microsoft Agent Framework with GitHub Models for free AI development in .NET 9"
keywords: ["Semantic Kernel", "Microsoft Agent Framework", "AI Agents", ".NET 9", "GitHub Models", "C#", "Migration Guide"]
tags: ["AI", "Azure", ".NET", "C#", "Agent Framework", "GitHub Models"]
---

# Why Migrate from Semantic Kernel to Microsoft Agent Framework

## Introduction

Microsoft has introduced the Agent Framework as the next evolution of AI agent development in .NET. If you're currently using Semantic Kernel, you might be wondering whether it's worth migrating to this new framework. In this post, we'll explore the key benefits and why now might be the perfect time to make the switch.

## What is Microsoft Agent Framework?

Microsoft Agent Framework is a modern, streamlined approach to building AI agents in .NET, built on top of `Microsoft.Extensions.AI`. It provides a unified interface for creating AI agents across different providers (OpenAI, Azure OpenAI, Azure AI Foundry, and GitHub Models).

Unlike Semantic Kernel, which uses a Kernel orchestrator pattern, Agent Framework takes a more direct approach - you create agents directly from chat clients without the overhead of an orchestrator layer.

## Key Benefits of Migrating

### 1. Simplified API and Less Boilerplate

**Before (Semantic Kernel):**
```csharp
var kernel = Kernel.CreateBuilder()
    .AddOpenAIChatCompletion("gpt-4o-mini", apiKey)
    .Build();

var agent = new ChatCompletionAgent
{
    Kernel = kernel,
    Name = "Assistant",
    Instructions = "You are a helpful assistant."
};
```

**After (Agent Framework with GitHub Models):**
```csharp
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
        Instructions = "You are a helpful assistant."
    });
```

**Result**: 20-40% less code, clearer intent, and no Kernel orchestrator overhead.

### 2. Free AI Models with GitHub Models

One of the biggest advantages of migrating now is the integration with **GitHub Models**. This free service provides access to popular AI models for development and testing without API costs:

- **Free tier** for development
- **Multiple models** available (GPT-4o-mini, GPT-4, and more)
- **Same API patterns** as production OpenAI
- **Easy switching** to production endpoints when ready

```csharp
// Development with GitHub Models (free!)
var devClient = new ChatClient(
    "gpt-4o-mini",
    new ApiKeyCredential(githubToken),
    new OpenAIClientOptions { 
        Endpoint = new Uri("https://models.github.ai/inference") 
    });

// Production with OpenAI (same code pattern!)
var prodClient = new ChatClient("gpt-4o-mini", new ApiKeyCredential(openAIKey));
```

### 3. Unified Interface Across Providers

Agent Framework provides consistent patterns regardless of which AI provider you're using. The same code works with:

- OpenAI
- Azure OpenAI
- Azure AI Foundry
- GitHub Models
- Other compatible providers

No more provider-specific thread types or patterns. One API to learn, use everywhere.

### 4. Direct Function Registration

**Semantic Kernel** requires plugin classes with attributes:
```csharp
public class WeatherPlugin
{
    [KernelFunction]
    [Description("Get weather")]
    public string GetWeather(string location) => $"Weather in {location}";
}

kernel.Plugins.Add(KernelPluginFactory.CreateFromType<WeatherPlugin>());
```

**Agent Framework** uses direct function references:
```csharp
string GetWeather(string location) => $"Weather in {location}";

var agent = new ChatClientAgent(
    chatClient,
    new ChatClientAgentOptions
    {
        Tools = { GetWeather }
    });
```

Much simpler! No attributes, no plugin classes, just pass your functions directly.

### 5. Modern .NET Integration

Agent Framework is built on .NET 9 and follows modern .NET patterns:

- Standard dependency injection
- Natural ASP.NET Core integration
- `Microsoft.Extensions.AI` abstractions
- Async/await best practices
- Interface-based design for testing

### 6. Future-Proof Architecture

Agent Framework is Microsoft's strategic direction for AI agent development in .NET. New features, improvements, and optimizations will focus on this framework going forward.

## When Should You Migrate?

### Good Time to Migrate

✅ **Starting a new project** - Begin with Agent Framework from day one
✅ **Upgrading to .NET 9** - Perfect opportunity to modernize
✅ **Major refactoring planned** - Include migration in your work
✅ **Want to reduce code complexity** - Simplified patterns help
✅ **Using free AI models for development** - GitHub Models integration

### Consider Waiting

⏳ **Legacy .NET Framework** - Can't use .NET 9
⏳ **Critical production system** - Wait for more stability
⏳ **Heavy investment in SK plugins** - Plan gradual migration
⏳ **Limited team bandwidth** - Focus on current work

## Migration Effort

The migration effort varies by project size:

- **Simple console app** (< 100 lines): 15-30 minutes
- **Basic chatbot** (100-500 lines): 1-2 hours  
- **Complex application** (500+ lines): 4-8 hours per major component

Most migrations follow these steps:

1. Update NuGet packages
2. Update namespace imports
3. Replace Kernel with ChatClient
4. Convert plugins to functions
5. Update agent creation
6. Test and verify

## Real-World Benefits

Teams that have migrated report:

- **Faster development** - Less boilerplate means faster feature development
- **Easier onboarding** - New team members grasp the simpler patterns quickly
- **Better testability** - Interface-based design makes unit testing straightforward
- **Lower costs** - GitHub Models for development eliminates API costs during development

## Getting Started

Ready to migrate? Here's your path:

1. **Review the [migration modules](../modules/)** - 15 comprehensive guides
2. **Try Module 02** - Basic migration with working code samples
3. **Experiment with GitHub Models** - Get your free token at github.com/settings/tokens
4. **Migrate incrementally** - One component at a time
5. **Join the community** - Share your experience and learn from others

## Conclusion

Migrating from Semantic Kernel to Agent Framework offers clear benefits: simpler code, unified patterns, modern .NET integration, and free AI models for development. While Semantic Kernel remains supported, Agent Framework represents the future of AI agent development in .NET.

The best time to migrate depends on your situation, but with comprehensive migration guides, working examples, and free GitHub Models for development, the barrier to entry has never been lower.

## Hands-On Learning

Want to try it yourself? Check out our hands-on labs with complete working code:

- **[Lab 01: Basic Migration](../labs/lab-01-basic-migration/)** - Migrate your first chatbot ([starter](../labs/lab-01-basic-migration/starter/before-sk/) | [solution](../labs/lab-01-basic-migration/solution/after-af/))
- **[Lab 02: Tool Migration](../labs/lab-02-tool-migration/)** - Handle complex async functions ([starter](../labs/lab-02-tool-migration/starter/before-sk/) | [solution](../labs/lab-02-tool-migration/solution/after-af/))
- **[Lab 03: ASP.NET Integration](../labs/lab-03-aspnet-integration/)** - Build production web APIs ([starter](../labs/lab-03-aspnet-integration/starter/before-sk/) | [solution](../labs/lab-03-aspnet-integration/solution/after-af/))
- **[Lab 04: Testing Strategies](../labs/lab-04-testing-strategies/)** - Write comprehensive tests ([starter](../labs/lab-04-testing-strategies/starter/before-sk/) | [solution](../labs/lab-04-testing-strategies/solution/after-af/))

Each lab includes:
- ✅ Complete working code samples (before/after)
- ✅ Step-by-step instructions
- ✅ GitHub Models integration (free testing!)
- ✅ README with configuration guide

## Resources

- [Complete Migration Guide](../README.md)
- [Module 02: Basic Migration](../modules/02-Basic-Migration/)
- [Quick Reference Guide](../docs/QUICK-REFERENCE.md)
- [GitHub Models Documentation](https://github.com/features/models)
- [Agent Framework Documentation](https://learn.microsoft.com/dotnet/ai/)
- [All Lab Code Samples](../labs/)

---

**Next Post**: [Step-by-Step Migration Guide](./02-step-by-step-migration-guide.md) - Learn the detailed migration process with practical examples.

---

*Have questions or feedback? Open an issue in the repository or connect with the community!*
