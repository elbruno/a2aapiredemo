using Chat.Hubs;
using Chat.Services;
using Search.Services;
using SearchEntities;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add Aspire service defaults
builder.AddServiceDefaults();

// Add services to the container
builder.Services.AddOpenApi();

// Configure HttpClient with resilience and service discovery
builder.Services.AddHttpClient<INlWebClient, MockNlWebClient>();

// Register NLWeb client
builder.Services.AddScoped<INlWebClient, MockNlWebClient>();

// Register Chat service
builder.Services.AddScoped<IChatService, ChatService>();

// Add SignalR
builder.Services.AddSignalR();

// Add CORS for SignalR and API
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowStore", policy =>
    {
        policy.WithOrigins("https://localhost:7147", "https://store") // Allow Store service
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials(); // Required for SignalR
    });
});

// Add rate limiting
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = System.Threading.RateLimiting.PartitionedRateLimiter.Create<HttpContext, string>(
        httpContext => System.Threading.RateLimiting.RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.User.Identity?.Name ?? httpContext.Request.Headers.Host.ToString(),
            factory: partition => new System.Threading.RateLimiting.FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 100,
                Window = TimeSpan.FromMinutes(1)
            }));
});

var app = builder.Build();

// Map Aspire default endpoints
app.MapDefaultEndpoints();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("AllowStore");
app.UseRateLimiter();

// Map SignalR hub
app.MapHub<ChatHub>("/chat-hub");

// Chat API endpoint
app.MapPost("/api/v1/chat/message", async (
    ChatMessage chatMessage,
    IChatService chatService,
    ILogger<Program> logger) =>
{
    var correlationId = Guid.NewGuid().ToString();
    logger.LogInformation("Chat message request: sessionId={SessionId}, correlationId={CorrelationId}", 
        chatMessage.SessionId, correlationId);

    try
    {
        // Input validation
        if (string.IsNullOrWhiteSpace(chatMessage.Message))
        {
            return Results.BadRequest(new SearchErrorResponse
            {
                Code = "INVALID_MESSAGE",
                Message = "Message content is required and cannot be empty",
                CorrelationId = correlationId
            });
        }

        if (chatMessage.Message.Length > 5000)
        {
            return Results.BadRequest(new SearchErrorResponse
            {
                Code = "MESSAGE_TOO_LONG",
                Message = "Message cannot exceed 5000 characters",
                CorrelationId = correlationId
            });
        }

        // Generate session ID if not provided
        if (string.IsNullOrEmpty(chatMessage.SessionId))
        {
            chatMessage.SessionId = Guid.NewGuid().ToString();
        }

        var response = await chatService.SendMessageAsync(chatMessage);
        
        logger.LogInformation("Chat message processed: sessionId={SessionId}, responseTime={ResponseTime}ms, correlationId={CorrelationId}", 
            response.SessionId, response.Metadata.ResponseTime, correlationId);

        return Results.Ok(response);
    }
    catch (OperationCanceledException)
    {
        logger.LogWarning("Chat message request cancelled: correlationId={CorrelationId}", correlationId);
        return Results.StatusCode(499);
    }
    catch (TimeoutException)
    {
        logger.LogError("Chat message request timeout: correlationId={CorrelationId}", correlationId);
        return Results.Problem(
            statusCode: 502,
            detail: "NLWeb service timeout",
            title: "NLWEB_TIMEOUT",
            instance: $"correlation:{correlationId}");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Chat message request failed: correlationId={CorrelationId}", correlationId);
        return Results.Problem(
            statusCode: 500,
            detail: "An internal error occurred",
            title: "INTERNAL_ERROR",
            instance: $"correlation:{correlationId}");
    }
})
.WithName("SendChatMessage")
.WithSummary("Send a chat message and get an AI response")
.WithOpenApi();

// Chat session endpoint
app.MapGet("/api/v1/chat/session/{sessionId}", async (
    string sessionId,
    IChatService chatService,
    ILogger<Program> logger) =>
{
    var correlationId = Guid.NewGuid().ToString();
    logger.LogInformation("Chat session request: sessionId={SessionId}, correlationId={CorrelationId}", 
        sessionId, correlationId);

    try
    {
        if (string.IsNullOrWhiteSpace(sessionId))
        {
            return Results.BadRequest(new SearchErrorResponse
            {
                Code = "INVALID_SESSION_ID",
                Message = "Session ID is required",
                CorrelationId = correlationId
            });
        }

        var session = await chatService.GetSessionAsync(sessionId);
        return Results.Ok(session);
    }
    catch (KeyNotFoundException)
    {
        return Results.NotFound(new SearchErrorResponse
        {
            Code = "SESSION_NOT_FOUND",
            Message = $"Chat session not found: {sessionId}",
            CorrelationId = correlationId
        });
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Chat session request failed: sessionId={SessionId}, correlationId={CorrelationId}", 
            sessionId, correlationId);
        return Results.Problem(
            statusCode: 500,
            detail: "An internal error occurred",
            title: "INTERNAL_ERROR",
            instance: $"correlation:{correlationId}");
    }
})
.WithName("GetChatSession")
.WithSummary("Get chat session history")
.WithOpenApi();

app.Run();

// Make Program class visible to tests
public partial class Program { }
