# Module 14: Production Deployment

**Duration**: 20 minutes  
**Level**: Advanced  

---

## Production Deployment Guide

Best practices for deploying Agent Framework applications with GitHub Models to production.

---

## Environment Configuration

### Development (User Secrets)

```bash
dotnet user-secrets set "GITHUB_TOKEN" "ghp_development_token"
```

### Production (Environment Variables)

```bash
# Azure App Service
az webapp config appsettings set --name myapp --resource-group mygroup \
  --settings GITHUB_TOKEN="ghp_prod_token"

# Kubernetes
kubectl create secret generic agent-secrets \
  --from-literal=GITHUB_TOKEN="ghp_prod_token"

# Docker
docker run -e GITHUB_TOKEN="ghp_prod_token" myapp
```

---

## Secure Token Management

### Azure Key Vault Integration

```csharp
var builder = WebApplication.CreateBuilder(args);

// Add Azure Key Vault
if (builder.Environment.IsProduction())
{
    var keyVaultEndpoint = builder.Configuration["KeyVault:Endpoint"];
    builder.Configuration.AddAzureKeyVault(
        new Uri(keyVaultEndpoint!),
        new DefaultAzureCredential());
}

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
```

---

## Error Handling and Resilience

### Retry Policy with Polly

```csharp
builder.Services.AddSingleton<ChatClientAgent>(sp =>
{
    var client = sp.GetRequiredService<IChatClient>();
    
    // Wrap client with retry policy
    var retryPolicy = Policy
        .Handle<Exception>()
        .WaitAndRetryAsync(3, retryAttempt => 
            TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

    return new ChatClientAgent(client, new()
    {
        Name = "ProductionAgent",
        Instructions = "You are a production assistant.",
        Temperature = 0.7f
    });
});
```

---

## Logging and Monitoring

### Application Insights Integration

```csharp
builder.Services.AddApplicationInsightsTelemetry();

builder.Services.AddSingleton<IChatClient>(sp =>
{
    var logger = sp.GetRequiredService<ILogger<Program>>();
    var config = sp.GetRequiredService<IConfiguration>();
    
    logger.LogInformation("Initializing ChatClient with GitHub Models");
    
    var githubToken = config["GITHUB_TOKEN"] 
        ?? throw new InvalidOperationException("GITHUB_TOKEN not configured");

    return new ChatClient(
        "gpt-4o-mini",
        new ApiKeyCredential(githubToken),
        new OpenAIClientOptions { 
            Endpoint = new Uri("https://models.github.ai/inference") 
        }).AsIChatClient();
});

// Controller with logging
[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly ChatClientAgent _agent;
    private readonly ILogger<ChatController> _logger;

    public ChatController(ChatClientAgent agent, ILogger<ChatController> logger)
    {
        _agent = agent;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Chat([FromBody] ChatRequest request)
    {
        _logger.LogInformation("Received chat request: {Message}", request.Message);
        
        try
        {
            var response = await _agent.RunAsync(request.Message);
            _logger.LogInformation("Chat response generated successfully");
            return Ok(new { response = response.Text });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing chat request");
            return StatusCode(500, "An error occurred processing your request");
        }
    }
}
```

---

## Health Checks

```csharp
builder.Services.AddHealthChecks()
    .AddCheck("agent_ready", () =>
    {
        try
        {
            var client = builder.Services.BuildServiceProvider()
                .GetRequiredService<IChatClient>();
            return HealthCheckResult.Healthy("Agent client is ready");
        }
        catch
        {
            return HealthCheckResult.Unhealthy("Agent client not initialized");
        }
    });

app.MapHealthChecks("/health");
```

---

## Rate Limiting

```csharp
using Microsoft.AspNetCore.RateLimiting;

builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.User.Identity?.Name ?? context.Request.Headers.Host.ToString(),
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 10,
                QueueLimit = 0,
                Window = TimeSpan.FromMinutes(1)
            }));
});

app.UseRateLimiter();
```

---

## Dockerfile

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["MyApp.csproj", "./"]
RUN dotnet restore
COPY . .
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Set environment
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "MyApp.dll"]
```

---

## Kubernetes Deployment

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: agent-app
spec:
  replicas: 3
  selector:
    matchLabels:
      app: agent-app
  template:
    metadata:
      labels:
        app: agent-app
    spec:
      containers:
      - name: agent-app
        image: myregistry/agent-app:latest
        ports:
        - containerPort: 8080
        env:
        - name: GITHUB_TOKEN
          valueFrom:
            secretKeyRef:
              name: agent-secrets
              key: GITHUB_TOKEN
        resources:
          requests:
            memory: "256Mi"
            cpu: "250m"
          limits:
            memory: "512Mi"
            cpu: "500m"
        livenessProbe:
          httpGet:
            path: /health
            port: 8080
          initialDelaySeconds: 30
          periodSeconds: 10
```

---

## Production Checklist

- [ ] Secrets in Key Vault/Secrets Manager
- [ ] HTTPS enabled
- [ ] Rate limiting configured
- [ ] Logging and monitoring setup
- [ ] Health checks implemented
- [ ] Error handling comprehensive
- [ ] Resource limits set
- [ ] Backup and disaster recovery plan
- [ ] Security headers configured
- [ ] CORS properly configured

---

## Monitoring Queries (Application Insights)

```kusto
// Track agent response times
requests
| where name == "POST /api/chat"
| summarize avg(duration), count() by bin(timestamp, 1h)

// Track errors
exceptions
| where outerMessage contains "agent"
| summarize count() by outerMessage

// Token usage tracking
customMetrics
| where name == "TokensUsed"
| summarize sum(value) by bin(timestamp, 1d)
```

---

## Next: [Module 15: ASP.NET Core Integration](../15-ASPNetCore-Integration/)
