# Lab 02 Solution: Customer Management Bot (Agent Framework)

## Overview

This is the solution for Lab 02, demonstrating Agent Framework migration with:
- Async functions using closures for database access
- No plugin class or attributes required
- Direct ChatClient integration
- GitHub Models for free AI

## Key Migration Changes

### 1. Plugin Class → Async Functions with Closures

**Before (Semantic Kernel):**
```csharp
public sealed class CustomerPlugin
{
    private readonly DatabaseService _database;
    
    public CustomerPlugin(DatabaseService database)
    {
        _database = database;
    }
    
    [KernelFunction]
    public async Task<string> GetCustomer(int customerId)
    {
        return await _database.GetCustomerAsync(customerId);
    }
}
```

**After (Agent Framework):**
```csharp
var database = new DatabaseService();

// Functions use closure to access database
async Task<string> GetCustomer(int customerId) =>
    await database.GetCustomerAsync(customerId);
```

### 2. Kernel → ChatClient

**Before:**
```csharp
var kernel = Kernel.CreateBuilder()
    .AddOpenAIChatCompletion(modelId, apiKey, endpoint)
    .Build();
kernel.ImportPluginFromObject(plugin, "CustomerManagement");
```

**After:**
```csharp
IChatClient chatClient = new ChatClient(
        deploymentName,
        new ApiKeyCredential(githubToken),
        new OpenAIClientOptions { Endpoint = new Uri(endpoint) })
    .AsIChatClient();
```

### 3. Agent Creation

**Before:**
```csharp
var agent = new ChatCompletionAgent
{
    Kernel = kernel,
    Name = "CustomerBot",
    Instructions = "..."
};
```

**After:**
```csharp
var agent = new ChatClientAgent(
    chatClient,
    new ChatClientAgentOptions
    {
        Name = "CustomerBot",
        Instructions = "..."
    });
```

### 4. Invocation Pattern

**Before:**
```csharp
await foreach (var update in agent.InvokeAsync(thread))
{
    // Process updates
}
```

**After:**
```csharp
AgentRunResponse response = await agent.RunAsync(input);
Console.WriteLine(response.Text);
```

## Prerequisites

- .NET 9 SDK
- GitHub personal access token

## Configuration

### 1. Initialize User Secrets

```bash
cd labs/lab-02-tool-migration/solution/after-af
dotnet user-secrets init
```

### 2. Set GitHub Token

```bash
dotnet user-secrets set "GITHUB_TOKEN" "your-github-token-here"
```

## How to Run

```bash
dotnet build
dotnet run
```

## Sample Commands

- "Get customer 123"
- "Create a new customer named David Lee with email david@example.com in Boston"
- "Search for customers in Seattle"
- "Update email for customer 456 to newemail@example.com"

## Benefits of Agent Framework

1. **Simpler Code**: ~30% fewer lines than Semantic Kernel version
2. **No Attributes**: Functions don't need `[KernelFunction]` decorations
3. **Closure Support**: Easy dependency access via closures
4. **Direct Pattern**: Simpler invocation with `RunAsync`
5. **Free Testing**: GitHub Models for development

## Note on Tool Registration

The current preview version of Agent Framework has evolving support for tool/function registration. This example demonstrates the patterns, and the actual registration mechanism may vary based on the specific preview version. Check the official documentation for the latest API patterns.
