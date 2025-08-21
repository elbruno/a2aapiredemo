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

// Single Agent Demo API - Demonstrates single AI agent workflow
app.MapPost("/api/single-agent/analyze", (AnalysisRequest request) =>
{
    try
    {
        // For now, simulate AI analysis with a placeholder response
        // In a real implementation, this would call OpenAI/Azure OpenAI
        var analysis = $"[AI Analysis] The text '{request.Text}' contains {request.Text.Split(' ').Length} words and appears to be {(request.Text.Length > 100 ? "lengthy" : "concise")} in nature. Key themes detected: communication, context analysis.";
        
        return Results.Ok(new AnalysisResponse(
            Input: request.Text, 
            Analysis: analysis,
            Agent: "SingleAgent",
            Timestamp: DateTime.UtcNow
        ));
    }
    catch (Exception ex)
    {
        return Results.Problem($"Error during analysis: {ex.Message}");
    }
})
.WithName("AnalyzeWithSingleAgent")
.WithOpenApi()
.WithSummary("Analyze text using a single AI agent")
.WithDescription("Provides AI-powered text analysis using a single agent workflow");

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new { Status = "Healthy", Service = "SingleAgentDemo" }))
.WithName("HealthCheck")
.WithOpenApi();

app.Logger.LogInformation("SingleAgentDemo service started successfully");

app.Run();

// Request/Response models for the API
public record AnalysisRequest(string Text);
public record AnalysisResponse(string Input, string Analysis, string Agent, DateTime Timestamp);
