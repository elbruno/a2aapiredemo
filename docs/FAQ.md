# Frequently Asked Questions (FAQ)

Common questions about migrating from Semantic Kernel to Microsoft Agent Framework.

---

## General Questions

### What is Microsoft Agent Framework?

Microsoft Agent Framework is the next evolution of AI agent development in .NET, built on top of `Microsoft.Extensions.AI`. It provides a simplified, unified interface for creating AI agents across different providers (OpenAI, Azure OpenAI, Azure AI Foundry, etc.).

### Why should I migrate from Semantic Kernel?

Key reasons to migrate:

1. **Simplified API** - Less boilerplate code and clearer patterns
2. **Unified Interface** - Consistent patterns across all AI providers
3. **Modern .NET** - Built on .NET 9 with latest C# features
4. **Future Direction** - Microsoft's strategic path for AI agents
5. **GitHub Models Ready** - Easy integration with free AI models for development

### Is this a breaking change?

Yes. Agent Framework is a different API surface that requires code changes. However, the concepts remain similar, and the migration path is straightforward.

### Can I use both frameworks in the same application?

Technically yes, but it's not recommended. Choose one framework for your application to maintain consistency and avoid confusion.

---

## Migration Questions

### How long does migration take?

Migration time varies by project size:

- **Simple agent** (< 100 lines): 15-30 minutes
- **Basic chatbot** (100-500 lines): 1-2 hours
- **Complex application** (500+ lines): 4-8 hours per major component

### Should I migrate all at once or incrementally?

For small projects, migrate all at once. For large applications, consider an incremental approach:

1. Create new features with Agent Framework
2. Migrate one component at a time
3. Run both frameworks side-by-side during transition
4. Complete migration and remove Semantic Kernel

See [incremental-migration-guide.md](incremental-migration-guide.md) for details.

### What if I have custom plugins?

Convert plugins to simple functions:

**Before (SK):**
```csharp
public class MyPlugin
{
    [KernelFunction]
    public string MyFunction(string input) => $"Result: {input}";
}
```

**After (AF):**
```csharp
string MyFunction(string input) => $"Result: {input}";
var agent = client.CreateAIAgent(tools: new[] { MyFunction });
```

### Will my existing prompts work?

Yes! Prompts are provider-specific, not framework-specific. Your existing prompts will work with Agent Framework.

---

## Technical Questions

### What .NET version do I need?

.NET 9.0 or later is required for Agent Framework. If you're on .NET 6/8, you'll need to upgrade your project.

### Do I need to change my AI provider?

No. Agent Framework works with the same providers as Semantic Kernel:
- OpenAI
- Azure OpenAI  
- Azure AI Foundry
- Other compatible providers

### How do I handle environment variables?

**Don't use environment variables or .env files.** Use .NET User Secrets for development:

```bash
dotnet user-secrets init
dotnet user-secrets set "OpenAI:ApiKey" "your-key"
```

For production, use Azure Key Vault or your platform's secret management.

### What about streaming responses?

Streaming is simplified in Agent Framework:

```csharp
// AF: Simple streaming
await foreach (var update in agent.RunStreamingAsync(thread, "Hello"))
{
    Console.Write(update.Text);
}
```

### How do I access conversation history?

Thread management is built-in:

```csharp
var thread = await agent.GetNewThread();
await agent.RunAsync(thread, "First message");
await agent.RunAsync(thread, "Follow-up question");
// History is maintained in the thread
```

---

## Configuration Questions

### How do I configure API keys securely?

Use .NET User Secrets for development:

```bash
dotnet user-secrets set "OpenAI:ApiKey" "sk-..."
dotnet user-secrets set "OpenAI:ChatDeploymentName" "gpt-4o"
```

For production:
- Azure: Use Azure Key Vault
- AWS: Use AWS Secrets Manager
- GCP: Use Secret Manager
- Other: Use your platform's secret management

### Can I use appsettings.json for configuration?

Yes, for non-sensitive values:

```json
{
  "OpenAI": {
    "ChatDeploymentName": "gpt-4o",
    "MaxTokens": 1000
  }
}
```

**Never** put API keys in appsettings.json.

### How do I use different models?

Pass the model name when creating the client:

```csharp
// OpenAI
var client = new OpenAIChatClient("gpt-4o", apiKey);

// Azure OpenAI
var client = new OpenAIChatClient(
    deployment: "gpt-4o",
    endpoint: new Uri("https://your-resource.openai.azure.com/"),
    credential: new AzureKeyCredential(apiKey)
);
```

---

## API Cost Questions

### Will my API costs change when migrating?

No. API costs depend on token usage, which is the same regardless of framework. You're still calling the same AI models with the same prompts.

### Can I use free AI models for development?

Yes! GitHub Models provides free access to several AI models for development and testing. See the code samples in Module 02 for examples using GitHub Models (https://models.github.ai/inference).

---

## Testing Questions

### How do I test Agent Framework code?

Use dependency injection and interfaces:

```csharp
// Production
services.AddSingleton<IChatClient>(sp => 
    new OpenAIChatClient("gpt-4o", apiKey)
);

// Testing
services.AddSingleton<IChatClient>(sp => 
    new MockChatClient() // Your test implementation
);
```

See [Module 14: Testing Strategies](../modules/14-Testing-Strategies/) for examples.

### Can I mock IChatClient?

Yes! `IChatClient` is an interface, making it easy to mock:

```csharp
public class MockChatClient : IChatClient
{
    public async Task<ChatCompletion> CompleteAsync(
        IList<ChatMessage> messages,
        ChatOptions options = null,
        CancellationToken cancellationToken = default)
    {
        return new ChatCompletion
        {
            Message = new ChatMessage("Mock response")
        };
    }
    // ... other methods
}
```

---

## ASP.NET Core Questions

### How do I use Agent Framework in Web API?

Register in `Program.cs`:

```csharp
builder.Services.AddSingleton<IChatClient>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var apiKey = config["OpenAI:ApiKey"];
    return new OpenAIChatClient("gpt-4o", apiKey);
});

builder.Services.AddScoped<AIAgent>(sp =>
{
    var client = sp.GetRequiredService<IChatClient>();
    return client.CreateAIAgent();
});
```

Then inject in controllers:

```csharp
[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly AIAgent _agent;

    public ChatController(AIAgent agent)
    {
        _agent = agent;
    }

    [HttpPost("message")]
    public async Task<IActionResult> SendMessage([FromBody] string message)
    {
        var thread = await _agent.GetNewThread();
        var response = await _agent.RunAsync(thread, message);
        return Ok(response.Text);
    }
}
```

See [Module 15: ASP.NET Core Integration](../modules/15-ASPNetCore-Integration/) for complete examples.

### What about Blazor?

Same pattern - inject `IChatClient` or `AIAgent` into Blazor components:

```csharp
@inject AIAgent Agent

<button @onclick="SendMessage">Send</button>

@code {
    private async Task SendMessage()
    {
        var thread = await Agent.GetNewThread();
        var response = await Agent.RunAsync(thread, "Hello");
        // Handle response
    }
}
```

---

## Troubleshooting Questions

### My code doesn't compile after migration

Common issues:

1. **Missing packages**: Add `Microsoft.Agents.AI` and `Microsoft.Extensions.AI`
2. **Wrong namespaces**: Update `using` statements
3. **Old types**: Replace `Kernel` with `IChatClient`, etc.
4. **.NET version**: Upgrade to .NET 9

See [TROUBLESHOOTING.md](TROUBLESHOOTING.md) for detailed solutions.

### I get "null reference" errors

Usually caused by missing configuration:

```bash
# Add required User Secrets
dotnet user-secrets set "OpenAI:ApiKey" "your-key"
dotnet user-secrets set "OpenAI:ChatDeploymentName" "gpt-4o"
```

### The AI responses are different

AI responses can vary between calls due to non-deterministic behavior. To reduce variance:

```csharp
var options = new ChatClientAgentRunOptions
{
    Temperature = 0.0  // More deterministic
};

await agent.RunAsync(thread, "message", options);
```

---

## Production Questions

### Is Agent Framework production-ready?

Yes. Agent Framework (version 1.0+) is production-ready and supported by Microsoft.

### What about backward compatibility?

Agent Framework is a new API surface. While Semantic Kernel continues to be supported, new features and improvements will focus on Agent Framework.

### Where can I get support?

- **Documentation**: [Official Microsoft Docs](https://learn.microsoft.com/en-us/agent-framework/)
- **Issues**: [GitHub Issues](https://github.com/microsoft/agent-framework/issues)
- **Community**: Microsoft Learn Q&A
- **This Repo**: [GitHub Discussions](https://github.com/elbruno/a2aapiredemo/discussions)

### Will Semantic Kernel be deprecated?

Microsoft hasn't announced deprecation plans, but Agent Framework is the strategic direction for new development.

---

## Additional Resources

### Where can I learn more?

- [Official Documentation](https://learn.microsoft.com/en-us/agent-framework/)
- [Migration Guide](https://learn.microsoft.com/en-us/agent-framework/migration-guide/from-semantic-kernel/)
- [Module 01: Introduction](../modules/01-Introduction/)
- [Module 02: Basic Migration](../modules/02-Basic-Migration/)
- [Blog Post Series](../blog-posts/)

### Are there video tutorials?

Check these resources:
- [Video Recording Scripts](video-recording-scripts.md) for creating your own
- Microsoft Learn videos (coming soon)
- Community content on YouTube

### Can I contribute to this guide?

Yes! See [CONTRIBUTING.md](../CONTRIBUTING.md) for guidelines on contributing code, documentation, or examples.

---

## Still Have Questions?

If your question isn't answered here:

1. Check [TROUBLESHOOTING.md](TROUBLESHOOTING.md)
2. Search [GitHub Discussions](https://github.com/elbruno/a2aapiredemo/discussions)
3. Review [Module Documentation](../modules/)
4. Open a new discussion or issue

---

**Can't find what you're looking for? Ask in GitHub Discussions!** 💬
