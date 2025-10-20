# Lab 03 Starter: Chat API (Semantic Kernel)

## Overview

This is the starting point for Lab 03. It demonstrates an ASP.NET Core Web API with Semantic Kernel:
- Environment-based configuration (GitHub Models for dev, Azure OpenAI for prod)
- Dependency injection for Kernel and ChatCompletionAgent
- REST API endpoints for chat
- Streaming responses support
- Health checks
- Swagger/OpenAPI documentation

## Prerequisites

- .NET 9 SDK
- GitHub personal access token (for development)
- Azure OpenAI credentials (for production, optional)

## Configuration

### 1. Initialize User Secrets

```bash
cd labs/lab-03-aspnet-integration/starter/before-sk
dotnet user-secrets init
```

### 2. Set GitHub Token (Development)

```bash
dotnet user-secrets set "GITHUB_TOKEN" "your-github-token-here"
```

### 3. Set Azure OpenAI (Production, Optional)

```bash
dotnet user-secrets set "AzureOpenAI:Endpoint" "https://your-resource.openai.azure.com/"
dotnet user-secrets set "AzureOpenAI:ApiKey" "your-azure-key-here"
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

## Key Concepts

### Environment-Based Configuration

```csharp
if (env.IsDevelopment())
{
    // GitHub Models (free!)
    kernelBuilder.AddOpenAIChatCompletion(
        modelId: deploymentName,
        apiKey: githubToken,
        endpoint: new Uri(endpoint));
}
else
{
    // Azure OpenAI (production)
    kernelBuilder.AddAzureOpenAIChatCompletion(
        deploymentName: deploymentName,
        endpoint: azureEndpoint,
        apiKey: azureKey);
}
```

### Dependency Injection

```csharp
// Singleton Kernel
builder.Services.AddSingleton<Kernel>(...);

// Scoped Agent
builder.Services.AddScoped<ChatCompletionAgent>(...);
```

### Minimal API Endpoints

```csharp
app.MapPost("/api/chat", async (ChatRequest request, ChatCompletionAgent agent) =>
{
    // Handle request
    return Results.Ok(new { response });
});
```

## Your Task

Migrate this application to Agent Framework in the `solution/after-af/` directory. Focus on:
1. Replacing Kernel with IChatClient
2. Using ChatClientAgent instead of ChatCompletionAgent
3. Simplifying the invocation pattern
4. Maintaining environment-based configuration

Compare your solution with `solution/after-af/` when complete.
