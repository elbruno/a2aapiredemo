# Module 15: ASP.NET Core Deep Dive

**Duration**: 25 minutes  
**Level**: Advanced  

---

## ASP.NET Core Integration with Agent Framework

Complete guide to integrating Agent Framework in ASP.NET Core applications with GitHub Models.

---

## Minimal API Example

```csharp
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI.Chat;
using System.ClientModel;

var builder = WebApplication.CreateBuilder(args);

// Register services
builder.Services.AddSingleton<IChatClient>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var githubToken = config["GITHUB_TOKEN"]!;

    return new ChatClient(
        "gpt-4o-mini",
        new ApiKeyCredential(githubToken),
        new OpenAIClientOptions { 
            Endpoint = new Uri("https://models.github.ai/inference") 
        }).AsIChatClient();
});

builder.Services.AddScoped<ChatClientAgent>(sp =>
{
    var client = sp.GetRequiredService<IChatClient>();
    return new ChatClientAgent(client, new()
    {
        Name = "WebAssistant",
        Instructions = "You are a helpful web assistant."
    });
});

var app = builder.Build();

// Endpoints
app.MapPost("/api/chat", async (ChatRequest request, ChatClientAgent agent) =>
{
    var response = await agent.RunAsync(request.Message);
    return Results.Ok(new { response = response.Text });
});

app.MapPost("/api/chat/stream", async (ChatRequest request, ChatClientAgent agent) =>
{
    return Results.Stream(async stream =>
    {
        await foreach (var update in agent.RunStreamingAsync(request.Message))
        {
            await stream.WriteAsync(System.Text.Encoding.UTF8.GetBytes(update.Text));
            await stream.FlushAsync();
        }
    }, contentType: "text/plain");
});

app.Run();

record ChatRequest(string Message);
```

---

## Controller-Based API

```csharp
[ApiController]
[Route("api/[controller]")]
public class AgentController : ControllerBase
{
    private readonly ChatClientAgent _agent;
    private readonly ILogger<AgentController> _logger;

    public AgentController(ChatClientAgent agent, ILogger<AgentController> logger)
    {
        _agent = agent;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<ChatResponse>> Chat([FromBody] ChatRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Message))
            return BadRequest("Message cannot be empty");

        try
        {
            var response = await _agent.RunAsync(request.Message);
            return Ok(new ChatResponse(response.Text));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing chat request");
            return StatusCode(500, "An error occurred");
        }
    }

    [HttpPost("stream")]
    public async IAsyncEnumerable<string> ChatStream([FromBody] ChatRequest request)
    {
        await foreach (var update in _agent.RunStreamingAsync(request.Message))
        {
            yield return update.Text;
        }
    }
}

public record ChatRequest(string Message);
public record ChatResponse(string Response);
```

---

## SignalR Integration

```csharp
using Microsoft.AspNetCore.SignalR;

public class ChatHub : Hub
{
    private readonly ChatClientAgent _agent;

    public ChatHub(ChatClientAgent agent)
    {
        _agent = agent;
    }

    public async Task SendMessage(string message)
    {
        await foreach (var update in _agent.RunStreamingAsync(message))
        {
            await Clients.Caller.SendAsync("ReceiveMessage", update.Text);
        }
    }
}

// Program.cs
builder.Services.AddSignalR();
app.MapHub<ChatHub>("/chathub");
```

---

## Blazor Server Integration

```csharp
@page "/chat"
@inject ChatClientAgent Agent

<h3>AI Chat</h3>

<div class="chat-container">
    @foreach (var message in messages)
    {
        <div class="message @message.Role">
            <strong>@message.Role:</strong> @message.Text
        </div>
    }
</div>

<input @bind="userInput" @onkeypress="HandleKeyPress" placeholder="Type a message..." />
<button @onclick="SendMessage">Send</button>

@code {
    private string userInput = "";
    private List<(string Role, string Text)> messages = new();

    private async Task SendMessage()
    {
        if (string.IsNullOrWhiteSpace(userInput)) return;

        messages.Add(("User", userInput));
        var input = userInput;
        userInput = "";

        var response = await Agent.RunAsync(input);
        messages.Add(("Agent", response.Text));
    }

    private async Task HandleKeyPress(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
            await SendMessage();
    }
}
```

---

## Background Service

```csharp
public class AgentBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<AgentBackgroundService> _logger;

    public AgentBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<AgentBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _serviceProvider.CreateScope();
            var agent = scope.ServiceProvider.GetRequiredService<ChatClientAgent>();

            try
            {
                // Process queued messages, scheduled tasks, etc.
                _logger.LogInformation("Processing agent tasks");
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in background service");
            }
        }
    }
}

// Registration
builder.Services.AddHostedService<AgentBackgroundService>();
```

---

## CORS Configuration

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("https://myapp.com")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

app.UseCors("AllowFrontend");
```

---

## Authentication Integration

```csharp
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SecureAgentController : ControllerBase
{
    private readonly ChatClientAgent _agent;

    public SecureAgentController(ChatClientAgent agent)
    {
        _agent = agent;
    }

    [HttpPost]
    public async Task<ActionResult> Chat([FromBody] ChatRequest request)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        // Could create user-specific agent or add context
        var response = await _agent.RunAsync(
            $"User {userId} asks: {request.Message}");
        
        return Ok(new { response = response.Text });
    }
}
```

---

## Complete Web API Project Structure

```
MyAgentApi/
├── Program.cs
├── appsettings.json
├── Controllers/
│   ├── ChatController.cs
│   └── HealthController.cs
├── Services/
│   ├── AgentService.cs
│   └── IAgentService.cs
├── Models/
│   ├── ChatRequest.cs
│   └── ChatResponse.cs
├── Hubs/
│   └── ChatHub.cs
└── Extensions/
    └── ServiceCollectionExtensions.cs
```

---

## Service Layer Pattern

```csharp
public interface IAgentService
{
    Task<string> GetResponseAsync(string message);
    IAsyncEnumerable<string> GetStreamingResponseAsync(string message);
}

public class AgentService : IAgentService
{
    private readonly ChatClientAgent _agent;

    public AgentService(ChatClientAgent agent)
    {
        _agent = agent;
    }

    public async Task<string> GetResponseAsync(string message)
    {
        var response = await _agent.RunAsync(message);
        return response.Text;
    }

    public async IAsyncEnumerable<string> GetStreamingResponseAsync(string message)
    {
        await foreach (var update in _agent.RunStreamingAsync(message))
        {
            yield return update.Text;
        }
    }
}

// Registration
builder.Services.AddScoped<IAgentService, AgentService>();
```

---

## Testing ASP.NET Core Integration

```csharp
public class ChatControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ChatControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((context, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string>
                {
                    ["GITHUB_TOKEN"] = "test-token"
                }!);
            });
        }).CreateClient();
    }

    [Fact]
    public async Task ChatEndpoint_ReturnsSuccess()
    {
        var request = new { message = "Hello" };
        var response = await _client.PostAsJsonAsync("/api/chat", request);
        
        response.EnsureSuccessStatusCode();
    }
}
```

---

## Best Practices Summary

1. **Use Dependency Injection** - Register as Singleton/Scoped appropriately
2. **Implement Health Checks** - Monitor agent availability
3. **Add Rate Limiting** - Protect against abuse
4. **Enable CORS Carefully** - Only allow trusted origins
5. **Log Comprehensively** - Track requests and errors
6. **Use User Secrets** - Never commit tokens
7. **Implement Retry Policies** - Handle transient failures
8. **Test Thoroughly** - Unit and integration tests

---

## Congratulations! 🎉

You've completed all 15 modules of the Semantic Kernel to Agent Framework migration guide!

### What's Next?

- Practice with [Hands-On Labs](../../labs/)
- Read the [Blog Post Series](../../blog-posts/)
- Explore [Real-World Examples](../12-Real-World-Migrations/)
- Join the community and contribute!

---

**Happy coding with Agent Framework and GitHub Models!** 🚀
