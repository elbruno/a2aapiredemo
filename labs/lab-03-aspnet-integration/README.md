# Lab 03: ASP.NET Core Integration

**Duration**: 60 minutes  
**Level**: Intermediate  
**Prerequisites**: Lab 01 and 02 completed

## Learning Objectives

- Set up Agent Framework in ASP.NET Core
- Configure dependency injection for IChatClient and agents
- Create REST API endpoints for chat
- Implement streaming responses over HTTP
- Add health checks and monitoring

## Scenario

Build a web API chatbot that:
- Accepts chat messages via POST endpoint
- Returns AI responses as JSON
- Supports streaming responses
- Includes health checks
- Uses GitHub Models in development, Azure OpenAI in production

## Lab Structure

### Starter Code (`starter/`)

Minimal ASP.NET Core project with:
- `Program.cs` - Basic web app setup
- `ChatController.cs` - Empty controller
- `appsettings.json` - Configuration template

### Your Tasks

#### Task 1: Configure Services (Program.cs)

Register IChatClient with environment-based configuration:
```csharp
builder.Services.AddSingleton<IChatClient>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var env = sp.GetRequiredService<IWebHostEnvironment>();
    
    if (env.IsDevelopment())
    {
        // Use GitHub Models (free)
        var githubToken = config["GITHUB_TOKEN"];
        return new ChatClient("gpt-4o-mini", 
            new ApiKeyCredential(githubToken!),
            new OpenAIClientOptions { 
                Endpoint = new Uri("https://models.github.ai/inference") 
            }).AsIChatClient();
    }
    else
    {
        // Use Azure OpenAI (production)
        var azureKey = config["AzureOpenAI:ApiKey"];
        return new ChatClient(
            deployment: "gpt-4",
            endpoint: new Uri(config["AzureOpenAI:Endpoint"]!),
            credential: new AzureKeyCredential(azureKey!)).AsIChatClient();
    }
});
```

Register ChatClientAgent as scoped:
```csharp
builder.Services.AddScoped<ChatClientAgent>(sp =>
{
    var client = sp.GetRequiredService<IChatClient>();
    return new ChatClientAgent(client, new()
    {
        Name = "WebAssistant",
        Instructions = "You are a helpful web assistant."
    });
});
```

#### Task 2: Implement Chat Endpoint

```csharp
[HttpPost]
public async Task<ActionResult> Chat([FromBody] ChatRequest request)
{
    var response = await _agent.RunAsync(request.Message);
    return Ok(new { response = response.Text });
}

public record ChatRequest(string Message);
```

#### Task 3: Implement Streaming Endpoint

```csharp
[HttpPost("stream")]
public async IAsyncEnumerable<string> ChatStream([FromBody] ChatRequest request)
{
    await foreach (var update in _agent.RunStreamingAsync(request.Message))
    {
        yield return update.Text;
    }
}
```

#### Task 4: Add Health Check

```csharp
builder.Services.AddHealthChecks()
    .AddCheck("agent_ready", () => HealthCheckResult.Healthy());

app.MapHealthChecks("/health");
```

## Testing

1. **Run the application**
   ```bash
   dotnet run
   ```

2. **Test with curl or Postman**
   ```bash
   curl -X POST https://localhost:5001/api/chat \
     -H "Content-Type: application/json" \
     -d '{"message":"Hello"}'
   ```

3. **Test health check**
   ```bash
   curl https://localhost:5001/health
   ```

## Key Concepts

- **Singleton vs Scoped**: Client is singleton, agent is scoped
- **Environment-based config**: Different providers per environment
- **Streaming over HTTP**: IAsyncEnumerable support
- **Health checks**: Monitor service availability

## Bonus: Add Rate Limiting

```csharp
builder.Services.AddRateLimiter(options => { ... });
app.UseRateLimiter();
```

## Solution

Check `solution/` for complete implementation including:
- Full dependency injection setup
- Both regular and streaming endpoints
- Logging and error handling
- Health checks

## Next: [Lab 04: Testing Strategies](../lab-04-testing-strategies/)
