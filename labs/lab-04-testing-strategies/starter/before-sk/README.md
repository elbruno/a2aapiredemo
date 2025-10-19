# Lab 04 Starter: Testing Strategies (Semantic Kernel)

## Overview

This is the starting point for Lab 04. It demonstrates testing patterns for Semantic Kernel:
- Function/Plugin unit tests (no AI required)
- Integration tests with real AI (GitHub Models)
- Context maintenance testing
- xUnit test framework

## Prerequisites

- .NET 9 SDK
- GitHub personal access token (for integration tests)

## Configuration

### 1. Initialize User Secrets

```bash
cd labs/lab-04-testing-strategies/starter/before-sk
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
```

## Test Structure

### Function Tests (Unit Tests)
Tests individual plugin functions without AI:
```csharp
[Fact]
public void GetOrderStatus_ReturnsCorrectFormat()
{
    var plugin = new SupportPlugin();
    var result = plugin.GetOrderStatus("12345");
    Assert.Contains("12345", result);
}
```

**Benefits:**
- Fast (no API calls)
- No cost
- Deterministic
- Test business logic

### Integration Tests (Real AI)
Tests complete agent behavior with GitHub Models:
```csharp
[Fact]
public async Task Agent_UsesOrderStatusFunction()
{
    var thread = new ChatHistory();
    thread.AddUserMessage("What's the status of order 12345?");
    
    var response = await GetAgentResponse(thread);
    
    Assert.Contains("Shipped", response);
}
```

**Benefits:**
- End-to-end validation
- Real AI behavior
- Function calling verification
- Free with GitHub Models!

## Key Testing Patterns

### 1. Test Function Logic Independently
```csharp
[Theory]
[InlineData("12345")]
[InlineData("67890")]
public void GetOrderStatus_HandlesVariousOrderIds(string orderId)
{
    var plugin = new SupportPlugin();
    var result = plugin.GetOrderStatus(orderId);
    Assert.NotNull(result);
}
```

### 2. Test AI Integration
```csharp
[Fact]
public async Task Agent_AnswersSimpleQuestion()
{
    var thread = new ChatHistory();
    thread.AddUserMessage("What is 2+2?");
    var response = await GetAgentResponse(thread);
    Assert.Contains("4", response);
}
```

### 3. Test Context Maintenance
```csharp
[Fact]
public async Task Agent_MaintainsContext()
{
    thread.AddUserMessage("Check order 12345");
    await agent.InvokeAsync(thread);
    
    thread.AddUserMessage("Can you refund it?");
    var response = await GetAgentResponse(thread);
    
    Assert.Contains("12345", response);
}
```

## Your Task

Migrate these tests to Agent Framework in the `solution/after-af/` directory. Focus on:
1. Using IChatClient and ChatClientAgent
2. Simplifying the invocation pattern
3. Maintaining the same test coverage
4. Keeping both unit and integration tests

Compare your solution with `solution/after-af/` when complete.

## Test Categories

| Category | Speed | Cost | When to Use |
|----------|-------|------|-------------|
| Function Tests | Fast | Free | Always |
| Integration Tests | Moderate | Free* | Before deployment |
| Manual Tests | Slow | Paid** | Edge cases |

*Free with GitHub Models for development
**Paid in production with OpenAI/Azure OpenAI
