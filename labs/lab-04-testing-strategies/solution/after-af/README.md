# Lab 04 Solution: Testing Strategies (Agent Framework)

## Overview

This is the solution for Lab 04, demonstrating testing patterns for Agent Framework:
- Function unit tests (static methods, no AI)
- Integration tests with real AI (GitHub Models)
- Simplified test setup and assertions
- xUnit test framework

## Key Migration Changes

### 1. Plugin Class → Static Functions

**Before (Semantic Kernel):**
```csharp
public sealed class SupportPlugin
{
    [KernelFunction]
    public string GetOrderStatus(string orderId) => ...;
}

// Test
var plugin = new SupportPlugin();
var result = plugin.GetOrderStatus("12345");
```

**After (Agent Framework):**
```csharp
public static class SupportFunctions
{
    public static string GetOrderStatus(string orderId) => ...;
}

// Test
var result = SupportFunctions.GetOrderStatus("12345");
```

### 2. Agent Setup in Tests

**Before:**
```csharp
var kernelBuilder = Kernel.CreateBuilder();
kernelBuilder.AddOpenAIChatCompletion(modelId, apiKey, endpoint);
var kernel = kernelBuilder.Build();
kernel.ImportPluginFromType<SupportPlugin>();

var agent = new ChatCompletionAgent
{
    Kernel = kernel,
    Name = "SupportBot"
};
```

**After:**
```csharp
var chatClient = new ChatClient(
        "gpt-4o-mini",
        new ApiKeyCredential(githubToken),
        new OpenAIClientOptions { Endpoint = new Uri(endpoint) })
    .AsIChatClient();

var agent = new ChatClientAgent(
    chatClient,
    new ChatClientAgentOptions
    {
        Name = "SupportBot",
        Instructions = "..."
    });
```

### 3. Test Invocation

**Before:**
```csharp
var thread = new ChatHistory();
thread.AddUserMessage("What is 2+2?");

var response = "";
await foreach (var update in agent.InvokeAsync(thread))
{
    // Complex extraction logic
}

Assert.Contains("4", response);
```

**After:**
```csharp
var response = await agent.RunAsync("What is 2+2?");
Assert.Contains("4", response.Text);
```

## Prerequisites

- .NET 9 SDK
- GitHub personal access token (for integration tests)

## Configuration

### 1. Initialize User Secrets

```bash
cd labs/lab-04-testing-strategies/solution/after-af
dotnet user-secrets init
```

### 2. Set GitHub Token (for Integration Tests)

```bash
dotnet user-secrets set "GITHUB_TOKEN" "your-github-token-here"
```

## How to Run Tests

```bash
# Run all tests
dotnet test

# Run only function tests (fast, no AI)
dotnet test --filter FullyQualifiedName~FunctionTests

# Run only integration tests (requires GitHub token)
dotnet test --filter FullyQualifiedName~IntegrationTests

# Run tests with detailed output
dotnet test --logger "console;verbosity=detailed"
```

## Test Structure

### Function Tests (Unit Tests)
Tests individual functions without AI:
```csharp
[Fact]
public void GetOrderStatus_ReturnsCorrectFormat()
{
    var result = SupportFunctions.GetOrderStatus("12345");
    Assert.Contains("12345", result);
}
```

**Benefits:**
- Fast (no API calls)
- No cost
- Deterministic
- Simple static method calls

### Integration Tests (Real AI)
Tests complete agent behavior with GitHub Models:
```csharp
[Fact]
public async Task Agent_AnswersSimpleQuestion()
{
    var response = await _agent.RunAsync("What is 2+2?");
    Assert.Contains("4", response.Text);
}
```

**Benefits:**
- Simpler than SK version
- Direct access to response.Text
- No thread management
- Free with GitHub Models!

## Key Testing Patterns

### 1. Test Function Logic Independently
```csharp
[Theory]
[InlineData("12345")]
[InlineData("67890")]
public void GetOrderStatus_HandlesVariousOrderIds(string orderId)
{
    var result = SupportFunctions.GetOrderStatus(orderId);
    Assert.NotNull(result);
}
```

### 2. Test AI Integration (Simplified)
```csharp
[Fact]
public async Task Agent_AnswersSimpleQuestion()
{
    var response = await _agent.RunAsync("What is 2+2?");
    Assert.Contains("4", response.Text);
}
```

### 3. Test Multiple Scenarios with Theory
```csharp
[Theory]
[InlineData("Hello")]
[InlineData("What is 5+5?")]
public async Task Agent_HandlesVariousQuestions(string question)
{
    var response = await _agent.RunAsync(question);
    Assert.NotNull(response.Text);
}
```

## Benefits of Agent Framework for Testing

1. **Simpler Setup**: Fewer lines to create test agents
2. **Clearer Assertions**: Direct access to `response.Text`
3. **Static Functions**: Easier to test without instantiation
4. **Less Boilerplate**: No thread management needed
5. **Free Testing**: GitHub Models for integration tests

## Test Categories

| Category | Speed | Cost | Lines of Code | When to Use |
|----------|-------|------|---------------|-------------|
| Function Tests | Fast | Free | ~5 lines | Always |
| Integration Tests | Moderate | Free* | ~8 lines (vs 20+ in SK) | Before deployment |
| Manual Tests | Slow | Paid** | N/A | Edge cases |

*Free with GitHub Models for development  
**Paid in production with OpenAI/Azure OpenAI

## CI/CD Integration

Example GitHub Actions workflow:

```yaml
name: Test

on: [push, pull_request]

jobs:
  test:
    runs-on: ubuntu-latest
    
    steps:
    - uses: actions/checkout@v3
    - uses: actions/setup-dotnet@v3
      with:
        dotnet-version: '9.0.x'
    
    - run: dotnet restore
    - run: dotnet build
    - run: dotnet test
      env:
        GITHUB_TOKEN: ${{ secrets.GITHUB_TOKEN }}
```

## What Makes This Better Than SK Tests?

| Aspect | Semantic Kernel | Agent Framework |
|--------|----------------|-----------------|
| Agent Setup | 10+ lines | 5 lines |
| Invocation | 15+ lines | 1 line |
| Response Access | Complex extraction | `response.Text` |
| Test Clarity | Moderate | High |
| Maintenance | Higher | Lower |
