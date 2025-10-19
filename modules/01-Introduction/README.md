# Module 01: Introduction to Agent Framework Migration

**Duration**: 10 minutes  
**Level**: Beginner  
**Prerequisites**: Basic understanding of AI agents and .NET development

---

## Learning Objectives

By the end of this module, you will:

- Understand what Microsoft Agent Framework is
- Recognize key differences from Semantic Kernel
- Identify benefits of migrating
- Know when migration makes sense for your project

---

## What is Microsoft Agent Framework?

Microsoft Agent Framework is the next evolution of AI agent development in .NET, built on top of `Microsoft.Extensions.AI`. It provides a **simplified, unified interface** for creating AI agents across different providers (OpenAI, Azure OpenAI, Azure AI Foundry, etc.).

### Key Characteristics

- **Simplified API** - Less boilerplate code
- **Provider-Agnostic** - Consistent patterns across all AI providers
- **Modern .NET** - Built on .NET 9 with latest C# features
- **Performance-Focused** - Optimized for speed and memory efficiency
- **Extensible** - Easy to integrate with existing .NET applications

---

## Why Migrate from Semantic Kernel?

### 1. Simplified Code

**Semantic Kernel** requires more setup:
```csharp
var kernel = Kernel.CreateBuilder()
    .AddOpenAIChatCompletion("gpt-4o", apiKey)
    .Build();

var agent = new ChatCompletionAgent
{
    Kernel = kernel,
    Name = "Assistant",
    Instructions = "You are a helpful assistant."
};
```

**Agent Framework** is more direct:
```csharp
var client = new OpenAIChatClient("gpt-4o", apiKey);
var agent = client.CreateAIAgent(
    name: "Assistant",
    instructions: "You are a helpful assistant."
);
```

**Result**: ~40% less boilerplate code

---

### 2. Better Performance

Benchmarks show significant improvements:

| Metric | Semantic Kernel | Agent Framework | Improvement |
|--------|----------------|-----------------|-------------|
| Agent Creation | 12ms | 7ms | **42% faster** |
| Memory Allocation | 2.1 KB | 1.5 KB | **29% less** |
| First Response | 850ms | 720ms | **15% faster** |

See [Module 13: Performance Benchmarking](../13-Performance-Benchmarking/) for detailed results.

---

### 3. Unified Interface

Agent Framework provides consistent patterns across providers:

```csharp
// Works the same for OpenAI, Azure OpenAI, Azure AI Foundry, etc.
var agent = client.CreateAIAgent();
var thread = await agent.GetNewThread();
var response = await agent.RunAsync(thread, "Hello");
```

No more provider-specific thread types or patterns.

---

### 4. Modern .NET Patterns

- Built on .NET 9
- Uses `Microsoft.Extensions.AI` abstractions
- Natural integration with ASP.NET Core
- Standard dependency injection patterns
- Async/await best practices

---

### 5. Future-Proof

Agent Framework is Microsoft's strategic direction for AI agent development in .NET. New features and improvements will focus on this framework.

---

## Key Differences at a Glance

### Architecture

**Semantic Kernel:**
```
Application
    └── Kernel (orchestrator)
        ├── Plugins
        ├── Chat Completion Service
        └── Agent
```

**Agent Framework:**
```
Application
    └── ChatClient (provider interface)
        └── AIAgent (agent instance)
            └── Direct functions (no plugin wrapper)
```

---

### Code Comparison

| Aspect | Semantic Kernel | Agent Framework |
|--------|----------------|-----------------|
| **Core Object** | `Kernel` | `IChatClient` |
| **Agent Type** | `ChatCompletionAgent` | `AIAgent` |
| **Tools** | Plugin classes with `[KernelFunction]` | Direct functions |
| **Invocation** | `InvokeAsync` | `RunAsync` |
| **Streaming** | `InvokeStreamingAsync` | `RunStreamingAsync` |
| **Thread** | Provider-specific types | Unified thread interface |
| **Configuration** | `KernelArguments` | `ChatClientAgentRunOptions` |

---

## When Should You Migrate?

### ✅ Good Time to Migrate

- Starting a new project (start with AF)
- Upgrading to .NET 9
- Major refactoring planned
- Performance is critical
- Want to reduce code complexity
- Building production applications

### ⏳ Consider Waiting

- Legacy .NET Framework projects (can't use .NET 9)
- Critical production system (wait for stability)
- Heavy investment in SK plugins (gradual migration)
- Team bandwidth constraints

### ⚠️ Decision Factors

Use the [Migration Decision Tree](../../docs/migration-decision-tree.md) to evaluate your specific situation.

---

## Migration Complexity Assessment

### Simple Projects (Easy Migration)

- Basic chatbot with few functions
- Single agent applications
- Console applications
- **Estimated Time**: 1-2 hours

### Medium Projects (Moderate Migration)

- Multi-agent systems
- ASP.NET Core Web APIs
- Applications with multiple plugins
- **Estimated Time**: 4-8 hours

### Complex Projects (Planned Migration)

- Enterprise applications
- Large plugin ecosystems
- Complex orchestration patterns
- **Estimated Time**: 1-2 weeks (incremental approach)

---

## What Stays the Same?

Good news! These don't change:

- ✅ **AI Models** - Same OpenAI, Azure OpenAI models
- ✅ **Prompts** - Your existing prompts work as-is
- ✅ **API Costs** - Token usage is the same
- ✅ **Responses** - AI behavior is identical
- ✅ **Concepts** - Agents, tools, threads remain

---

## What Changes?

You'll need to update:

- ❌ Package references
- ❌ Namespace imports
- ❌ Agent creation code
- ❌ Tool/plugin definitions
- ❌ Invocation patterns
- ❌ Configuration setup

**But**: All changes follow clear patterns. See [Quick Reference](../../docs/QUICK-REFERENCE.md).

---

## Benefits Summary

### Developer Experience

- **Clearer Code** - More intuitive API surface
- **Better IntelliSense** - Improved IDE support
- **Less Boilerplate** - 20-40% fewer lines
- **Easier Debugging** - Simpler call stacks

### Performance

- **Faster Startup** - Quicker agent initialization
- **Lower Memory** - Reduced allocations
- **Better Throughput** - More requests per second

### Maintainability

- **Simpler Patterns** - Easier to understand
- **Standard .NET** - Follows framework conventions
- **Better Testing** - Interface-based design
- **Future Updates** - Microsoft's focus area

---

## Next Steps

Ready to start migrating? Proceed to:

1. [Module 02: Basic Migration](../02-Basic-Migration/) - Your first migration
2. [Setup Guide](../../docs/SETUP-GUIDE.md) - Configure your environment
3. [Quick Reference](../../docs/QUICK-REFERENCE.md) - API cheat sheet

---

## Additional Resources

- [Official Documentation](https://learn.microsoft.com/en-us/agent-framework/)
- [Migration Guide](https://learn.microsoft.com/en-us/agent-framework/migration-guide/from-semantic-kernel/)
- [FAQ](../../docs/FAQ.md)
- [Blog Post: Why Migrate](../../blog-posts/01-why-migrate-to-agent-framework.md)

---

## Discussion Points for Presenters

When delivering this module, emphasize:

1. **Show, Don't Just Tell** - Use side-by-side code examples
2. **Relate to Audience** - Ask about their current SK usage
3. **Address Concerns** - Migration effort, backward compatibility
4. **Build Excitement** - Performance gains, cleaner code
5. **Provide Path** - Clear next steps

---

**Ready to see code?** Let's start with [Module 02: Basic Migration](../02-Basic-Migration/)! 🚀
