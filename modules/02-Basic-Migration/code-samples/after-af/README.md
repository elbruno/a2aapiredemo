# Basic Agent - Agent Framework Version

This is the "after" version demonstrating basic agent functionality using **Microsoft.Extensions.AI** and the modern Agent Framework patterns.

## What This Sample Demonstrates

- Creating an IChatClient directly from OpenAI client
- Using ChatMessage list for conversation management
- Simplified chat completion with CompleteAsync
- Cleaner, more direct API surface

## Prerequisites

- .NET 9 SDK
- OpenAI API key or Azure OpenAI access

## Configuration

This sample uses .NET User Secrets for secure API key storage.

### Initialize User Secrets

```bash
dotnet user-secrets init
dotnet user-secrets set "OpenAI:ApiKey" "sk-your-api-key-here"
dotnet user-secrets set "OpenAI:ChatDeploymentName" "gpt-4o"
```

## Running the Sample

```bash
# Restore packages
dotnet restore

# Build
dotnet build

# Run
dotnet run
```

## Expected Output

```
=== Agent Framework Basic Agent ===
Type 'exit' to quit

Using model: gpt-4o
Creating Agent Framework chat client...

You: Hello
Agent: Hello! How can I assist you today?

You: What is 2+2?
Agent: 2 + 2 equals 4.

You: exit
Goodbye!
```

## Key Agent Framework Concepts

### 1. Direct Client Creation

```csharp
IChatClient chatClient = new OpenAIClient(apiKey)
    .AsChatClient(model);
```

No Kernel needed - create the chat client directly.

### 2. Simple Message Management

```csharp
var messages = new List<ChatMessage>
{
    new(ChatRole.System, "You are a helpful AI assistant.")
};

messages.Add(new ChatMessage(ChatRole.User, input));
```

Use standard List<ChatMessage> for conversation history.

### 3. Streamlined Completion

```csharp
var response = await chatClient.CompleteAsync(messages);
var text = response.Message.Text;
```

Single method call, direct text access.

## Lines of Code

**Total**: ~70 lines (including comments and error handling)

## Comparison with Semantic Kernel

| Aspect | Semantic Kernel | Agent Framework |
|--------|----------------|-----------------|
| **Setup** | Kernel builder + service | Direct client creation |
| **Lines** | ~85 lines | ~70 lines (18% fewer) |
| **Complexity** | Medium | Low |
| **History** | ChatHistory object | List<ChatMessage> |
| **API Calls** | GetChatMessageContentAsync | CompleteAsync |

## Benefits Demonstrated

### Simplification

- ✅ No Kernel orchestrator needed
- ✅ Direct client creation
- ✅ Standard .NET collections for messages
- ✅ Clear, intuitive method names

### Performance

While this simple example doesn't stress performance:
- Faster agent initialization (no Kernel overhead)
- Lower memory footprint (fewer abstractions)
- More direct path to AI provider

### Maintainability

- Fewer objects to manage
- Clearer code flow
- Easier to understand and debug
- Better IDE support

## Migration Highlights

From Semantic Kernel to Agent Framework:

1. **Remove Kernel**: No more `Kernel.CreateBuilder()`
2. **Direct Client**: Use `OpenAIClient().AsChatClient()`
3. **Simplified Messages**: `List<ChatMessage>` instead of `ChatHistory`
4. **Better API**: `CompleteAsync()` instead of `GetChatMessageContentAsync()`

## Code Structure Improvements

### Before (SK):
```
Kernel → ChatCompletionService → ChatHistory → GetChatMessageContentAsync
```

### After (AF):
```
IChatClient → List<ChatMessage> → CompleteAsync
```

**Result**: More direct, less ceremony.

## Common Issues

### "Package not found"

**Solution**: Ensure you have .NET 9 SDK and the preview packages:
```bash
dotnet add package Microsoft.Extensions.AI --prerelease
dotnet add package Microsoft.Extensions.AI.OpenAI --prerelease
```

### "API key not found"

**Solution**: Configure User Secrets as shown above.

### "Model not found"

**Solution**: Verify the model name matches your OpenAI deployment.

## Next Steps

- Compare with [Semantic Kernel Version](../before-sk/)
- Learn about [Tool Registration](../../../06-Tool-Registration/)
- Explore [Streaming Responses](../../../08-Streaming-Responses/)

## Additional Resources

- [Microsoft.Extensions.AI Documentation](https://learn.microsoft.com/en-us/dotnet/ai/ai-extensions)
- [Quick Reference Guide](../../../../docs/QUICK-REFERENCE.md)
- [Migration Checklist](../../../../docs/migration-checklist.md)
