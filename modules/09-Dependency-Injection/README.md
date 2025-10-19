# Module 09: Dependency Injection

**Duration**: 15 minutes  
**Level**: Intermediate  
**Prerequisites**: ASP.NET Core knowledge

---

## Learning Objectives

- Register Agent Framework services in DI container
- Inject `IChatClient` and agents
- Configure for ASP.NET Core applications
- Use GitHub Models in web apps

---

## ASP.NET Core Integration

### Program.cs Setup

```csharp
using Microsoft.Extensions.AI;
using Microsoft.Agents.AI;
using OpenAI.Chat;
using System.ClientModel;

var builder = WebApplication.CreateBuilder(args);

// Register IChatClient for GitHub Models
builder.Services.AddSingleton<IChatClient>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var githubToken = config["GITHUB_TOKEN"]!;
    
    var chatClient = new ChatClient(
        "gpt-4o-mini",
        new ApiKeyCredential(githubToken),
        new OpenAIClientOptions { 
            Endpoint = new Uri("https://models.github.ai/inference") 
        });
    
    return chatClient.AsIChatClient();
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
            Instructions = "You are a helpful web assistant."
        });
});

builder.Services.AddControllers();
var app = builder.Build();

app.MapControllers();
app.Run();
```

---

## Controller Usage

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.Agents.AI;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly ChatClientAgent _agent;

    public ChatController(ChatClientAgent agent)
    {
        _agent = agent;
    }

    [HttpPost]
    public async Task<IActionResult> Chat([FromBody] ChatRequest request)
    {
        var response = await _agent.RunAsync(request.Message);
        return Ok(new { response = response.Text });
    }
}

public record ChatRequest(string Message);
```

---

## Configuration

### appsettings.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  }
}
```

### User Secrets (Development)

```bash
dotnet user-secrets set "GITHUB_TOKEN" "your-github-token"
```

### Environment Variables (Production)

```bash
export GITHUB_TOKEN="your-token"
```

---

## Lifetime Scopes

### Singleton (Recommended for IChatClient)
```csharp
builder.Services.AddSingleton<IChatClient>(...);
```
- One instance for entire app
- Best for stateless clients

### Scoped (Recommended for Agents)
```csharp
builder.Services.AddScoped<ChatClientAgent>(...);
```
- One instance per request
- Maintains conversation context per request

### Transient (For Specialized Cases)
```csharp
builder.Services.AddTransient<ChatClientAgent>(...);
```
- New instance each time
- Use for completely independent conversations

---

## Minimal API Example

```csharp
var app = builder.Build();

app.MapPost("/chat", async (
    ChatRequest request,
    ChatClientAgent agent) =>
{
    var response = await agent.RunAsync(request.Message);
    return Results.Ok(new { response = response.Text });
});

app.Run();
```

---

## Multiple Agents

```csharp
// Register multiple agents with different configurations
builder.Services.AddScoped<ChatClientAgent>(sp =>
{
    var client = sp.GetRequiredService<IChatClient>();
    return new ChatClientAgent(client, new() { Name = "GeneralAssistant" });
});

builder.Services.AddScoped(sp =>
{
    var client = sp.GetRequiredService<IChatClient>();
    return new ChatClientAgent(client, new() 
    { 
        Name = "CodeHelper",
        Instructions = "You are a coding assistant."
    });
});
```

---

## Best Practices

1. **Singleton for Client** - IChatClient should be singleton
2. **Scoped for Agents** - One agent per request
3. **Secure Tokens** - Never hardcode, use secrets
4. **Error Handling** - Handle agent failures gracefully
5. **Logging** - Add logging for debugging

---

## Next: [Module 10: Options Configuration](../10-Options-Configuration/)
