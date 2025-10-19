# Lab 04: Testing Strategies

**Duration**: 45 minutes  
**Level**: Advanced  
**Prerequisites**: Labs 01-03 completed

## Learning Objectives

- Write unit tests with mocked IChatClient
- Create integration tests with GitHub Models
- Test functions independently
- Set up CI/CD testing with GitHub Actions
- Apply testing best practices

## Scenario

Build comprehensive test suite for a chatbot that:
- Handles customer support queries
- Calls multiple functions
- Maintains conversation context
- Integrates with external services

## Lab Structure

### Starter Code (`starter/`)

Test project with:
- `AgentTests.cs` - Empty test class for unit tests
- `IntegrationTests.cs` - Empty test class for integration tests
- `FunctionTests.cs` - Empty test class for function tests

### Your Tasks

#### Task 1: Unit Tests with Mocks

Create tests using Moq to mock IChatClient:

```csharp
[Fact]
public async Task Agent_RespondsCorrectly()
{
    // Arrange
    var mockClient = new Mock<IChatClient>();
    mockClient.Setup(c => c.CompleteAsync(
        It.IsAny<IList<ChatMessage>>(),
        It.IsAny<ChatOptions>(),
        It.IsAny<CancellationToken>()))
    .ReturnsAsync(new ChatCompletion
    {
        Message = new ChatMessage(ChatRole.Assistant, "Test response")
    });

    var agent = new ChatClientAgent(mockClient.Object, new() { Name = "Test" });

    // Act
    var response = await agent.RunAsync("Hello");

    // Assert
    Assert.Equal("Test response", response.Text);
}
```

#### Task 2: Integration Tests with GitHub Models

Use real AI for comprehensive testing (free with GitHub Models!):

```csharp
public class IntegrationTests
{
    private readonly IChatClient _client;

    public IntegrationTests()
    {
        var githubToken = Environment.GetEnvironmentVariable("GITHUB_TOKEN");
        _client = new ChatClient(
            "gpt-4o-mini",
            new ApiKeyCredential(githubToken!),
            new OpenAIClientOptions { 
                Endpoint = new Uri("https://models.github.ai/inference") 
            }).AsIChatClient();
    }

    [Fact]
    public async Task Agent_AnswersSimpleQuestion()
    {
        var agent = new ChatClientAgent(_client, new() 
        { 
            Temperature = 0.0f  // Deterministic
        });

        var response = await agent.RunAsync("What is 2+2?");

        Assert.Contains("4", response.Text);
    }
}
```

#### Task 3: Function Tests

Test your functions independently:

```csharp
[Theory]
[InlineData("Seattle", "Seattle")]
[InlineData("Tokyo", "Tokyo")]
public void GetWeather_ReturnsCorrectCity(string input, string expected)
{
    string GetWeather(string city) => $"Weather in {city}: Sunny";
    
    var result = GetWeather(input);
    
    Assert.Contains(expected, result);
}
```

#### Task 4: GitHub Actions Setup

Create `.github/workflows/test.yml`:

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

## Testing Patterns

### Pattern 1: Deterministic Testing
Use `Temperature = 0.0f` for predictable results

### Pattern 2: Keyword Assertions
```csharp
Assert.Contains("expected_keyword", response.Text, StringComparison.OrdinalIgnoreCase);
```

### Pattern 3: Function Call Verification
Test that functions are called when expected

### Pattern 4: Context Maintenance
```csharp
await agent.RunAsync("My name is Alice");
var response = await agent.RunAsync("What's my name?");
Assert.Contains("Alice", response.Text);
```

## Verification

Run all tests:
```bash
dotnet test
```

Expected output:
- All unit tests pass
- All integration tests pass (with GitHub Models)
- All function tests pass
- Total test time < 30 seconds (with mocks)

## Key Concepts

- **Unit tests**: Fast, isolated, use mocks
- **Integration tests**: Real AI, use GitHub Models (free!)
- **Function tests**: Test business logic separately
- **CI/CD**: GitHub Actions runs tests automatically

## Best Practices Demonstrated

1. ✅ Mock external dependencies
2. ✅ Use free GitHub Models for integration tests
3. ✅ Test functions independently
4. ✅ Keep tests fast and reliable
5. ✅ Automate testing in CI/CD

## Solution

Check `solution/` for:
- Complete test suite
- GitHub Actions workflow
- Test helpers and utilities
- 90%+ code coverage

---

## Congratulations! 🎉

You've completed all 4 hands-on labs! You now know how to:

✅ Migrate basic Semantic Kernel apps to Agent Framework
✅ Handle complex tools and async functions
✅ Build ASP.NET Core applications with agents
✅ Test comprehensively with mocks and GitHub Models

## What's Next?

- **Explore [all 15 modules](../../modules/)** for deep dives
- **Read [blog posts](../../blog-posts/)** for real-world examples
- **Build your own project** using what you've learned
- **Contribute to the community** - share your experiences!

## Resources

- [Module 13: Testing Strategies](../../modules/13-Testing-Strategies/)
- [Module 14: Production Deployment](../../modules/14-Production-Deployment/)
- [Quick Reference](../../docs/QUICK-REFERENCE.md)
- [FAQ](../../docs/FAQ.md)

---

**Thank you for completing the labs!** Happy coding with Agent Framework! 🚀
