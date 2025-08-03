# A2A eShopLite Demo - Implementation Documentation

## Overview

This project demonstrates a practical Agent2Agent (A2A) scenario using the eShopLite sample as a foundation. The implementation showcases how multiple autonomous agents (microservices) can collaborate to fulfill complex business requests through the A2A SDK.

## Architecture

### Components

1. **Store (Frontend)** - Blazor web application providing the user interface
2. **Products API** - Main backend API that orchestrates agent communication
3. **Three Autonomous Agents:**
   - **Inventory Agent** - Provides real-time stock levels
   - **Promotions Agent** - Supplies active promotions and discounts  
   - **Researcher Agent** - Delivers product insights and reviews

### A2A Orchestration Flow

When a user selects "A2A Search" and submits a query:

1. **Products API** receives the search request via `/api/a2asearch/{search}`
2. **Products API** identifies relevant products using standard search
3. For each product, **Products API** orchestrates parallel calls to:
   - **Inventory Agent** (`/api/inventory/check`) - Gets stock levels
   - **Promotions Agent** (`/api/promotions/active`) - Gets active promotions
   - **Researcher Agent** (`/api/researcher/insights`) - Gets product insights
4. **Products API** aggregates all responses into a unified result
5. **Store Frontend** displays enriched product information

## Implementation Details

### Agent Endpoints

#### Inventory Agent
- **Endpoint:** `POST /api/inventory/check`
- **Request:** `{ "productId": "string" }`
- **Response:** `{ "productId": "string", "stock": 42 }`

#### Promotions Agent
- **Endpoint:** `POST /api/promotions/active`
- **Request:** `{ "productId": "string" }`
- **Response:** `{ "productId": "string", "promotions": [{ "title": "string", "discount": 10 }] }`

#### Researcher Agent
- **Endpoint:** `POST /api/researcher/insights`
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
- ✅ A2A orchestration service for coordinating agent communication
- ✅ Enhanced frontend with search type selection
- ✅ Aspire integration for service orchestration
- ✅ Unit tests for A2A orchestration functionality
- ✅ Error handling and graceful degradation
- ✅ Parallel agent calls for optimal performance

## Testing

The implementation includes comprehensive unit tests:
- A2A orchestration service tests with mocked agent responses
- Existing product API tests continue to pass
- All tests validate proper error handling and data aggregation

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

- Dynamic agent discovery and registration
- Advanced failure handling and retry policies
- Real-time agent health monitoring
- A2A SDK integration for standardized agent communication
- Agent authentication and authorization