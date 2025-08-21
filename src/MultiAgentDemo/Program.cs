var builder = WebApplication.CreateBuilder(args);

// Add Aspire service defaults for service discovery, logging, health checks, telemetry
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Map Aspire default endpoints for health checks and telemetry
app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Multi-Agent Demo API - Demonstrates multi-agent AI workflow
app.MapPost("/api/multi-agent/assist", (AssistanceRequest request) =>
{
    try
    {
        // Simulate multi-agent workflow with different AI personas
        // In a real implementation, this would call OpenAI/Azure OpenAI with different prompts
        var analysis = $"[Analyzer Agent] Query '{request.Query}' requires complex processing involving {request.Query.Split(' ').Length} key terms.";
        var planning = $"[Planner Agent] Recommended approach: 1) Parse requirements 2) Identify resources 3) Execute step-by-step 4) Validate results.";
        var execution = $"[Executor Agent] Actionable steps: Start with '{request.Query.Split(' ').FirstOrDefault()}', proceed systematically, monitor progress, adjust as needed.";
        
        return Results.Ok(new AssistanceResponse(
            Query: request.Query,
            Analysis: analysis,
            Plan: planning,
            ExecutionSteps: execution,
            Agents: new[] { "Analyzer", "Planner", "Executor" },
            Timestamp: DateTime.UtcNow
        ));
    }
    catch (Exception ex)
    {
        return Results.Problem($"Error during multi-agent assistance: {ex.Message}");
    }
})
.WithName("GetMultiAgentAssistance")
.WithOpenApi()
.WithSummary("Get assistance using multiple AI agents")
.WithDescription("Provides AI-powered assistance using a multi-agent workflow (analyzer, planner, executor)");

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new { Status = "Healthy", Service = "MultiAgentDemo" }))
.WithName("HealthCheck")
.WithOpenApi();

app.Logger.LogInformation("MultiAgentDemo service started successfully");

app.Run();

// Request/Response models for the API
public record AssistanceRequest(string Query);
public record AssistanceResponse(string Query, string Analysis, string Plan, string ExecutionSteps, string[] Agents, DateTime Timestamp);
