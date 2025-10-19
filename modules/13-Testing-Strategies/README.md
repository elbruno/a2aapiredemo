# Module 13: Testing Strategies

**Duration**: 20 minutes  
**Level**: Advanced  

---

## Testing Agent Framework Applications

This module covers comprehensive testing strategies for Agent Framework applications using GitHub Models.

---

## Unit Testing Agents

### Mock IChatClient

```csharp
using Microsoft.Extensions.AI;
using Moq;
using Xunit;

public class AgentTests
{
    [Fact]
    public async Task Agent_ReturnsExpectedResponse()
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
                Message = new ChatMessage(ChatRole.Assistant, "Mock response")
            });

        var agent = new ChatClientAgent(
            mockClient.Object,
            new ChatClientAgentOptions { Name = "TestAgent" });

        // Act
        var response = await agent.RunAsync("Test message");

        // Assert
        Assert.Equal("Mock response", response.Text);
    }
}
```

---

## Integration Testing with GitHub Models

### Test Configuration

```csharp
public class IntegrationTests : IDisposable
{
    private readonly IChatClient _chatClient;
    private readonly ChatClientAgent _agent;

    public IntegrationTests()
    {
        var config = new ConfigurationBuilder()
            .AddUserSecrets<IntegrationTests>()
            .Build();

        var githubToken = config["GITHUB_TOKEN"];

        _chatClient = new ChatClient(
            "gpt-4o-mini",
            new ApiKeyCredential(githubToken!),
            new OpenAIClientOptions { 
                Endpoint = new Uri("https://models.github.ai/inference") 
            }).AsIChatClient();

        _agent = new ChatClientAgent(_chatClient, new() 
        { 
            Name = "TestAgent",
            Temperature = 0.0f  // Deterministic for testing
        });
    }

    [Fact]
    public async Task Agent_RespondsToSimpleQuery()
    {
        var response = await _agent.RunAsync("What is 2+2?");
        Assert.Contains("4", response.Text);
    }

    public void Dispose()
    {
        // Cleanup if needed
    }
}
```

---

## Testing Functions/Tools

```csharp
public class ToolTests
{
    [Theory]
    [InlineData("Seattle", "Seattle")]
    [InlineData("New York", "New York")]
    public void GetWeather_ReturnsLocationInResponse(string input, string expected)
    {
        // Test function directly
        string GetWeather(string location) => $"Weather in {location}: Sunny";
        
        var result = GetWeather(input);
        
        Assert.Contains(expected, result);
    }
}
```

---

## Testing ASP.NET Core Endpoints

```csharp
public class ChatControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ChatControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Post_ReturnsSuccessAndResponse()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = new { message = "Hello" };

        // Act
        var response = await client.PostAsJsonAsync("/api/chat", request);

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<ChatResponse>();
        Assert.NotNull(result?.Response);
    }
}

record ChatResponse(string Response);
```

---

## Test Patterns

### Pattern 1: Deterministic Testing
```csharp
var agent = new ChatClientAgent(client, new()
{
    Temperature = 0.0f  // Most deterministic
});
```

### Pattern 2: Response Validation
```csharp
[Fact]
public async Task Agent_ContainsKeyword()
{
    var response = await agent.RunAsync("Tell me about dogs");
    Assert.Contains("dog", response.Text, StringComparison.OrdinalIgnoreCase);
}
```

### Pattern 3: Function Call Verification
```csharp
var functionCalled = false;
string TestFunction(string input)
{
    functionCalled = true;
    return "result";
}

var agent = new ChatClientAgent(client, new() 
{ 
    Tools = { TestFunction } 
});

await agent.RunAsync("Call the test function");
Assert.True(functionCalled);
```

---

## Best Practices

1. **Use GitHub Models for integration tests** - Free and fast
2. **Mock for unit tests** - Fast, isolated tests
3. **Set Temperature=0** - More predictable responses
4. **Test functions independently** - Before integrating with agent
5. **Use User Secrets** - Keep tokens secure
6. **Test error handling** - Verify graceful failures

---

## Test Project Structure

```
tests/
├── Unit/
│   ├── AgentTests.cs
│   ├── FunctionTests.cs
│   └── ConfigurationTests.cs
├── Integration/
│   ├── AgentIntegrationTests.cs
│   └── EndToEndTests.cs
└── TestHelpers/
    ├── MockChatClient.cs
    └── TestConfiguration.cs
```

---

## Example Test Suite

```csharp
public class ComprehensiveAgentTests
{
    private readonly IChatClient _mockClient;

    public ComprehensiveAgentTests()
    {
        var mock = new Mock<IChatClient>();
        mock.Setup(c => c.CompleteAsync(
            It.IsAny<IList<ChatMessage>>(),
            It.IsAny<ChatOptions>(),
            It.IsAny<CancellationToken>()))
        .ReturnsAsync((IList<ChatMessage> msgs, ChatOptions opts, CancellationToken ct) =>
            new ChatCompletion 
            { 
                Message = new ChatMessage(ChatRole.Assistant, $"Processed: {msgs.Last().Text}") 
            });

        _mockClient = mock.Object;
    }

    [Fact]
    public async Task Agent_CreatesSuccessfully()
    {
        var agent = new ChatClientAgent(_mockClient, new() { Name = "Test" });
        Assert.NotNull(agent);
    }

    [Fact]
    public async Task Agent_ProcessesMessage()
    {
        var agent = new ChatClientAgent(_mockClient, new() { Name = "Test" });
        var response = await agent.RunAsync("Hello");
        Assert.Contains("Hello", response.Text);
    }

    [Fact]
    public async Task Agent_WithTools_CallsFunction()
    {
        var called = false;
        string TestFunc() { called = true; return "result"; }

        var agent = new ChatClientAgent(_mockClient, new() 
        { 
            Tools = { TestFunc } 
        });

        // In real scenario, agent would call function based on AI response
        Assert.NotNull(agent.Options.Tools);
    }
}
```

---

## Next: [Module 14: Production Deployment](../14-Production-Deployment/)
