# Basic Agent - Semantic Kernel Version

This is the "before" version demonstrating basic agent functionality using **Semantic Kernel**.

## What This Sample Demonstrates

- Creating a Kernel with OpenAI chat completion
- Managing chat history manually
- Using Semantic Kernel's ChatHistory and IChatCompletionService
- Basic console chat loop

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

### For Azure OpenAI

If using Azure OpenAI instead:

```bash
dotnet user-secrets set "AzureOpenAI:Endpoint" "https://your-resource.openai.azure.com/"
dotnet user-secrets set "AzureOpenAI:ApiKey" "your-api-key"
dotnet user-secrets set "AzureOpenAI:ChatDeploymentName" "gpt-4o"
```

Note: You'll need to modify Program.cs to use Azure OpenAI configuration.

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
=== Semantic Kernel Basic Agent ===
Type 'exit' to quit

Using model: gpt-4o
Creating Semantic Kernel agent...

You: Hello
Agent: Hello! How can I assist you today?

You: What is 2+2?
Agent: 2 + 2 equals 4.

You: exit
Goodbye!
```

## Key Semantic Kernel Concepts

### 1. Kernel Creation

```csharp
var kernel = Kernel.CreateBuilder()
    .AddOpenAIChatCompletion(modelId: model, apiKey: apiKey)
    .Build();
```

The Kernel is the central orchestrator in Semantic Kernel.

### 2. Chat Completion Service

```csharp
var chatService = kernel.GetRequiredService<IChatCompletionService>();
```

Get the chat completion service from the kernel.

### 3. Chat History Management

```csharp
var history = new ChatHistory();
history.AddSystemMessage("You are a helpful AI assistant.");
history.AddUserMessage(input);
history.AddAssistantMessage(response.Content);
```

Manually manage conversation history.

### 4. Getting Responses

```csharp
var response = await chatService.GetChatMessageContentAsync(history, kernel: kernel);
```

## Lines of Code

**Total**: ~85 lines (including comments and error handling)

## Complexity Assessment

- **Setup Complexity**: Medium (Kernel builder, service retrieval)
- **History Management**: Manual (must track history yourself)
- **Error Handling**: Required for robust operation

## Migration Notes

When migrating to Agent Framework, this code will:

1. **Simplify**: No Kernel needed, direct chat client
2. **Streamline**: Thread management handles history automatically
3. **Reduce**: Approximately 22% fewer lines of code

See the Agent Framework version in `../after-af/` for comparison.

## Common Issues

### "API key not found"

**Solution**: Configure User Secrets as shown above.

### "Model not found"

**Solution**: Verify the model name matches your OpenAI or Azure OpenAI deployment.

### Package restore errors

**Solution**: Ensure you have .NET 9 SDK installed:
```bash
dotnet --version  # Should be 9.0.x
```

## Next Steps

Compare this with the Agent Framework version:
- [Agent Framework Version](../after-af/)
- [Module Documentation](../../README.md)

## Additional Resources

- [Semantic Kernel Documentation](https://learn.microsoft.com/en-us/semantic-kernel/)
- [Quick Reference Guide](../../../../docs/QUICK-REFERENCE.md)
