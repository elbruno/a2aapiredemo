# eShop Lite - .NET Aspire Demo with AI-Powered Search

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4)](https://dotnet.microsoft.com/download/dotnet/9.0)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)
[![C#](https://img.shields.io/badge/Language-C%23-239120)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![.NET Aspire](https://img.shields.io/badge/.NET-Aspire-512BD4)](https://learn.microsoft.com/en-us/dotnet/aspire/)

> **A sample eShop application built with .NET Aspire, featuring AI-powered product search using Azure OpenAI and vector embeddings**

This repository demonstrates a modern cloud-native e-commerce application using .NET Aspire for orchestration, Blazor for the front-end, and AI capabilities for intelligent product search.

---

## 🎯 Features

- 🛒 **eShop Lite Store** - Blazor Server front-end with product browsing, cart, and checkout
- 🔍 **AI-Powered Search** - Semantic search using Azure OpenAI and vector embeddings
- 🚀 **.NET Aspire** - Cloud-native orchestration with service discovery
- 💾 **SQL Server** - Persistent data storage with Entity Framework Core
- 🧠 **Vector Database** - In-memory vector store for product embeddings
- 👥 **Customer Management** - Customer profiles with membership tiers

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

3. **Configure AI Connection (for AI-powered search)**
   
   Set up User Secrets for the Products project:
   ```bash
   cd src/Products
   dotnet user-secrets init
   dotnet user-secrets set "ConnectionStrings:microsoftfoundry" "your-azure-openai-connection-string"
   ```

4. **Run with .NET Aspire**
   ```bash
   cd src/eShopAppHost
   dotnet run
   ```

5. **Open the Aspire Dashboard**
   
   The Aspire dashboard will open automatically, showing all running services. Click on the **Store** endpoint to access the e-commerce application.

---

## 📁 Project Structure

```
src/
├── eShopLite-Aspire.slnx     # Solution file
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
├── CartEntities/             # Cart and order models
├── DataEntities/             # Product and customer models
├── SearchEntities/           # AI search response models
├── VectorEntities/           # Vector embedding models
│
├── Products.Tests/           # Products API tests
└── Store.Tests/              # Store front-end tests
```

---

## 🔧 Architecture

### Services

| Service | Description | Technology |
|---------|-------------|------------|
| **eShopAppHost** | .NET Aspire orchestrator | .NET Aspire |
| **Products** | Product catalog API with AI search | ASP.NET Core Minimal API |
| **Store** | E-commerce front-end | Blazor Server |
| **SQL Server** | Product database | SQL Server (containerized) |

### AI Features

The application includes AI-powered product search using:

- **Azure OpenAI GPT-5-mini** - For generating friendly search responses
- **Text Embedding 3 Small** - For creating product vector embeddings
- **In-Memory Vector Store** - For semantic similarity search

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
# Run all tests
cd src
dotnet test

# Run specific test project
dotnet test Products.Tests/Products.Tests.csproj
dotnet test Store.Tests/Store.Tests.csproj
```

---

## ☁️ Azure Deployment

The application supports deployment to Azure using Azure Developer CLI (azd):

```bash
cd src/eShopAppHost
azd up
```

This will provision:
- Azure Container Apps for services
- Azure SQL Database
- Azure OpenAI with GPT-5-mini and text-embedding-3-small deployments
- Azure Application Insights for monitoring

For more details, see [src/eShopAppHost/next-steps.md](src/eShopAppHost/next-steps.md).

---

## 📖 Key Technologies

- **.NET 9** - Latest .NET framework
- **.NET Aspire** - Cloud-native app orchestration
- **Blazor Server** - Interactive web UI
- **Entity Framework Core** - Data access
- **Microsoft.Extensions.AI** - AI abstractions
- **Azure OpenAI** - AI models for search
- **SQL Server** - Database storage
- **Docker** - Containerization

---

## 🤝 Contributing

Contributions are welcome! Please feel free to submit issues and pull requests.

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
