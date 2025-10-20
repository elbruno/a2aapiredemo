using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI;
using OpenAI.Chat;
using Azure.AI.OpenAI;
using Azure.AI.Inference;
using Azure.Identity;
using System.ClientModel;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure IChatClient with environment-based settings
builder.Services.AddSingleton<IChatClient>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var deploymentName = config["OpenAI:ChatDeploymentName"] ?? "gpt-4o-mini";
    var provider = config["AI:Provider"] ?? "GitHubModels"; // Default to GitHub Models

    return provider.ToUpperInvariant() switch
    {
        "GITHUBMODELS" => CreateGitHubModelsClient(config, deploymentName),
        "OPENAI" => CreateOpenAIClient(config, deploymentName),
        "AZUREAIFOUNDRY" => CreateAzureAIFoundryClient(config, deploymentName),
        "AZUREOPENAI" => CreateAzureOpenAIClient(config, deploymentName),
        _ => throw new InvalidOperationException($"Unknown AI provider: {provider}. Valid options: GitHubModels, OpenAI, AzureOpenAI, AzureAIFoundry")
    };
});

static IChatClient CreateGitHubModelsClient(IConfiguration config, string deploymentName)
{
    var githubToken = config["GITHUB_TOKEN"]
        ?? throw new InvalidOperationException("GITHUB_TOKEN not configured for GitHub Models");
    var endpoint = config["OpenAI:Endpoint"] ?? "https://models.github.ai/inference";

    return new ChatClient(
            deploymentName,
            new ApiKeyCredential(githubToken),
            new OpenAIClientOptions { Endpoint = new Uri(endpoint) })
        .AsIChatClient();
}

static IChatClient CreateOpenAIClient(IConfiguration config, string deploymentName)
{
    var apiKey = config["OpenAI:ApiKey"]
        ?? throw new InvalidOperationException("OpenAI:ApiKey not configured for OpenAI");

    return new ChatClient(deploymentName, new ApiKeyCredential(apiKey))
        .AsIChatClient();
}

static IChatClient CreateAzureOpenAIClient(IConfiguration config, string deploymentName)
{
    var endpoint = config["AzureOpenAI:Endpoint"]
        ?? throw new InvalidOperationException("AzureOpenAI:Endpoint not configured");
    var apiKey = config["AzureOpenAI:ApiKey"];

    if (!string.IsNullOrEmpty(apiKey))
    {
        var azureClient = new AzureOpenAIClient(
            new Uri(endpoint),
            new Azure.AzureKeyCredential(apiKey));
        return azureClient.GetChatClient(deploymentName).AsIChatClient();
    }
    else
    {
        var azureClient = new AzureOpenAIClient(
            new Uri(endpoint),
            new DefaultAzureCredential());
        return azureClient.GetChatClient(deploymentName).AsIChatClient();
    }
}

static IChatClient CreateAzureAIFoundryClient(IConfiguration config, string deploymentName)
{
    var endpoint = config["AzureAIFoundry:Endpoint"]
        ?? throw new InvalidOperationException("AzureAIFoundry:Endpoint not configured");
    var apiKey = config["AzureAIFoundry:ApiKey"];

    // Use managed identity (DefaultAzureCredential) by default, or API key if provided
    if (!string.IsNullOrEmpty(apiKey))
    {
        return new ChatCompletionsClient(
                new Uri(endpoint),
                new Azure.AzureKeyCredential(apiKey))
            .AsIChatClient(deploymentName);
    }
    else
    {
        return new ChatCompletionsClient(
                new Uri(endpoint),
                new DefaultAzureCredential())
            .AsIChatClient(deploymentName);
    }
}

// Register ChatClientAgent as scoped
builder.Services.AddScoped<ChatClientAgent>(sp =>
{
    var client = sp.GetRequiredService<IChatClient>();
    return new ChatClientAgent(
        client,
        new ChatClientAgentOptions
        {
            Name = "WebAssistant",
            Instructions = "You are a helpful web assistant. Provide clear and concise answers."
        });
});

builder.Services.AddHealthChecks();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapHealthChecks("/health");

// Chat endpoint
app.MapPost("/api/chat", async (ChatRequest request, ChatClientAgent agent) =>
{
    var response = await agent.RunAsync(request.Message);
    return Results.Ok(new { response = response.Text });
})
.WithName("Chat")
.WithOpenApi();

// Streaming chat endpoint
app.MapPost("/api/chat/stream", async (ChatRequest request, ChatClientAgent agent) =>
{
    async IAsyncEnumerable<string> StreamResponse()
    {
        await foreach (var update in agent.RunStreamingAsync(request.Message))
        {
            if (!string.IsNullOrEmpty(update.Text))
            {
                yield return update.Text;
            }
        }
    }
    
    return Results.Ok(StreamResponse());
})
.WithName("ChatStream")
.WithOpenApi();

app.Run();

public record ChatRequest(string Message);
