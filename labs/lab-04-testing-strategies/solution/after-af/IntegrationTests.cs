using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using OpenAI;
using OpenAI.Chat;
using System.ClientModel;

/// <summary>
/// Integration tests using real AI (GitHub Models).
/// </summary>
public class IntegrationTests
{
    private readonly IChatClient _chatClient;
    private readonly ChatClientAgent _agent;

    public IntegrationTests()
    {
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<IntegrationTests>()
            .Build();

        var githubToken = configuration["GITHUB_TOKEN"] 
            ?? throw new InvalidOperationException("GITHUB_TOKEN not configured");

        _chatClient = new ChatClient(
                "gpt-4o-mini",
                new ApiKeyCredential(githubToken),
                new OpenAIClientOptions { Endpoint = new Uri("https://models.github.ai/inference") })
            .AsIChatClient();

        _agent = new ChatClientAgent(
            _chatClient,
            new ChatClientAgentOptions
            {
                Name = "SupportBot",
                Instructions = "You are a customer support assistant. Use the available tools to help customers."
            });
    }

    [Fact]
    public async Task Agent_AnswersSimpleQuestion()
    {
        // Arrange & Act
        var response = await _agent.RunAsync("What is 2+2?");

        // Assert
        Assert.Contains("4", response.Text);
    }

    [Fact]
    public async Task Agent_RespondsToGreeting()
    {
        // Arrange & Act
        var response = await _agent.RunAsync("Hello!");

        // Assert
        Assert.NotNull(response.Text);
        Assert.NotEmpty(response.Text);
    }

    [Fact(Skip = "Requires GitHub Token - run manually")]
    public async Task Agent_ProvidesHelpfulResponse()
    {
        // Arrange & Act
        var response = await _agent.RunAsync("What can you help me with?");

        // Assert
        Assert.NotNull(response.Text);
        Assert.True(response.Text.Length > 10, "Response should be substantive");
    }

    [Fact(Skip = "Requires GitHub Token and function registration support - run manually")]
    public async Task Agent_UsesOrderStatusFunction()
    {
        // Note: This test demonstrates the pattern for when function registration is supported
        // Arrange
        var message = "What's the status of order 12345?";

        // Act
        var response = await _agent.RunAsync(message);

        // Assert
        Assert.NotNull(response.Text);
        // In a full implementation, we'd verify the function was called
    }

    [Theory]
    [InlineData("Hello")]
    [InlineData("What is 5+5?")]
    [InlineData("Tell me a short fact")]
    public async Task Agent_HandlesVariousQuestions(string question)
    {
        // Act
        var response = await _agent.RunAsync(question);

        // Assert
        Assert.NotNull(response.Text);
        Assert.NotEmpty(response.Text);
    }
}
