using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure Semantic Kernel with environment-based settings
builder.Services.AddSingleton<Kernel>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var env = sp.GetRequiredService<IWebHostEnvironment>();
    
    var deploymentName = config["OpenAI:ChatDeploymentName"] ?? "gpt-4o-mini";
    
    var kernelBuilder = Kernel.CreateBuilder();
    
    if (env.IsDevelopment())
    {
        // Use GitHub Models in development (free!)
        var githubToken = config["GITHUB_TOKEN"] 
            ?? throw new InvalidOperationException("GITHUB_TOKEN not configured");
        var endpoint = config["OpenAI:Endpoint"] ?? "https://models.github.ai/inference";
        
        kernelBuilder.AddOpenAIChatCompletion(
            modelId: deploymentName,
            apiKey: githubToken,
            endpoint: new Uri(endpoint));
    }
    else
    {
        // Use Azure OpenAI in production
        var azureEndpoint = config["AzureOpenAI:Endpoint"] 
            ?? throw new InvalidOperationException("AzureOpenAI:Endpoint not configured");
        var azureKey = config["AzureOpenAI:ApiKey"] 
            ?? throw new InvalidOperationException("AzureOpenAI:ApiKey not configured");
        
        kernelBuilder.AddAzureOpenAIChatCompletion(
            deploymentName: deploymentName,
            endpoint: azureEndpoint,
            apiKey: azureKey);
    }
    
    return kernelBuilder.Build();
});

// Register ChatCompletionAgent as scoped
builder.Services.AddScoped<ChatCompletionAgent>(sp =>
{
    var kernel = sp.GetRequiredService<Kernel>();
    return new ChatCompletionAgent
    {
        Kernel = kernel,
        Name = "WebAssistant",
        Instructions = "You are a helpful web assistant. Provide clear and concise answers."
    };
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
app.MapPost("/api/chat", async (ChatRequest request, ChatCompletionAgent agent) =>
{
    var thread = new ChatHistory();
    thread.AddUserMessage(request.Message);
    
    var response = "";
    await foreach (var update in agent.InvokeAsync(thread))
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
        else if (dynamicUpdate.Message is ChatMessageContent singleMessage)
        {
            if (singleMessage.Role == AuthorRole.Assistant && !string.IsNullOrWhiteSpace(singleMessage.Content))
            {
                response = singleMessage.Content;
            }
        }
    }
    
    return Results.Ok(new { response });
})
.WithName("Chat")
.WithOpenApi();

// Streaming chat endpoint
app.MapPost("/api/chat/stream", async (ChatRequest request, ChatCompletionAgent agent) =>
{
    var thread = new ChatHistory();
    thread.AddUserMessage(request.Message);
    
    async IAsyncEnumerable<string> StreamResponse()
    {
        await foreach (var update in agent.InvokeAsync(thread))
        {
            dynamic dynamicUpdate = update;
            if (dynamicUpdate.Content is IEnumerable<ChatMessageContent> messages)
            {
                foreach (var message in messages)
                {
                    if (message.Role == AuthorRole.Assistant && !string.IsNullOrWhiteSpace(message.Content))
                    {
                        yield return message.Content;
                    }
                }
            }
            else if (dynamicUpdate.Message is ChatMessageContent singleMessage)
            {
                if (singleMessage.Role == AuthorRole.Assistant && !string.IsNullOrWhiteSpace(singleMessage.Content))
                {
                    yield return singleMessage.Content;
                }
            }
        }
    }
    
    return Results.Ok(StreamResponse());
})
.WithName("ChatStream")
.WithOpenApi();

app.Run();

public record ChatRequest(string Message);
