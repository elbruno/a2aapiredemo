# Service Communication Architecture Documentation

This document describes the service communication architecture for SingleAgentDemo and MultiAgentDemo projects.

## Overview

Both SingleAgentDemo and MultiAgentDemo follow a service layer pattern for external service communication, ensuring consistency, maintainability, and testability across the solution.

## SingleAgentDemo Service Communications

The SingleAgentDemo project orchestrates a single Semantic Kernel Agent that coordinates with multiple external services to provide AI-powered analysis and recommendations.

### Services Called by SingleAgentDemo

| Service | Endpoint | Purpose | Interface |
|---------|----------|---------|-----------|
| **AnalyzePhotoService** | `https+http://analyzephotoservice` | AI-powered image analysis and photo understanding | `IAnalyzePhotoService` |
| **CustomerInformationService** | `https+http://customerinformationservice` | Customer profile management and tool matching | `ICustomerInformationService` |
| **ToolReasoningService** | `https+http://toolreasoningservice` | AI-powered reasoning using Semantic Kernel | `IToolReasoningService` |
| **InventoryService** | `https+http://inventoryservice` | Product search and inventory management | `IInventoryService` |

### SingleAgentDemo API Endpoints

- `POST /api/single-agent/analyze` - Orchestrates all services through a single agent workflow

### Service Usage Pattern

```csharp
// Single agent orchestrates multiple service calls
var analysisResult = await _analyzePhotoService.AnalyzePhotoAsync(image, prompt);
var customerInfo = await _customerInformationService.GetCustomerProfileAsync(userId);
var reasoning = await _toolReasoningService.ReasonAboutToolsAsync(request);
var inventory = await _inventoryService.SearchProductsAsync(query);
```

## MultiAgentDemo Service Communications

The MultiAgentDemo project implements a multi-agent system where specialized agents handle different aspects of customer assistance.

### Services Called by MultiAgentDemo

| Service | Endpoint | Purpose | Interface |
|---------|----------|---------|-----------|
| **InventoryService** | `https+http://inventoryservice` | Product search and inventory management | `IInventoryAgentService` |
| **MatchmakingService** | `https+http://matchmakingservice` | Alternative product recommendations | `IMatchmakingAgentService` |
| **LocationService** | `https+http://locationservice` | Store location and product positioning | `ILocationAgentService` |
| **NavigationService** | `https+http://navigationservice` | In-store navigation instructions | `INavigationAgentService` |

### MultiAgentDemo API Endpoints

- `POST /api/multi-agent/assist` - Orchestrates multiple specialized agents

### Agent Workflow

1. **Inventory Agent** - Searches for requested products
2. **Matchmaking Agent** - Finds alternative and similar products
3. **Location Agent** - Determines where products are located in store
4. **Navigation Agent** - Provides step-by-step directions

### Service Usage Pattern

```csharp
// Multi-agent orchestration with specialized agents
var inventoryStep = await RunInventoryAgentAsync(request.ProductQuery);
var matchmakingStep = await RunMatchmakingAgentAsync(request.ProductQuery, request.UserId);
var locationStep = await RunLocationAgentAsync(request.ProductQuery);
var navigationStep = await RunNavigationAgentAsync(request.Location, request.ProductQuery);
```

## Service Layer Architecture

### Interface Pattern

All external service communications follow the same pattern established by `Store/ProductService.cs`:

```csharp
public interface IServiceName
{
    Task<ResultType> MethodNameAsync(RequestType request);
}

public class ServiceName : IServiceName
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ServiceName> _logger;

    // HttpClient-based implementation with error handling
    // Fallback mechanisms when services are unavailable
    // Comprehensive logging
}
```

### Dependency Injection Registration

Services are registered in `Program.cs` using typed HttpClient configuration:

```csharp
builder.Services.AddHttpClient<IServiceInterface, ServiceImplementation>(
    client => client.BaseAddress = new("https+http://servicename"));
```

### Error Handling

Each service implements:
- Try-catch blocks around HTTP calls
- Logging of successes and failures
- Fallback data when external services are unavailable
- Graceful degradation of functionality

## .NET Aspire Integration

All services are registered in the Aspire AppHost (`src/ZavaAppHost/Program.cs`) for:
- Service discovery
- Health checks
- Centralized logging
- Application Insights integration
- OpenAI configuration

### Aspire Service Registration

```csharp
var serviceName = builder.AddProject<Projects.ServiceName>("servicename")
    .WithExternalHttpEndpoints()
    .WithReference(openai)
    .WithEnvironment("AI_ChatDeploymentName", chatDeploymentName);
```

## Shared Entities

All services use entities from the `SharedEntities` project to ensure consistency:
- `InventorySearchResult`
- `MatchmakingResult`
- `LocationResult`
- `NavigationInstructions`
- `ProductInfo`
- `StoreLocation`
- And more...

## API Communication Patterns

### SingleAgentDemo Pattern
- Centralized orchestration through single agent
- Sequential service calls with reasoning
- Unified response combining all service results

### MultiAgentDemo Pattern
- Specialized agents for specific tasks
- Parallel execution where possible
- Coordinated multi-step workflow
- Rich step-by-step execution tracking

## Health Monitoring

Each service provides health endpoints:
- `/health` - Service health status
- Aspire dashboard integration
- Application Insights telemetry

## Development and Testing

### Local Development
- Services run on different ports via Aspire orchestration
- Service discovery handles endpoint resolution
- Fallback data ensures development without dependencies

### Testing Strategy
- Services can be easily mocked via interfaces
- Unit tests can focus on orchestration logic
- Integration tests can verify service communication

## Benefits of This Architecture

1. **Consistency** - All projects follow the same service communication pattern
2. **Maintainability** - Centralized HTTP communication logic
3. **Testability** - Easy to mock and unit test
4. **Observability** - Comprehensive logging and monitoring
5. **Resilience** - Fallback mechanisms for service failures
6. **Scalability** - Clear separation of concerns enables independent scaling

This architecture establishes the rule that whenever a project or Semantic Kernel Agent needs to call another service, it must use this service layer pattern for consistency and maintainability.