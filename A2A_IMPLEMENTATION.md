# A2A eShopLite Demo - Implementation Documentation

## Overview

This project demonstrates a practical Agent2Agent (A2A) scenario using the eShopLite sample as a foundation. The implementation showcases how multiple autonomous agents (microservices) can collaborate to fulfill complex business requests through the Microsoft Semantic Kernel Agents framework and A2A SDK.

## Architecture

### Components

1. **Store (Frontend)** - Blazor web application providing the user interface
2. **Products API** - Main backend API that orchestrates agent communication using Semantic Kernel Agents
3. **Three Autonomous Agents:**
   - **Inventory Agent** - Provides real-time stock levels via Semantic Kernel Agent functions
   - **Promotions Agent** - Supplies active promotions and discounts via Semantic Kernel Agent functions  
   - **Researcher Agent** - Delivers product insights and reviews via Semantic Kernel Agent functions

### A2A Orchestration Flow

When a user selects "A2A Search" and submits a query:

1. **Products API** receives the search request via `/api/a2asearch/{search}`
2. **Products API** identifies relevant products using standard search
3. For each product, **Products API** orchestrates calls using Semantic Kernel Agents:
   - **Inventory Agent** function (`check_inventory`) - Gets stock levels
   - **Promotions Agent** function (`get_promotions`) - Gets active promotions
   - **Researcher Agent** function (`get_insights`) - Gets product insights
4. **Products API** aggregates all responses into a unified result
5. **Store Frontend** displays enriched product information

## Implementation Details

### Semantic Kernel Agents Framework

The implementation uses Microsoft.SemanticKernel.Agents.Core package for agent management:

#### Inventory Agent
```csharp
[KernelFunction("check_inventory")]
[Description("Check inventory levels for a product")]
public async Task<InventoryResponse?> CheckInventoryAsync(
    [Description("The product ID to check inventory for")] string productId)
```

#### Promotions Agent  
```csharp
[KernelFunction("get_promotions")]
[Description("Get active promotions for a product")]
public async Task<PromotionsResponse?> GetPromotionsAsync(
    [Description("The product ID to get promotions for")] string productId)
```

#### Researcher Agent
```csharp
[KernelFunction("get_insights")]
[Description("Get product insights and reviews")]
public async Task<ResearchResponse?> GetInsightsAsync(
    [Description("The product ID to get insights for")] string productId)
```

### A2A Orchestration Service

The `A2AOrchestrationService` has been updated to use the Semantic Kernel Agents framework:

```csharp
public class A2AOrchestrationService : IA2AOrchestrationService
{
    private readonly InventoryAgent _inventoryAgent;
    private readonly PromotionsAgent _promotionsAgent;
    private readonly ResearcherAgent _researcherAgent;
    
    // Orchestrates agent calls using Semantic Kernel framework
    public async Task<A2ASearchResponse> ExecuteA2ASearchAsync(string searchTerm)
    {
        // Parallel agent function calls
        var inventoryTask = _inventoryAgent.CheckInventoryAsync(product.Id.ToString());
        var promotionsTask = _promotionsAgent.GetPromotionsAsync(product.Id.ToString());
        var insightsTask = _researcherAgent.GetInsightsAsync(product.Id.ToString());
        
        await Task.WhenAll(inventoryTask, promotionsTask, insightsTask);
        // ... result aggregation
    }
}
```

### NuGet Packages

The implementation leverages the following key package:

- **Microsoft.SemanticKernel.Agents.Core** (v1.61.0) - Provides the Semantic Kernel Agents framework for A2A communication

### Service Registration

```csharp
// Configure HttpClients for agents (still needed for agent implementations)
builder.Services.AddHttpClient("InventoryAgent", client =>
{
    client.BaseAddress = new Uri("http://inventory-agent");
});

// Add A2A Agents using Semantic Kernel framework
builder.Services.AddScoped<InventoryAgent>();
builder.Services.AddScoped<PromotionsAgent>();
builder.Services.AddScoped<ResearcherAgent>();

// Add A2A Orchestration Service
builder.Services.AddScoped<IA2AOrchestrationService, A2AOrchestrationService>();
```

### Agent Endpoints

The agents still expose HTTP endpoints for external communication but are wrapped by Semantic Kernel Agent functions:

#### Inventory Agent
- **HTTP Endpoint:** `POST /api/inventory/check`
- **Agent Function:** `check_inventory`
- **Request:** `{ "productId": "string" }`
- **Response:** `{ "productId": "string", "stock": 42 }`

#### Promotions Agent
- **HTTP Endpoint:** `POST /api/promotions/active`
- **Agent Function:** `get_promotions`
- **Request:** `{ "productId": "string" }`
- **Response:** `{ "productId": "string", "promotions": [{ "title": "string", "discount": 10 }] }`

#### Researcher Agent
- **HTTP Endpoint:** `POST /api/researcher/insights`
- **Agent Function:** `get_insights`
- **Request:** `{ "productId": "string" }`
- **Response:** `{ "productId": "string", "insights": [{ "review": "string", "rating": 4.5 }] }`

### Frontend Features

The Store application includes a search page with:
- **Search Type Selector**: Dropdown with options:
  - Standard Search
  - Semantic Search  
  - A2A Search (Agent-to-Agent)
- **Enhanced Results Display**: Shows enriched data from all agents for A2A searches

### Data Models

#### A2A Enriched Product Response
```json
{
  "response": "Found X products enriched with inventory, promotions, and insights data.",
  "products": [
    {
      "productId": "1",
      "name": "Product Name",
      "description": "Product Description",
      "price": 99.99,
      "imageUrl": "image.jpg",
      "stock": 42,
      "promotions": [
        {
          "title": "Special Offer",
          "discount": 15
        }
      ],
      "insights": [
        {
          "review": "Great product!",
          "rating": 4.5
        }
      ]
    }
  ]
}
```

## Project Structure

```
src/
├── eShopAppHost/              # Aspire orchestration host
├── eShopServiceDefaults/      # Shared Aspire service configurations
├── Store/                     # Blazor frontend application
├── Products/                  # Main Products API with A2A orchestration
│   ├── Services/             # A2A orchestration service
│   │   └── Agents/           # Semantic Kernel Agent implementations
│   └── Endpoints/            # API endpoints including A2ASearch
├── InventoryAgent/           # Inventory management agent
├── PromotionsAgent/          # Promotions management agent
├── ResearcherAgent/          # Product research agent
├── DataEntities/             # Shared data models
├── SearchEntities/           # Search and A2A response models
└── Products.Tests/           # Unit tests including A2A orchestration tests
```

## Key Features Implemented

- ✅ Three autonomous agent projects with dedicated APIs
- ✅ **Semantic Kernel Agents framework integration** for A2A orchestration
- ✅ **KernelFunction-decorated agent methods** for structured agent communication
- ✅ **Microsoft.SemanticKernel.Agents.Core** package integration
- ✅ Enhanced frontend with search type selection
- ✅ Aspire integration for service orchestration
- ✅ Unit tests for A2A orchestration functionality with agent mocking
- ✅ Error handling and graceful degradation
- ✅ Parallel agent calls for optimal performance

## Testing

The implementation includes comprehensive unit tests updated for Semantic Kernel Agents:
- A2A orchestration service tests with mocked agent responses using the Semantic Kernel framework
- Agent dependency injection and testing with proper logger mocking
- Existing product API tests continue to pass
- All tests validate proper error handling and data aggregation through agent functions

Example test setup:
```csharp
var inventoryAgent = new InventoryAgent(httpClientFactory, _inventoryLogger);
var promotionsAgent = new PromotionsAgent(httpClientFactory, _promotionsLogger);
var researchAgent = new ResearcherAgent(httpClientFactory, _researchLogger);

var orchestrationService = new A2AOrchestrationService(
    context, inventoryAgent, promotionsAgent, researchAgent, _logger);
```

## Usage

1. Start the Aspire AppHost: `dotnet run` in `eShopAppHost` directory
2. Navigate to the Store application
3. Go to the Search page
4. Select "A2A Search (Agent-to-Agent)" from the dropdown
5. Enter a search term and click Search
6. View enriched results with inventory, promotions, and insights data

## Performance Considerations

- Agent calls are made in parallel to minimize response time
- Graceful handling of agent failures (partial results returned)
- Response caching can be added for production scenarios
- Circuit breaker patterns can be implemented for resilience

## Future Enhancements

- Dynamic agent discovery and registration using Semantic Kernel
- Advanced failure handling and retry policies within agent functions
- Real-time agent health monitoring through the Semantic Kernel framework
- Enhanced A2A SDK integration for standardized agent communication
- Agent authentication and authorization through Semantic Kernel security features
- Advanced agent orchestration patterns using Semantic Kernel's planning capabilities