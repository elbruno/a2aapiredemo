# eShop Lite - Complete Solution

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4)](https://dotnet.microsoft.com/download/dotnet/10.0)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](../LICENSE)
[![C#](https://img.shields.io/badge/Language-C%23-239120)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![.NET Aspire](https://img.shields.io/badge/.NET-Aspire-512BD4)](https://learn.microsoft.com/en-us/dotnet/aspire/)

> **The complete, production-ready eShop Lite application with AI-powered agents for intelligent checkout experiences.**

This folder contains the **fully implemented agentic solution** demonstrating how .NET Aspire applications can be modernized with AI agents using Microsoft Extensions AI and Azure OpenAI.

---

## 📂 Repository Context

This is part of a **multi-stage demo repository** for agentic modernization sessions:

| Folder | Description |
|--------|-------------|
| [`/src-01-start`](../src-01-start) | Starting point - baseline app with TODOs for live coding |
| [`/src-02-multiagent`](../src-02-multiagent) | Multi-agent workflow with StockAgent and DiscountAgent |
| [`/src-03-dependency-injection`](../src-03-dependency-injection) | DI patterns and observability |
| **`/src-04-complete`** | **This folder** - Full reference implementation |

---

## 🏗️ Architecture Overview

```
┌─────────────────────────────────────────────────────────────────────────┐
│                           .NET Aspire AppHost                            │
│                      (Service Orchestration Layer)                       │
└─────────────────────────────────────────────────────────────────────────┘
                                    │
          ┌─────────────────────────┼─────────────────────────┐
          │                         │                         │
          ▼                         ▼                         ▼
┌─────────────────┐       ┌─────────────────┐       ┌─────────────────┐
│   Store (UI)    │       │   Products API  │       │   SQL Server    │
│ Blazor Server   │◄─────►│ ASP.NET Core    │◄─────►│   (Database)    │
│                 │       │ + AI Search     │       │                 │
└────────┬────────┘       └─────────────────┘       └─────────────────┘
         │
         │  Checkout Flow
         ▼
┌─────────────────────────────────────────────────────────────────────────┐
│                        AgentServices Library                             │
│  ┌─────────────────────────────────────────────────────────────────┐   │
│  │               AgentCheckoutOrchestrator                          │   │
│  │                  (Coordinates Workflow)                          │   │
│  └─────────────────────────┬───────────────────────────────────────┘   │
│                            │                                            │
│          ┌─────────────────┴─────────────────┐                         │
│          ▼                                   ▼                         │
│  ┌───────────────┐                  ┌───────────────┐                  │
│  │  StockAgent   │                  │ DiscountAgent │                  │
│  │ Validates     │                  │ Computes      │                  │
│  │ Availability  │                  │ Discounts     │                  │
│  └───────────────┘                  └───────────────┘                  │
│                                                                         │
└─────────────────────────────────────────────────────────────────────────┘
                                    │
                                    ▼
                           ┌───────────────┐
                           │ Azure OpenAI  │
                           │ (GPT Models)  │
                           └───────────────┘
```

---

## 🎯 Key Features

### AI-Powered Agents

| Agent | Responsibility | Implementation |
|-------|---------------|----------------|
| **StockAgent** | Validates product availability and generates friendly status messages | [`AgentServices/Stock/StockAgentService.cs`](AgentServices/Stock/StockAgentService.cs) |
| **DiscountAgent** | Computes membership-based discounts using AI | [`AgentServices/Discount/DiscountAgentService.cs`](AgentServices/Discount/DiscountAgentService.cs) |
| **AgentCheckoutOrchestrator** | Coordinates the multi-agent checkout workflow | [`AgentServices/Checkout/AgentCheckoutOrchestrator.cs`](AgentServices/Checkout/AgentCheckoutOrchestrator.cs) |

### Membership Tier Discounts

| Tier | Discount | Description |
|------|----------|-------------|
| **Gold** | 20% | Premium members get the best discount |
| **Silver** | 10% | Standard members get moderate savings |
| **Normal** | 0% | No discount for regular customers |

### Core Functionality

- 🛒 **eShop Lite Store** - Blazor Server front-end with product browsing, cart, and checkout
- 🔍 **AI-Powered Search** - Semantic search using Azure OpenAI and vector embeddings
- 🤖 **Agentic Checkout** - AI agents for stock validation and membership-based discounts
- 🚀 **.NET Aspire** - Cloud-native orchestration with service discovery
- 💾 **SQL Server** - Persistent data storage with Entity Framework Core
- 🧠 **Vector Database** - In-memory vector store for product embeddings

---

## 📁 Project Structure

```
src-04-complete/
├── eShopLite-Aspire-Modernization.slnx  # Solution file
│
├── eShopAppHost/             # .NET Aspire App Host (orchestration)
│   ├── Program.cs            # Service registration and startup
│   ├── azure.yaml            # Azure deployment configuration
│   └── infra/                # Infrastructure as code templates
│
├── eShopServiceDefaults/     # Shared service configuration
│
├── Products/                 # Products API with AI search capabilities
│   ├── Endpoints/            # REST API endpoints
│   │   └── ProductEndpoints.cs
│   ├── Memory/               # Vector database for AI search
│   │   └── MemoryContext.cs
│   ├── Models/               # EF Core context and initialization
│   │   └── Context.cs
│   └── Data/                 # Additional data context
│
├── Store/                    # Blazor Server front-end
│   ├── Components/           # Razor components
│   │   ├── Pages/            # Application pages
│   │   │   ├── CartPage.razor        # Shopping cart with agent integration
│   │   │   ├── CheckoutPage.razor    # Checkout flow
│   │   │   └── Products.razor        # Product catalog
│   │   ├── Cart/             # Shopping cart components
│   │   └── Layout/           # Layout components
│   └── Services/             # Business logic services
│       ├── CartService.cs
│       └── CheckoutService.cs
│
├── AgentServices/            # AI Agent services for checkout
│   ├── AgentServicesExtensions.cs    # DI registration
│   ├── Configuration/                 # Agent settings
│   │   └── AgentSettings.cs
│   ├── Checkout/                      # Checkout orchestration
│   │   ├── AgentCheckoutOrchestrator.cs
│   │   └── IAgentCheckoutOrchestrator.cs
│   ├── Discount/                      # Discount agent
│   │   ├── DiscountAgentService.cs
│   │   └── IDiscountAgentService.cs
│   ├── Stock/                         # Stock agent
│   │   ├── StockAgentService.cs
│   │   └── IStockAgentService.cs
│   └── Models/                        # Agent DTOs
│       ├── CheckoutModels.cs
│       ├── DiscountModels.cs
│       └── StockModels.cs
│
├── CartEntities/             # Cart and order models
├── DataEntities/             # Product and customer models
├── SearchEntities/           # AI search response models
├── VectorEntities/           # Vector embedding models
│
├── Products.Tests/           # Products API tests
└── Store.Tests/              # Store front-end tests
```

---

## 🚀 Quick Start

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) (Preview)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (for SQL Server container)
- Azure OpenAI or Microsoft Foundry connection string (for AI features)

### Running the Application

1. **Configure AI Connection**
   
   Set up User Secrets for the Store project:
   ```bash
   cd Store
   dotnet user-secrets init
   dotnet user-secrets set "ConnectionStrings:microsoftfoundry" "your-azure-openai-connection-string"
   ```

2. **Run with .NET Aspire**
   ```bash
   cd eShopAppHost
   dotnet run
   ```

3. **Open the Aspire Dashboard**
   
   The Aspire dashboard will open automatically. Click on the **Store** endpoint to access the e-commerce application.

### Demo Flow

1. **Select a customer** using the user picker in the top-right corner:
   - **Alice Johnson** - Gold member (20% discount)
   - **Bob Smith** - Silver member (10% discount)
   - **Carol/David** - Normal members (no discount)

2. **Add products** to your cart from the Products page

3. **Go to Cart** and click "Apply AI Discount" to see:
   - Agent steps execution (StockAgent → DiscountAgent)
   - Discount calculation based on membership tier
   - Updated totals with discount applied

4. **Proceed to Checkout** to complete the order with discounts applied

---

## 🔧 Key Code Locations

### Agent Implementations

- **Discount Agent Logic**: [`AgentServices/Discount/DiscountAgentService.cs`](AgentServices/Discount/DiscountAgentService.cs)
  - System prompt with discount rules
  - AI-powered discount calculation
  - Fallback deterministic logic

- **Stock Agent Logic**: [`AgentServices/Stock/StockAgentService.cs`](AgentServices/Stock/StockAgentService.cs)
  - Availability validation
  - AI-generated friendly messages

- **Multi-Agent Orchestrator**: [`AgentServices/Checkout/AgentCheckoutOrchestrator.cs`](AgentServices/Checkout/AgentCheckoutOrchestrator.cs)
  - Coordinates StockAgent → DiscountAgent workflow
  - Agent steps logging for UI display

### UI Integration

- **Cart Page with Agents**: [`Store/Components/Pages/CartPage.razor`](Store/Components/Pages/CartPage.razor)
  - "Apply AI Discount" button
  - Agent steps display
  - Discount amount and reason

- **Checkout Service**: [`Store/Services/CheckoutService.cs`](Store/Services/CheckoutService.cs)
  - Integration with AgentCheckoutOrchestrator

### DI Registration

- **Agent Services Registration**: [`AgentServices/AgentServicesExtensions.cs`](AgentServices/AgentServicesExtensions.cs)
  - Scoped lifetime for agents
  - Configuration binding

Look for `// DEMO:` comments throughout the codebase to find key demonstration points.

---

## 🧪 Running Tests

```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test Products.Tests/Products.Tests.csproj
dotnet test Store.Tests/Store.Tests.csproj
```

---

## ☁️ Azure Deployment

Deploy to Azure using Azure Developer CLI:

```bash
cd eShopAppHost
azd up
```

This will provision:
- Azure Container Apps for services
- Azure SQL Database
- Azure OpenAI with GPT models and embeddings
- Azure Application Insights for monitoring

For more details, see [eShopAppHost/next-steps.md](eShopAppHost/next-steps.md).

---

## 📖 Related Documentation

| Document | Description |
|----------|-------------|
| [Session Delivery Guide](../docs/03_session-delivery-guide.md) | Full session context and timeline |
| [Speaker Demo Walkthrough](../docs/04_speaker-demo-walkthrough.md) | Step-by-step code implementation guide |
| [Slide Content](../docs/05_slide-content-and-speaker-notes.md) | Presentation slides with speaker notes |

---

## 🛡️ Fallback Behavior

The application is designed for resilience:

- **AI Unavailable**: Falls back to deterministic discount logic
- **Connection Issues**: Displays user-friendly messages
- **Agent Errors**: Continues with standard mode operation

```csharp
// Example fallback pattern from DiscountAgentService.cs
if (_chatClient == null || !_settings.AgentsEnabled)
{
    _logger.LogWarning("DEMO: AI not available, using fallback discount logic");
    return ComputeFallbackDiscount(request);
}
```

---

## 📞 Support

- **Issues**: [GitHub Issues](https://github.com/elbruno/a2aapiredemo/issues)
- **Author**: [Bruno Capuano](https://github.com/elbruno)
- **Blog**: [elbruno.com](https://www.elbruno.com)

---

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](../LICENSE) file for details.

---

<p align="center">
  <strong>Built with ❤️ by <a href="https://www.elbruno.com">Bruno Capuano</a></strong>
</p>
