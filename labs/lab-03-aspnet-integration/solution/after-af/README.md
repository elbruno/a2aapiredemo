# Lab 03 Solution: Chat API (Agent Framework)

## Overview

This is the solution for Lab 03, demonstrating ASP.NET Core integration with Agent Framework:
- IChatClient dependency injection
- ChatClientAgent for web scenarios
- Simplified invocation with RunAsync/RunStreamingAsync
- Environment-based configuration maintained
- GitHub Models for development, Azure OpenAI for production

## Key Migration Changes

### 1. Kernel → IChatClient

**Before (Semantic Kernel):**
```csharp
builder.Services.AddSingleton<Kernel>(sp =>
{
    var kernelBuilder = Kernel.CreateBuilder();
    kernelBuilder.AddOpenAIChatCompletion(modelId, apiKey, endpoint);
    return kernelBuilder.Build();
});
```

**After (Agent Framework):**
```csharp
builder.Services.AddSingleton<IChatClient>(sp =>
{
    return new ChatClient(
            deploymentName,
            new ApiKeyCredential(githubToken),
            new OpenAIClientOptions { Endpoint = new Uri(endpoint) })
        .AsIChatClient();
});
```

### 2. ChatCompletionAgent → ChatClientAgent

**Before:**
```csharp
builder.Services.AddScoped<ChatCompletionAgent>(sp =>
{
    var kernel = sp.GetRequiredService<Kernel>();
    return new ChatCompletionAgent
    {
        Kernel = kernel,
        Name = "WebAssistant",
        Instructions = "..."
    };
});
```

**After:**
```csharp
builder.Services.AddScoped<ChatClientAgent>(sp =>
{
    var client = sp.GetRequiredService<IChatClient>();
    return new ChatClientAgent(
        client,
        new ChatClientAgentOptions
        {
            Name = "WebAssistant",
            Instructions = "..."
        });
});
```

### 3. Simplified Endpoint Implementation

**Before:**
```csharp
app.MapPost("/api/chat", async (ChatRequest request, ChatCompletionAgent agent) =>
{
    var thread = new ChatHistory();
    thread.AddUserMessage(request.Message);
    
    var response = "";
    await foreach (var update in agent.InvokeAsync(thread))
    {
        // Complex logic to extract response
    }
    
    return Results.Ok(new { response });
});
```

**After:**
```csharp
app.MapPost("/api/chat", async (ChatRequest request, ChatClientAgent agent) =>
{
    var response = await agent.RunAsync(request.Message);
    return Results.Ok(new { response = response.Text });
});
```

### 4. Simplified Streaming

**Before:**
```csharp
async IAsyncEnumerable<string> StreamResponse()
{
    await foreach (var update in agent.InvokeAsync(thread))
    {
        // Complex extraction logic
    }
}
```

**After:**
```csharp
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
```

## Prerequisites

- .NET 9 SDK
- One of the following:
  - GitHub personal access token (for GitHub Models - free!)
  - OpenAI API key
  - Azure OpenAI endpoint and credentials
  - Azure AI Foundry endpoint and credentials

## Configuration

### 1. Initialize User Secrets

```bash
cd labs/lab-03-aspnet-integration/solution/after-af
dotnet user-secrets init
```

### 2. Configure Your AI Provider

Choose one of the following options:

#### Option A: GitHub Models (Free - Default)

```bash
dotnet user-secrets set "AI:Provider" "GitHubModels"
dotnet user-secrets set "GITHUB_TOKEN" "your-github-token-here"
```

#### Option B: OpenAI

```bash
dotnet user-secrets set "AI:Provider" "OpenAI"
dotnet user-secrets set "OpenAI:ApiKey" "your-openai-api-key"
dotnet user-secrets set "OpenAI:ChatDeploymentName" "gpt-4o-mini"
```

#### Option C: Azure OpenAI

```bash
dotnet user-secrets set "AI:Provider" "AzureOpenAI"
dotnet user-secrets set "AzureOpenAI:Endpoint" "https://your-resource.openai.azure.com/"
dotnet user-secrets set "AzureOpenAI:ApiKey" "your-azure-key-here"
dotnet user-secrets set "OpenAI:ChatDeploymentName" "gpt-4o-mini"
```

#### Option D: Azure AI Foundry (Managed Identity)

```bash
dotnet user-secrets set "AI:Provider" "AzureAIFoundry"
dotnet user-secrets set "AzureAIFoundry:Endpoint" "https://your-endpoint.inference.ai.azure.com"
dotnet user-secrets set "OpenAI:ChatDeploymentName" "gpt-4o-mini"
```

#### Option E: Azure AI Foundry (API Key)

```bash
dotnet user-secrets set "AI:Provider" "AzureAIFoundry"
dotnet user-secrets set "AzureAIFoundry:Endpoint" "https://your-endpoint.inference.ai.azure.com"
dotnet user-secrets set "AzureAIFoundry:ApiKey" "your-api-key"
dotnet user-secrets set "OpenAI:ChatDeploymentName" "gpt-4o-mini"
```

## How to Run

```bash
dotnet build
dotnet run
```

The API will be available at:
- HTTPS: https://localhost:5001
- HTTP: http://localhost:5000
- Swagger: https://localhost:5001/swagger

## Testing the API

### Using curl

```bash
# Regular chat
curl -X POST https://localhost:5001/api/chat \
  -H "Content-Type: application/json" \
  -d '{"message":"Hello, who are you?"}'

# Health check
curl https://localhost:5001/health
```

### Using Swagger UI

1. Navigate to https://localhost:5001/swagger
2. Try out the `/api/chat` endpoint
3. Enter a message in the request body

## Benefits of Agent Framework

1. **Simpler Code**: ~40% fewer lines in endpoint implementations
2. **Direct Patterns**: No ChatHistory management needed
3. **Clear Response**: Direct access to `.Text` property
4. **Better DI**: Natural singleton/scoped patterns
5. **Easier Streaming**: Simplified async enumeration

## Environment Support

- **Development**: GitHub Models (free, no cost)
- **Production**: Azure OpenAI (switch seamlessly with same code patterns)

## API Endpoints

- `POST /api/chat` - Send a message and get a response
- `POST /api/chat/stream` - Send a message and get streaming response
- `GET /health` - Health check endpoint
