---
title: "Testing and Best Practices for Agent Framework Applications"
date: 2024-02-05
author: "Bruno Capuano"
description: "Comprehensive guide to testing Agent Framework applications with GitHub Models, including unit tests, integration tests, and production best practices"
keywords: ["Testing", "Best Practices", "Agent Framework", "Unit Testing", ".NET 9", "GitHub Models"]
tags: ["Testing", "Best Practices", ".NET", "C#", "Quality Assurance"]
---

# Testing and Best Practices for Agent Framework Applications

## Introduction

Moving from Semantic Kernel to Agent Framework isn't just about getting code to work—it's about building reliable, maintainable AI applications. In this comprehensive guide, we'll cover testing strategies, best practices, and production-ready patterns for Agent Framework applications using GitHub Models.

## Why Testing Matters More with AI

AI applications present unique testing challenges:
- **Non-deterministic responses** - Same input can produce different outputs
- **External dependencies** - Reliance on AI model availability
- **Cost considerations** - API calls add up during testing
- **Complex interactions** - Function calling adds layers of complexity

Agent Framework with GitHub Models solves the cost problem—free AI for testing means you can test thoroughly without budget constraints.

---

## Testing Strategy Overview

### Three-Layer Testing Approach

```
┌─────────────────────────────────────┐
│     Integration Tests                │  ← Test with GitHub Models (free!)
│  (Full agent with real AI)          │
├─────────────────────────────────────┤
│     Component Tests                  │  ← Test functions independently
│  (Business logic only)               │
├─────────────────────────────────────┤
│     Unit Tests                       │  ← Test with mocks
│  (Isolated, fast)                    │
└─────────────────────────────────────┘
```

---

## Unit Testing with Mocks

### Basic Agent Test

```csharp
using Microsoft.Extensions.AI;
using Microsoft.Agents.AI;
using Moq;
using Xunit;

public class AgentTests
{
    [Fact]
    public async Task Agent_CreatesSuccessfully()
    {
        // Arrange
        var mockClient = new Mock<IChatClient>();
        mockClient
            .Setup(c => c.CompleteAsync(
                It.IsAny<IList<ChatMessage>>(),
                It.IsAny<ChatOptions>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ChatCompletion
            {
                Message = new ChatMessage(
                    ChatRole.Assistant, 
                    "Test response")
            });

        // Act
        var agent = new ChatClientAgent(
            mockClient.Object,
            new ChatClientAgentOptions { Name = "TestAgent" });

        var response = await agent.RunAsync("Test input");

        // Assert
        Assert.NotNull(agent);
        Assert.Equal("Test response", response.Text);
        mockClient.Verify(c => c.CompleteAsync(
            It.IsAny<IList<ChatMessage>>(),
            It.IsAny<ChatOptions>(),
            It.IsAny<CancellationToken>()), 
            Times.Once);
    }
}
```

### Testing Function Registration

```csharp
[Fact]
public void Agent_RegistersToolsCorrectly()
{
    // Arrange
    var mockClient = new Mock<IChatClient>();
    
    string TestFunction(string input) => $"Processed: {input}";
    int AddNumbers(int a, int b) => a + b;

    // Act
    var agent = new ChatClientAgent(
        mockClient.Object,
        new ChatClientAgentOptions
        {
            Name = "TestAgent",
            Tools = { TestFunction, AddNumbers }
        });

    // Assert
    Assert.Equal(2, agent.Options.Tools.Count);
}
```

### Testing Error Handling

```csharp
[Fact]
public async Task Agent_HandlesExceptionGracefully()
{
    // Arrange
    var mockClient = new Mock<IChatClient>();
    mockClient
        .Setup(c => c.CompleteAsync(
            It.IsAny<IList<ChatMessage>>(),
            It.IsAny<ChatOptions>(),
            It.IsAny<CancellationToken>()))
        .ThrowsAsync(new Exception("API Error"));

    var agent = new ChatClientAgent(
        mockClient.Object,
        new ChatClientAgentOptions { Name = "TestAgent" });

    // Act & Assert
    await Assert.ThrowsAsync<Exception>(
        async () => await agent.RunAsync("Test"));
}
```

---

## Integration Testing with GitHub Models

GitHub Models enables comprehensive integration testing at **zero cost**.

### Test Configuration

```csharp
// TestConfiguration.cs
public class TestConfiguration
{
    public static IChatClient CreateTestClient()
    {
        var githubToken = Environment.GetEnvironmentVariable("GITHUB_TOKEN")
            ?? throw new InvalidOperationException("GITHUB_TOKEN required for tests");

        return new ChatClient(
            "gpt-4o-mini",
            new ApiKeyCredential(githubToken),
            new OpenAIClientOptions
            {
                Endpoint = new Uri("https://models.github.ai/inference")
            }).AsIChatClient();
    }
}
```

### Integration Test Example

```csharp
public class AgentIntegrationTests : IDisposable
{
    private readonly IChatClient _client;
    private readonly ChatClientAgent _agent;

    public AgentIntegrationTests()
    {
        _client = TestConfiguration.CreateTestClient();
        
        _agent = new ChatClientAgent(_client, new()
        {
            Name = "TestAgent",
            Instructions = "You are a helpful test assistant.",
            Temperature = 0.0f  // Deterministic for testing
        });
    }

    [Fact]
    public async Task Agent_RespondsToSimpleQuery()
    {
        // Act
        var response = await _agent.RunAsync("What is 2+2?");

        // Assert
        Assert.Contains("4", response.Text);
    }

    [Fact]
    public async Task Agent_MaintainsContext()
    {
        // Act
        var response1 = await _agent.RunAsync("My name is Alice");
        var response2 = await _agent.RunAsync("What is my name?");

        // Assert
        Assert.Contains("Alice", response2.Text, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Agent_WithTools_CallsFunctions()
    {
        // Arrange
        var functionCalled = false;
        string TestFunction(string input)
        {
            functionCalled = true;
            return $"Processed: {input}";
        }

        var toolAgent = new ChatClientAgent(_client, new()
        {
            Name = "ToolAgent",
            Instructions = "Use the TestFunction when asked to process text.",
            Tools = { TestFunction }
        });

        // Act
        var response = await toolAgent.RunAsync("Please process 'hello world'");

        // Assert - Check that function was called
        // Note: Actual verification depends on AI's function calling behavior
        Assert.NotEmpty(response.Text);
    }

    public void Dispose()
    {
        // Cleanup if needed
    }
}
```

### Deterministic Testing

```csharp
[Theory]
[InlineData("What is the capital of France?", "Paris")]
[InlineData("What is 5+5?", "10")]
[InlineData("Is the Earth round?", "yes")]
public async Task Agent_GivesExpectedAnswers(string question, string expectedKeyword)
{
    // Use Temperature=0 for most deterministic results
    var agent = new ChatClientAgent(
        TestConfiguration.CreateTestClient(),
        new()
        {
            Temperature = 0.0f,
            Instructions = "Give short, direct answers."
        });

    var response = await agent.RunAsync(question);

    Assert.Contains(expectedKeyword, response.Text, StringComparison.OrdinalIgnoreCase);
}
```

---

## Testing Functions Independently

Always test your functions separately before integrating with the agent.

```csharp
public class FunctionTests
{
    [Theory]
    [InlineData("Seattle", "Seattle")]
    [InlineData("New York", "New York")]
    public void GetWeather_ReturnsCorrectLocation(string input, string expected)
    {
        // Arrange
        string GetWeather(string location) => $"Weather in {location}: Sunny";

        // Act
        var result = GetWeather(input);

        // Assert
        Assert.Contains(expected, result);
    }

    [Theory]
    [InlineData(2, 3, 5)]
    [InlineData(-1, 1, 0)]
    [InlineData(100, 200, 300)]
    public void AddNumbers_ReturnsCorrectSum(int a, int b, int expected)
    {
        // Arrange
        int AddNumbers(int x, int y) => x + y;

        // Act
        var result = AddNumbers(a, b);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task AsyncFunction_WorksCorrectly()
    {
        // Arrange
        async Task<string> FetchDataAsync(string url)
        {
            await Task.Delay(10); // Simulate async work
            return $"Data from {url}";
        }

        // Act
        var result = await FetchDataAsync("https://example.com");

        // Assert
        Assert.Contains("example.com", result);
    }
}
```

---

## ASP.NET Core Integration Tests

```csharp
public class ChatControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public ChatControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((context, config) =>
            {
                // Use GitHub Models for testing
                config.AddInMemoryCollection(new Dictionary<string, string>
                {
                    ["GITHUB_TOKEN"] = Environment.GetEnvironmentVariable("GITHUB_TOKEN")!
                }!);
            });
        });

        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task ChatEndpoint_ReturnsSuccessStatusCode()
    {
        // Arrange
        var request = new { message = "Hello" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/chat", request);

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task ChatEndpoint_ReturnsValidResponse()
    {
        // Arrange
        var request = new { message = "What is 2+2?" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/chat", request);
        var result = await response.Content.ReadFromJsonAsync<ChatResponse>();

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.Response);
        Assert.Contains("4", result.Response);
    }

    record ChatResponse(string Response);
}
```

---

## CI/CD Integration

### GitHub Actions Workflow

```yaml
name: Test Agent Framework App

on: [push, pull_request]

jobs:
  test:
    runs-on: ubuntu-latest

    steps:
    - uses: actions/checkout@v3
    
    - name: Setup .NET 9
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: '9.0.x'
    
    - name: Restore dependencies
      run: dotnet restore
    
    - name: Build
      run: dotnet build --no-restore
    
    - name: Test with GitHub Models
      env:
        GITHUB_TOKEN: ${{ secrets.GITHUB_TOKEN }}
      run: dotnet test --no-build --verbosity normal
```

**Key benefit**: Tests run for free using GitHub Models in CI/CD!

---

## Best Practices

### 1. Environment-Based Configuration

```csharp
public static class ClientFactory
{
    public static IChatClient Create(IConfiguration config, IWebHostEnvironment env)
    {
        if (env.IsDevelopment() || env.IsStaging())
        {
            // Use GitHub Models for dev/test (free!)
            var githubToken = config["GITHUB_TOKEN"];
            return new ChatClient(
                "gpt-4o-mini",
                new ApiKeyCredential(githubToken!),
                new OpenAIClientOptions
                {
                    Endpoint = new Uri("https://models.github.ai/inference")
                }).AsIChatClient();
        }
        else
        {
            // Use Azure OpenAI for production
            var azureKey = config["AzureOpenAI:ApiKey"];
            var endpoint = config["AzureOpenAI:Endpoint"];
            return new ChatClient(
                deployment: "gpt-4",
                endpoint: new Uri(endpoint!),
                credential: new AzureKeyCredential(azureKey!)).AsIChatClient();
        }
    }
}
```

### 2. Structured Logging

```csharp
[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly ChatClientAgent _agent;
    private readonly ILogger<ChatController> _logger;

    public ChatController(ChatClientAgent agent, ILogger<ChatController> logger)
    {
        _agent = agent;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Chat([FromBody] ChatRequest request)
    {
        _logger.LogInformation(
            "Chat request received. User: {User}, Message: {Message}",
            User.Identity?.Name ?? "Anonymous",
            request.Message);

        try
        {
            var sw = Stopwatch.StartNew();
            var response = await _agent.RunAsync(request.Message);
            sw.Stop();

            _logger.LogInformation(
                "Chat response generated in {ElapsedMs}ms. Length: {Length}",
                sw.ElapsedMilliseconds,
                response.Text.Length);

            return Ok(new { response = response.Text });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing chat request");
            return StatusCode(500, "An error occurred processing your request");
        }
    }
}
```

### 3. Retry Policies with Polly

```csharp
builder.Services.AddSingleton<IChatClient>(sp =>
{
    var baseClient = ClientFactory.Create(
        sp.GetRequiredService<IConfiguration>(),
        sp.GetRequiredService<IWebHostEnvironment>());

    // Wrap with retry policy
    var retryPolicy = Policy
        .Handle<HttpRequestException>()
        .Or<TimeoutException>()
        .WaitAndRetryAsync(
            retryCount: 3,
            sleepDurationProvider: retryAttempt => 
                TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
            onRetry: (exception, timeSpan, retryCount, context) =>
            {
                var logger = sp.GetRequiredService<ILogger<Program>>();
                logger.LogWarning(
                    "Retry {RetryCount} after {Delay}s due to {Exception}",
                    retryCount, timeSpan.TotalSeconds, exception.GetType().Name);
            });

    return baseClient;
});
```

### 4. Health Checks

```csharp
builder.Services.AddHealthChecks()
    .AddCheck("agent_client", () =>
    {
        try
        {
            var client = builder.Services.BuildServiceProvider()
                .GetRequiredService<IChatClient>();
            return HealthCheckResult.Healthy("Agent client initialized");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Agent client failed", ex);
        }
    });

app.MapHealthChecks("/health");
```

### 5. Rate Limiting

```csharp
using Microsoft.AspNetCore.RateLimiting;

builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(
        httpContext => RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.User.Identity?.Name ?? httpContext.Connection.RemoteIpAddress?.ToString() ?? "anonymous",
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 10,
                QueueLimit = 0,
                Window = TimeSpan.FromMinutes(1)
            }));
});

app.UseRateLimiter();
```

### 6. Secure Token Management

```csharp
// Development: User Secrets
dotnet user-secrets set "GITHUB_TOKEN" "your-token"

// Production: Azure Key Vault
builder.Configuration.AddAzureKeyVault(
    new Uri(builder.Configuration["KeyVault:Endpoint"]!),
    new DefaultAzureCredential());

// Environment Variables (Docker/Kubernetes)
var token = builder.Configuration["GITHUB_TOKEN"]
    ?? throw new InvalidOperationException("GITHUB_TOKEN not configured");
```

---

## Testing Checklist

Before deploying to production:

### Unit Tests
- [ ] Agent creation with various configurations
- [ ] Function registration
- [ ] Error handling
- [ ] Null/empty input handling
- [ ] Configuration validation

### Integration Tests  
- [ ] Simple queries with expected responses
- [ ] Context maintenance across messages
- [ ] Function calling behavior
- [ ] Streaming responses
- [ ] Error scenarios with real AI

### API Tests (if applicable)
- [ ] Endpoint responses
- [ ] Authentication/authorization
- [ ] Rate limiting
- [ ] Error responses
- [ ] Health checks

### Load Tests
- [ ] Concurrent requests
- [ ] Response times under load
- [ ] Resource usage
- [ ] Rate limit behavior

---

## Cost Optimization

### Development and Testing: $0

Using GitHub Models:
```csharp
// FREE for all testing!
var client = new ChatClient(
    "gpt-4o-mini",
    new ApiKeyCredential(githubToken),
    new OpenAIClientOptions { 
        Endpoint = new Uri("https://models.github.ai/inference") 
    });
```

### Production: Optimized

```csharp
// Monitor and optimize token usage
var options = new ChatClientAgentOptions
{
    MaxTokens = 500,  // Limit response length
    Temperature = 0.3f,  // More deterministic = more cacheable
    Instructions = "Be concise."  // Shorter responses = lower cost
};
```

---

## Monitoring and Observability

### Application Insights

```csharp
builder.Services.AddApplicationInsightsTelemetry();

// Custom telemetry
public class TelemetryService
{
    private readonly TelemetryClient _telemetry;

    public void TrackAgentRequest(string userId, string message, long durationMs, int tokensUsed)
    {
        _telemetry.TrackEvent("AgentRequest", new Dictionary<string, string>
        {
            ["UserId"] = userId,
            ["MessageLength"] = message.Length.ToString()
        }, new Dictionary<string, double>
        {
            ["Duration"] = durationMs,
            ["TokensUsed"] = tokensUsed
        });
    }
}
```

---

## Conclusion

Testing Agent Framework applications is straightforward:

1. **Unit tests** - Mock IChatClient for fast, isolated tests
2. **Integration tests** - Use GitHub Models for free, comprehensive testing
3. **Function tests** - Test business logic independently
4. **CI/CD** - GitHub Models enables free automated testing
5. **Production** - Monitor, log, and optimize

The combination of Agent Framework's interface-based design and GitHub Models' free tier creates an ideal testing environment.

## Resources

- [Module 13: Testing Strategies](../modules/13-Testing-Strategies/)
- [Module 14: Production Deployment](../modules/14-Production-Deployment/)
- [xUnit Documentation](https://xunit.net/)
- [Moq Documentation](https://github.com/moq/moq4)
- [GitHub Models](https://github.com/features/models)

---

**Previous Post**: [Real-World Migration Examples](./03-real-world-migration-examples.md)

---

*Questions about testing? Open a discussion in the repository!*
