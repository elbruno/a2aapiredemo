# eShop Lite - .NET Aspire Demo with AI-Powered Search

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4)](https://dotnet.microsoft.com/download/dotnet/9.0)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)
[![C#](https://img.shields.io/badge/Language-C%23-239120)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![.NET Aspire](https://img.shields.io/badge/.NET-Aspire-512BD4)](https://learn.microsoft.com/en-us/dotnet/aspire/)

> **A sample eShop application built with .NET Aspire, featuring AI-powered product search using Azure OpenAI and vector embeddings**

This repository demonstrates a modern cloud-native e-commerce application using .NET Aspire for orchestration, Blazor for the front-end, and AI capabilities for intelligent product search.

---

## 📂 Repository Structure - Two-Stage Conference Demo

This repository is structured for a **30-minute agentic modernization live session** with two parallel folders:

### 🏁 `/src-01-start` - Starting Point

A **minimal, fully building** version of the app **without agents implemented**:
- eShop Lite baseline features (products, cart, checkout)
- AI-powered vector search
- Working checkout flow (no discounts, no agent integration)
- Membership tier selector (UI present, but non-functional)
- **Agent logic replaced with clear TODO instructions** for live coding

### ✅ `/src-05-complete` - Complete Solution

The **fully implemented agentic solution**:
- DiscountAgent (AI-powered membership discounts)
- StockAgent (AI-generated stock availability messages)
- Multi-agent checkout orchestrator
- UI updates showing agent steps and discounts
- Full Azure AI Foundry workflow support

### 🔧 `/src-04-devui` - Agent Framework with DevUI

An advanced scenario demonstrating:
- Agent Framework registration approach
- DevUI package integration for agent debugging
- Visual interface for inspecting agent behavior

**Both folders build and run cleanly using .NET Aspire.**

---

## 📚 Documentation for Speakers

This repository includes comprehensive documentation for presenters delivering this demo:

| Document | Description |
|----------|-------------|
| [01_prep_for_event.md](docs/01_prep_for_event.md) | GitHub Copilot agent prompt for preparing the repository |
| [02_copilot-session-setup-prompt.md](docs/02_copilot-session-setup-prompt.md) | Copilot session setup and configuration |
| [03_speaker-demo-walkthrough.md](docs/03_speaker-demo-walkthrough.md) | **Step-by-step code implementation guide for live demos** |
| [session-delivery-guide.md](docs/session-delivery-guide.md) | Full session context, timeline, and delivery tips |

### 📖 Speaker Quick Start

1. **Before the event**: Review [session-delivery-guide.md](docs/session-delivery-guide.md) for the complete session narrative
2. **During the demo**: Follow [03_speaker-demo-walkthrough.md](docs/03_speaker-demo-walkthrough.md) for step-by-step code changes
3. **Reference**: Keep `/src-05-complete` open as a fallback reference

---

## 🎯 Features

- 🛒 **eShop Lite Store** - Blazor Server front-end with product browsing, cart, and checkout
- 🔍 **AI-Powered Search** - Semantic search using Azure OpenAI and vector embeddings
- 🤖 **Agentic Checkout** - AI agents for stock validation and membership-based discounts
- 🚀 **.NET Aspire** - Cloud-native orchestration with service discovery
- 💾 **SQL Server** - Persistent data storage with Entity Framework Core
- 🧠 **Vector Database** - In-memory vector store for product embeddings
- 👥 **Customer Management** - Customer profiles with membership tiers (Gold/Silver/Normal)

---

## 🚀 Quick Start

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) (9.0.x or later)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (for SQL Server container)
- IDE: [Visual Studio 2022](https://visualstudio.microsoft.com/) (17.12+), [VS Code](https://code.visualstudio.com/) with C# Dev Kit, or [JetBrains Rider](https://www.jetbrains.com/rider/)
- Azure OpenAI or Microsoft Foundry connection string (for AI features)

### Running the Application

1. **Clone the repository**
   ```bash
   git clone https://github.com/elbruno/a2aapiredemo.git
   cd a2aapiredemo
   ```

2. **Verify .NET 9 SDK**
   ```bash
   dotnet --version
   # Should output 9.0.x
   ```

3. **Choose your starting point**
   - For the **complete solution** with agents: `cd src-05-complete`
   - For the **demo starting point** (no agents): `cd src-01-start`

4. **Configure AI Connection (for AI-powered search)**
   
   Set up User Secrets for the Products project:
   ```bash
   cd Products
   dotnet user-secrets init
   dotnet user-secrets set "ConnectionStrings:microsoftfoundry" "your-azure-openai-connection-string"
   ```

5. **Run with .NET Aspire**
   ```bash
   cd eShopAppHost
   dotnet run
   ```

6. **Open the Aspire Dashboard**
   
   The Aspire dashboard will open automatically, showing all running services. Click on the **Store** endpoint to access the e-commerce application.

---

## 📁 Project Structure

Both `/src-01-start` and `/src-05-complete` contain the same project structure:

```
src-01-start/ or src-05-complete/
├── eShopLite-Aspire-Modernization.slnx  # Solution file
│
├── eShopAppHost/             # .NET Aspire App Host (orchestration)
├── eShopServiceDefaults/     # Shared service configuration
│
├── Products/                 # Products API with AI search capabilities
│   ├── Endpoints/            # REST API endpoints
│   ├── Memory/               # Vector database for AI search
│   ├── Models/               # EF Core context and initialization
│   └── Data/                 # Additional data context
│
├── Store/                    # Blazor Server front-end
│   ├── Components/           # Razor components
│   │   ├── Pages/            # Application pages
│   │   ├── Cart/             # Shopping cart components
│   │   └── Layout/           # Layout components
│   └── Services/             # Business logic services
│
├── AgentServices/            # AI Agent services for checkout
│   ├── Checkout/             # Checkout orchestration
│   ├── Discount/             # Discount agent
│   ├── Stock/                # Stock agent
│   └── Models/               # Agent DTOs
│
├── CartEntities/             # Cart and order models
├── DataEntities/             # Product and customer models
├── SearchEntities/           # AI search response models
├── VectorEntities/           # Vector embedding models
│
├── Products.Tests/           # Products API tests
└── Store.Tests/              # Store front-end tests
```

### Key Differences Between Folders

| File | src-01-start | src-05-complete |
|------|-----------|--------------|
| `AgentServices/Discount/DiscountAgentService.cs` | TODO stub | Full AI implementation |
| `AgentServices/Stock/StockAgentService.cs` | TODO stub | Full AI implementation |
| `AgentServices/Checkout/AgentCheckoutOrchestrator.cs` | TODO stub | Full orchestration |

---

## 🔧 Architecture

### Services

| Service | Description | Technology |
|---------|-------------|------------|
| **eShopAppHost** | .NET Aspire orchestrator | .NET Aspire |
| **Products** | Product catalog API with AI search | ASP.NET Core Minimal API |
| **Store** | E-commerce front-end | Blazor Server |
| **AgentServices** | AI agents for checkout | Microsoft.Extensions.AI |
| **SQL Server** | Product database | SQL Server (containerized) |

### AI Features

The application includes AI-powered capabilities:

- **AI-Powered Search**:
  - **Azure OpenAI GPT-5-mini** - For generating friendly search responses
  - **Text Embedding 3 Small** - For creating product vector embeddings
  - **In-Memory Vector Store** - For semantic similarity search

- **Agentic Checkout**:
  - **StockAgent** - Validates product availability with AI-generated messages
  - **DiscountAgent** - Computes membership-based discounts using AI
  - **Checkout Orchestrator** - Coordinates multi-agent workflow

### API Endpoints

| Endpoint | Method | Description |
|----------|--------|-------------|
| `/api/Product/` | GET | Get all products |
| `/api/Product/{id}` | GET | Get product by ID |
| `/api/Product/{id}` | PUT | Update product |
| `/api/Product/` | POST | Create product |
| `/api/Product/{id}` | DELETE | Delete product |
| `/api/Product/search/{search}` | GET | Text search products |
| `/api/aisearch/{search}` | GET | AI-powered semantic search |

---

## 🧪 Running Tests

```bash
# Run all tests for src-05-complete
cd src-05-complete
dotnet test

# Run all tests for src-01-start
cd src-01-start
dotnet test

# Run specific test project
dotnet test Products.Tests/Products.Tests.csproj
dotnet test Store.Tests/Store.Tests.csproj
```

---

## ☁️ Azure Deployment

The application supports deployment to Azure using Azure Developer CLI (azd):

```bash
cd src-05-complete/eShopAppHost
azd up
```

This will provision:
- Azure Container Apps for services
- Azure SQL Database
- Azure OpenAI with GPT-5-mini and text-embedding-3-small deployments
- Azure Application Insights for monitoring

For more details, see [src-05-complete/eShopAppHost/next-steps.md](src-05-complete/eShopAppHost/next-steps.md).

---

## 📖 Key Technologies

- **.NET 9** - Latest .NET framework
- **.NET Aspire** - Cloud-native app orchestration
- **Blazor Server** - Interactive web UI
- **Entity Framework Core** - Data access
- **Microsoft Agent Framework** - Building AI agents with `CreateAIAgent` and `RunAsync` patterns
- **Microsoft.Extensions.AI** - AI abstractions (underlying infrastructure)
- **Azure OpenAI** - AI models for search
- **SQL Server** - Database storage
- **Docker** - Containerization

---

## 🤝 Contributing

Contributions are welcome! Please feel free to submit issues and pull requests.

---

## 🧠 Agentic Checkout Demo (Microsoft Agent Framework)

This demo showcases an **AI-powered checkout experience** using the **Microsoft Agent Framework** for building intelligent agents for stock validation and membership-based discounts.

### Microsoft Agent Framework Integration

The demo uses the `Microsoft.Agents.AI.OpenAI` package to create AIAgents with the modern pattern:

```csharp
// Create an AIAgent with instructions
var discountAgent = _chatClient.CreateAIAgent(
    instructions: AgentInstructions,
    name: "DiscountAgent");

// Run the agent
var response = await discountAgent.RunAsync(userMessage);
var content = response.Text;
```

This pattern replaces the older ChatClient direct calls with a higher-level Agent abstraction that:
- Encapsulates system prompts as agent instructions
- Provides named agents for better logging and debugging
- Returns structured `AgentRunResponse` with convenient `.Text` property
- Supports conversation context management

### What the Agents Do

| Agent | Responsibility |
|-------|---------------|
| **StockAgent** | Validates product availability and generates friendly stock status messages |
| **DiscountAgent** | Computes membership-based discounts (Gold: 20%, Silver: 10%, Normal: 0%) using AI |
| **AgentCheckoutOrchestrator** | Coordinates the multi-agent checkout workflow |

### Demo Flow

1. **Start the application** using .NET Aspire (`dotnet run` from `eShopAppHost`)
2. **Open the Store** front-end from the Aspire dashboard
3. **Select a customer** using the user picker in the top-right corner:
   - **Alice Johnson** - Gold member (20% discount)
   - **Bob Smith** - Silver member (10% discount)
   - **Carol/David** - Normal members (no discount)
4. **Add products** to your cart from the Products page
5. **Go to Cart** and click "Apply AI Discount" to see:
   - Agent steps execution (StockAgent → DiscountAgent)
   - Discount calculation based on membership tier
   - Updated totals with discount applied
6. **Proceed to Checkout** to complete the order with discounts applied

### Configuration

The agentic checkout uses the same Azure OpenAI connection as the AI search feature:

```bash
# Set up User Secrets for the Store project
cd src-05-complete/Store
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:microsoftfoundry" "your-azure-openai-connection-string"
```

The connection string format should be:
```
Endpoint=https://your-resource.openai.azure.com;Key=your-api-key
```

### Fallback Mode

If Azure OpenAI is not configured or unavailable:
- The application still works in **standard mode**
- Discounts are computed using deterministic fallback logic
- UI displays a message indicating "standard mode"

### Project Structure for Agents

```
src-05-complete/AgentServices/     # Full implementation
src-01-start/AgentServices/        # TODO stubs for live coding
├── Configuration/              # Agent settings
│   └── AgentSettings.cs
├── Checkout/                   # Checkout orchestration
│   ├── AgentCheckoutOrchestrator.cs
│   └── IAgentCheckoutOrchestrator.cs
├── Discount/                   # Discount agent
│   ├── DiscountAgentService.cs
│   └── IDiscountAgentService.cs
├── Stock/                      # Stock agent
│   ├── StockAgentService.cs
│   └── IStockAgentService.cs
└── Models/                     # Agent DTOs
    ├── CheckoutModels.cs
    ├── DiscountModels.cs
    └── StockModels.cs
```

### Key Code Locations for Demo

**In `src-05-complete`** (reference implementation):
- **Agent orchestration**: `AgentServices/Checkout/AgentCheckoutOrchestrator.cs`
- **Discount AI logic**: `AgentServices/Discount/DiscountAgentService.cs`
- **Cart UI with agents**: `Store/Components/Pages/CartPage.razor`
- **Checkout integration**: `Store/Services/CheckoutService.cs`

**In `src-01-start`** (TODOs for live coding):
- Each agent file contains detailed TODO instructions and comments
- Look for `// TODO:` comments to find where to implement agent logic

Look for `// DEMO:` comments in `src-05-complete` to find key demo points.

### 📖 Live Coding Guide

For **step-by-step instructions** on implementing the agents during your live demo, see:

👉 **[docs/03_speaker-demo-walkthrough.md](docs/03_speaker-demo-walkthrough.md)**

This guide includes:
- Exact code snippets to type/paste
- Expected outcomes after each step
- Timing estimates (~12 minutes total coding time)
- Troubleshooting tips
- Key messages to communicate to the audience

---

## 📞 Support and Community

- **Issues**: [GitHub Issues](https://github.com/elbruno/a2aapiredemo/issues)
- **Discussions**: [GitHub Discussions](https://github.com/elbruno/a2aapiredemo/discussions)
- **Blog**: [elbruno.com](https://www.elbruno.com)
- **Author**: [Bruno Capuano](https://github.com/elbruno)

---

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

<p align="center">
  <strong>Built with ❤️ by <a href="https://www.elbruno.com">Bruno Capuano</a></strong>
</p>
