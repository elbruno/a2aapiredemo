using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI;
using OpenAI.Chat;
using Azure.AI.OpenAI;
using System.ClientModel;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure IChatClient with environment-based settings
builder.Services.AddSingleton<IChatClient>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var env = sp.GetRequiredService<IWebHostEnvironment>();
    
    var deploymentName = config["OpenAI:ChatDeploymentName"] ?? "gpt-4o-mini";
    
    if (env.IsDevelopment())
    {
        // Use GitHub Models in development (free!)
        var githubToken = config["GITHUB_TOKEN"] 
            ?? throw new InvalidOperationException("GITHUB_TOKEN not configured");
        var endpoint = config["OpenAI:Endpoint"] ?? "https://models.github.ai/inference";
        
        return new ChatClient(
                deploymentName,
                new ApiKeyCredential(githubToken),
                new OpenAIClientOptions { Endpoint = new Uri(endpoint) })
            .AsIChatClient();
    }
    else
    {
        // Use Azure OpenAI in production
        var azureEndpoint = config["AzureOpenAI:Endpoint"] 
            ?? throw new InvalidOperationException("AzureOpenAI:Endpoint not configured");
        var azureKey = config["AzureOpenAI:ApiKey"] 
            ?? throw new InvalidOperationException("AzureOpenAI:ApiKey not configured");
        
        var azureClient = new AzureOpenAIClient(
            new Uri(azureEndpoint),
            new Azure.AzureKeyCredential(azureKey));
        return azureClient.GetChatClient(deploymentName).AsIChatClient();
    }
});

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
