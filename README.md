[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](/LICENSE)

## Description

**eShopLite - Semantic Search using SQL 2025** is a reference .NET application implementing an eCommerce site with Search features using vector search and vector indexes in the SQL Database Engine

- [Features](#features)
- [Architecture diagram](#architecture-diagram)
- [Getting started](#getting-started)
- [Deploying to Azure](#deploying)
- Run solution
- [Resources](#resources)
- [Video Recordings](#video-recordings)
- [Guidance](#guidance)
  - [Costs](#costs)
  - [Security Guidelines](#security-guidelines)
- [Resources](#resources)

## Features

**GitHub CodeSpaces:** This project is designed to be opened in GitHub Codespaces as an easy way for anyone to deploy the solution entirely in the browser.

This is the eShopLite Application running, performing a **Keyword Search**:

![eShopLite Application running doing search using keyword search](./images/05eShopLite-SearchKeyWord.gif)

This is the eShopLite Application running, performing a **Semantic Search**:

![eShopLite Application running doing search using keyword search](./images/06eShopLite-SearchSemantic.gif)

## Architecture diagram

  ```mermaid
  flowchart TD
      subgraph "Azure Container Apps Environment - .NET Aspire"
          store[store service]
          products[products service]
          sql[SQL service]
          store --> products
          products --> sql
      end
  
      ContainerRegistry[Container Registry]
      StorageAccount[Storage Account]
      ManagedIdentity[Managed Identity]
      OpenAI[Azure OpenAI\nChat + Embeddings]
      AISearch[Azure AI Search\nVector Index]
      LogAnalytics[Log Analytics]
  
      ContainerRegistry --> ManagedIdentity
      ManagedIdentity --> OpenAI
      ManagedIdentity --> AISearch
  
      products --> ManagedIdentity
      products -->|semantic search| AISearch
      products -->|generate embeddings + chat| OpenAI
  
      store --> LogAnalytics
      products --> LogAnalytics
  
  ```

## Getting Started

## Main Concepts in this Scenario

This scenario demonstrates how to use [SQL Server 2025's Vector search and Vector index features](https://learn.microsoft.com/sql/relational-databases/vectors/vectors-sql-server?view=sql-server-ver17) in a .NET Aspire application. The main concepts and implementation details are:

- The .NET Aspire AppHost project creates the SQL Server 2025 instance directly using the container image `mcr.microsoft.com/mssql/server:2025-latest` from the Docker repository for SQL Server 2025: [Microsoft SQL Server - Ubuntu based images](https://hub.docker.com/r/microsoft/mssql-server/).

- The logic for initializing and running the SQL Server container is implemented in [`scenarios/08-Sql2025/src/eShopAppHost/Program.cs`](scenarios/08-Sql2025/src/eShopAppHost/Program.cs):

    ```csharp
    var builder = DistributedApplication.CreateBuilder(args);
    
    var sql = builder.AddSqlServer("sql")
        .WithLifetime(ContainerLifetime.Persistent)
        .WithImageTag("2025-latest")
        .WithEnvironment("ACCEPT_EULA", "Y");
    
    var productsDb = sql
        .WithDataVolume()
        .AddDatabase("productsDb");
    
    var products = builder.AddProject<Projects.Products>("products")
        .WithReference(productsDb)
        .WaitFor(productsDb);    
    ```

- Using an embedding client, once the database is initialized and a set of sample products is added, a new vector field is completed using an embedding. This logic is in [`scenarios/08-Sql2025/src/Products/Models/DbInitializer.cs`](./src/Products/Models/DbInitializer.cs).

- The `ProductApiActions` class ([`scenarios/08-Sql2025/src/Products/Endpoints/ProductApiActions.cs`](./src/Products/Endpoints/ProductApiActions.cs)) implements an `AISearch()` function that performs semantic search using [EFCore.SqlServer.VectorSearch](https://www.nuget.org/packages/EFCore.SqlServer.VectorSearch/9.0.0-preview.2#show-readme-container) functions:

    ```csharp
    public static async Task<IResult> AISearch(string search, Context db, EmbeddingClient embeddingClient, int dimensions = 1536)
    {
        Console.WriteLine("Querying for similar products...");
    
        var embeddingSearch = embeddingClient.GenerateEmbedding(search, new() { Dimensions = dimensions });
        var vectorSearch = embeddingSearch.Value.ToFloats().ToArray();
        var products = await db.Product
            .OrderBy(p => EF.Functions.VectorDistance("cosine", p.Embedding, vectorSearch))
            .Take(3)
            .ToListAsync();
    
        var response = new SearchResponse
        {
            Products = products,
            Response = products.Count > 0 ?
                $"{products.Count} Products found for [{search}]" :
                $"No products found for [{search}]"
        [![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](/LICENSE)

        ## eShopLite - .NET Aspire + Azure Functions (Semantic Search)

        This repository demonstrates a small reference scenario that combines .NET Aspire (service discovery, resilience and orchestration) with an Azure Function that provides a semantic search endpoint over product data powered by SQL Server 2025 vector features.

        Goals for this README
        - Explain how .NET Aspire is used for service discovery and resilience
        - Explain how the Store app calls both the Products API and the independent Semantic Search Azure Function
        - Provide ASCII and mermaid diagrams to illustrate runtime architecture
        - Show configuration keys and quick run steps

        ## High-level architecture (ASCII)

        Simple ASCII diagram showing the main runtime pieces and service discovery layer:

        ```
                                +---------------------+
                                |   Service Registry  |  <- .NET Aspire Service Discovery
                                +----------+----------+
                                           ^
                       service discovery    |    service discovery
           +-----------+        +-----------+v+         +---------------------+
           |  Browser  | <----> |   Store    | <-----> |   Products Service   |  (product APIs, embeddings, SQL2025)
           +-----------+        +-----------+           +---------------------+
                                         \
                                          \_ HTTP (via Aspire HttpClient)
                                           \
                                            \-> +-------------------------+
                                                 |  Semantic Search Azure   |  (POST /api/semanticsearch)
                                                 |  Function (SQL2025)      |
                                                 +-------------------------+

        ```

        ## High-level architecture (Mermaid)

        ```mermaid
        flowchart LR
            Browser[Browser / Store UI]
            StoreService[Store Service (Blazor/UI)]
            Products[Products Service (API + SQL2025)]
            SemanticFn[Semantic Search Azure Function]
            ServiceDiscovery[(Service Discovery)]
            SQL[(SQL Server 2025 - Vector Index)]

            Browser -->|HTTP| StoreService
            StoreService -->|aspire-http (products)| ServiceDiscovery --> Products
            StoreService -->|aspire-http (semanticfunction)| ServiceDiscovery --> SemanticFn
            Products -->|vector search| SQL
            SemanticFn -->|vector search| SQL

            classDef infra fill:#f9f,stroke:#333,stroke-width:1px;
            class ServiceDiscovery infra;
        ```

        ## How the code uses .NET Aspire

        - The repository includes an `eShopServiceDefaults` project which exposes `AddServiceDefaults()`.
          - This method registers service discovery and configures HttpClient defaults (resilience handlers and service discovery handlers).
          - Files to look at: `src/eShopServiceDefaults/Extensions.cs`.
        - In `src/Store/Program.cs` the Store app registers HttpClients that use the Aspire service discovery URIs, e.g.:

        ```csharp
        builder.AddServiceDefaults();

        // ProductService uses Aspire service discovery to resolve "products" service
        builder.Services.AddHttpClient<ProductService>(static client =>
        {
            client.BaseAddress = new Uri("https+http://products");
        });

        // Azure Function Semantic Search service uses Aspire service discovery to resolve "semanticsearchfunction"
        builder.Services.AddHttpClient<IAzFunctionSearchService, AzFunctionSearchService>(static client =>
        {
            client.BaseAddress = new Uri("https+http://semanticsearchfunction");
        });
        ```

        This avoids hard-coded hostnames and ports in code; the `https+http://{service}` pattern is interpreted by the service discovery/resolver configured by Aspire.

        ## Store -> Semantic Search integration

        - The Store UI provides a search page with 3 modes: Standard, Semantic (AI via Products API), and Azure Function Semantic Search.
        - For the Azure Function mode the Store's `ProductService` delegates the call to `IAzFunctionSearchService` which posts JSON { query, top } to `/api/semanticsearch`.
        - The `AzFunctionSearchService` was implemented to prefer a configuration value `SemanticSearchFunctionEndpoint` (which can be a service discovery URI or a real URL) and otherwise use the HttpClient base address (registered via service discovery).

        ## Config keys you may want to set locally

        - `ProductEndpoint` (optional) — legacy; with Aspire you should prefer service discovery
        - `SemanticSearchFunctionEndpoint` — set this to a concrete URL (eg. `http://localhost:7071/`) for local debugging or to a service discovery URI (eg. `https+http://semanticsearchfunction`) if using Aspire discovery
        - Connection strings and SQL settings (see `src/SemanticSearchFunction/local.settings.json` and `src/Store/appsettings.json`)

        Example `appsettings.Development.json` snippet:

        ```json
        {
          "ProductEndpoint": "http://localhost:5228/",
          "SemanticSearchFunctionEndpoint": "http://localhost:7071/"
        }
        ```

        Or, to use service discovery for both services in an Aspire-hosted environment:

        ```json
        {
          "SemanticSearchFunctionEndpoint": "https+http://semanticsearchfunction"
        }
        ```

        ## Quick local run (developer flow)

        1. Start the Products service and SQL2025 (see `src/Products` instructions) so the product API and database are available.
        2. Start the Semantic Search Azure Function (open `src/SemanticSearchFunction` in VS Code and run the function locally) or run the function in your environment.
        3. Start the Store app:

        ```powershell
        cd src/Store
        dotnet run
        # Open https://localhost:5001/search (or the address displayed by ASP.NET Core)
        ```

        4. On the Store search page, choose "Azure Function Semantic Search" from the dropdown, enter a query and run.

        ## Diagrams (ASCII + Mermaid) — Usage notes

        - ASCII diagrams are intentionally simple and useful when viewing README in plain terminals or small editors.
        - Mermaid diagrams render nicely on GitHub and VS Code markdown preview and give a clearer view of relationships and service discovery.

        ## Troubleshooting

        - If service discovery does not resolve a service name, check that the environment where Aspire runs provides a discovery provider (for local dev this may be skipped and you must use local URLs in `SemanticSearchFunctionEndpoint`).
        - Use logs: the solution is instrumented with OpenTelemetry and logging via `AddServiceDefaults()` — check app logs for discovery/resolver entries.

        ## Contributing

        If you want to extend the scenario:
        - Add health checks for the function and wire them into `MapDefaultEndpoints()`
        - Add a configuration UI to allow switching between service discovery and explicit endpoints at runtime
        - Add tests for `AzFunctionSearchService` (mock HttpClient) and for `ProductService` delegating behavior

        ---

        For full technical documentation about SQL Server 2025 vector features and the original scenario, see `./docs/README.md`.
