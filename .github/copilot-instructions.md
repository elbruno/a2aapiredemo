# GitHub Copilot Instructions for eShop Lite Aspire Demo

## Repository Purpose

This repository contains an eShop Lite demo application built with .NET Aspire, featuring AI-powered product search using Azure OpenAI and vector embeddings. It demonstrates modern cloud-native e-commerce patterns with Blazor Server front-end and ASP.NET Core Minimal API backend.

## Code Generation Guidelines

### When Working with the Products API

- Use ASP.NET Core Minimal API patterns
- Use `Microsoft.Extensions.AI` for AI abstractions
- Use Entity Framework Core for database access
- Follow the existing endpoint pattern in `ProductEndpoints.cs`
- Use `Context` for database operations
- Use `MemoryContext` for AI-powered search operations

### When Working with the Store Front-End

- Use Blazor Server components
- Follow the existing component structure in `Components/Pages/`
- Use dependency injection for services (`IProductService`, `ICartService`, etc.)
- Use `ProtectedSessionStorage` for cart persistence

### When Working with Aspire Configuration

- Configure services in `eShopAppHost/Program.cs`
- Use `builder.AddProject<>()` for adding projects
- Use `WithReference()` for service dependencies
- Use `WaitFor()` for startup ordering

## Naming Conventions

- Entity projects: Suffix with `Entities` (e.g., `DataEntities`, `CartEntities`)
- Test projects: Suffix with `.Tests` (e.g., `Products.Tests`, `Store.Tests`)
- Use PascalCase for class names and public members
- Use camelCase for private fields and local variables

## Configuration Standards

**CRITICAL**: All projects must use .NET User Secrets for sensitive configuration. NO .env files allowed.

```csharp
// Good - Using IConfiguration with User Secrets
var connectionString = configuration["ConnectionStrings:microsoftfoundry"];

// Bad - Never use .env files or environment variables directly
// DotNetEnv.Env.Load();
```

### User Secrets Setup

```bash
cd src/Products
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:microsoftfoundry" "your-connection-string"
```

### appsettings.json Structure

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  },
  "AI_ChatDeploymentName": "gpt-5-mini",
  "AI_embeddingsDeploymentName": "text-embedding-3-small"
}
```

## Project Structure

```
src/
├── eShopLite-Aspire.slnx     # Solution file
│
├── 1 Aspire/
│   ├── eShopAppHost/         # .NET Aspire App Host
│   └── eShopServiceDefaults/ # Shared service configuration
│
├── 2 eShopLite/
│   ├── Products/             # Products API
│   │   ├── Endpoints/        # REST API endpoints
│   │   ├── Memory/           # Vector database context
│   │   ├── Models/           # EF Core context
│   │   └── Data/             # Database initialization
│   └── Store/                # Blazor Server front-end
│       ├── Components/       # Razor components
│       │   ├── Pages/        # Application pages
│       │   ├── Cart/         # Cart components
│       │   └── Layout/       # Layout components
│       └── Services/         # Business logic
│
├── 3 Models/
│   ├── CartEntities/         # Cart and order models
│   ├── DataEntities/         # Product and customer models
│   ├── SearchEntities/       # Search response models
│   └── VectorEntities/       # Vector embedding models
│
└── 4 Tests/
    ├── Products.Tests/       # Products API tests
    └── Store.Tests/          # Store front-end tests
```

## Common Patterns to Follow

### Adding API Endpoints

Follow the pattern in `ProductEndpoints.cs`:

```csharp
public static void MapMyEndpoints(this IEndpointRouteBuilder routes)
{
    var group = routes.MapGroup("/api/MyEntity");

    group.MapGet("/", MyApiActions.GetAll)
        .WithName("GetAllMyEntities")
        .Produces<List<MyEntity>>(StatusCodes.Status200OK);
}
```

### Adding Blazor Pages

Follow the existing page structure:

```razor
@page "/mypage"
@inject IMyService MyService

<PageTitle>My Page</PageTitle>

<h1>My Page Title</h1>

@code {
    private List<MyEntity> entities = [];

    protected override async Task OnInitializedAsync()
    {
        entities = await MyService.GetAllAsync();
    }
}
```

### Adding Entity Models

Follow the existing pattern in `DataEntities`:

```csharp
using System.Text.Json.Serialization;

namespace DataEntities;

public class MyEntity
{
    [JsonPropertyName("id")]
    public virtual int Id { get; set; }

    [JsonPropertyName("name")]
    public virtual string Name { get; set; } = "not defined";
}
```

## Repository-Specific Rules

1. Use .NET Aspire for service orchestration
2. Use SQL Server for persistent storage
3. Use in-memory vector store for AI search
4. Follow Minimal API patterns for endpoints
5. Use Blazor Server for the front-end
6. Follow .NET 9 best practices
7. Use async/await properly
8. Implement proper error handling
9. All configuration via User Secrets and IConfiguration
10. No .env files or direct environment variable access

## Testing Guidelines

- Use MSTest for unit tests
- Test API endpoints independently
- Mock `Context` and `MemoryContext` for unit tests
- Use integration tests for full service testing

## When Adding New Content

- Add entity models to the appropriate `*Entities` project
- Add API endpoints to the `Products/Endpoints/` directory
- Add Blazor pages to `Store/Components/Pages/`
- Update `eShopAppHost/Program.cs` for new services
- Add tests to the appropriate test project

## Target Framework

All projects must target .NET 9:

```xml
<PropertyGroup>
  <TargetFramework>net9.0</TargetFramework>
  <Nullable>enable</Nullable>
</PropertyGroup>
```

## Key Package References

### Products API
```xml
<PackageReference Include="Microsoft.Extensions.AI" Version="9.x" />
<PackageReference Include="Microsoft.Extensions.AI.OpenAI" Version="9.x" />
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="9.x" />
```

### Store Front-End
```xml
<PackageReference Include="Microsoft.AspNetCore.Components.WebAssembly.Server" Version="9.x" />
```

### Aspire
```xml
<PackageReference Include="Aspire.Hosting.AppHost" Version="9.x" />
<PackageReference Include="Aspire.Hosting.SqlServer" Version="9.x" />
```

## Quality Standards

- All code must compile without errors or warnings
- Follow C# naming conventions
- Use proper async/await patterns
- Include error handling
- Add XML documentation for public APIs
- Write clear, self-documenting code
