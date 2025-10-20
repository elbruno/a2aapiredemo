using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;

/// <summary>
/// Integration tests using real AI (GitHub Models).
/// </summary>
public class IntegrationTests
{
    private readonly Kernel _kernel;
    private readonly ChatCompletionAgent _agent;

    public IntegrationTests()
    {
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<IntegrationTests>()
            .Build();

        var githubToken = configuration["GITHUB_TOKEN"] 
            ?? throw new InvalidOperationException("GITHUB_TOKEN not configured");

        var kernelBuilder = Kernel.CreateBuilder();
        kernelBuilder.AddOpenAIChatCompletion(
            modelId: "gpt-4o-mini",
            apiKey: githubToken,
            endpoint: new Uri("https://models.github.ai/inference"));

        _kernel = kernelBuilder.Build();
        _kernel.ImportPluginFromType<SupportPlugin>();

        _agent = new ChatCompletionAgent
        {
            Kernel = _kernel,
            Name = "SupportBot",
            Instructions = "You are a customer support assistant. Use the available tools to help customers."
        };
    }

    [Fact]
    public async Task Agent_AnswersSimpleQuestion()
    {
        // Arrange
        var thread = new ChatHistory();
        thread.AddUserMessage("What is 2+2?");

        // Act
        var response = "";
        await foreach (var update in _agent.InvokeAsync(thread))
        {
            dynamic dynamicUpdate = update;
            if (dynamicUpdate.Content is IEnumerable<ChatMessageContent> messages)
            {
                foreach (var message in messages)
                {
                    if (message.Role == AuthorRole.Assistant && !string.IsNullOrWhiteSpace(message.Content))
                    {
                        response = message.Content;
                    }
                }
            }
        }

        // Assert
        Assert.Contains("4", response);
    }

    [Fact]
    public async Task Agent_UsesOrderStatusFunction()
    {
        // Arrange
        var thread = new ChatHistory();
        thread.AddUserMessage("What's the status of order 12345?");

        // Act
        var response = "";
        await foreach (var update in _agent.InvokeAsync(thread))
        {
            dynamic dynamicUpdate = update;
            if (dynamicUpdate.Content is IEnumerable<ChatMessageContent> messages)
            {
                foreach (var message in messages)
                {
                    if (message.Role == AuthorRole.Assistant && !string.IsNullOrWhiteSpace(message.Content))
                    {
                        response = message.Content;
                    }
                }
            }
        }

        // Assert
        Assert.Contains("12345", response);
        Assert.Contains("Shipped", response, StringComparison.OrdinalIgnoreCase);
    }

    [Fact(Skip = "Requires GitHub Token - run manually")]
    public async Task Agent_MaintainsContext()
    {
        // Arrange
        var thread = new ChatHistory();
        
        // First message
        thread.AddUserMessage("Check order 12345");
        await foreach (var update in _agent.InvokeAsync(thread))
        {
            dynamic dynamicUpdate = update;
            if (dynamicUpdate.Content is IEnumerable<ChatMessageContent> messages)
            {
                foreach (var message in messages)
                {
                    if (message.Role == AuthorRole.Assistant && !string.IsNullOrWhiteSpace(message.Content))
                    {
                        thread.AddAssistantMessage(message.Content);
                    }
                }
            }
        }

        // Second message referring to context
        thread.AddUserMessage("Can you refund it?");
        var response = "";
        await foreach (var update in _agent.InvokeAsync(thread))
        {
            dynamic dynamicUpdate = update;
            if (dynamicUpdate.Content is IEnumerable<ChatMessageContent> messages)
            {
                foreach (var message in messages)
                {
                    if (message.Role == AuthorRole.Assistant && !string.IsNullOrWhiteSpace(message.Content))
                    {
                        response = message.Content;
                    }
                }
            }
        }

        // Assert
        Assert.Contains("12345", response);
        Assert.Contains("refund", response, StringComparison.OrdinalIgnoreCase);
    }
}
