# Copilot Coding Agent Instructions — Zava sample upgrade and agents

Purpose

This file contains step-by-step instructions and code templates that a GitHub Copilot Coding Agent should execute to implement the requested changes.

Key goals

- Rename user-visible `eShop` strings to `Zava`.
- Add two backend Web API projects under `src/`: `SingleAgentDemo` and `MultiAgentDemo`.
- Enroll the new services into the existing Aspire orchestration hosted by `src/eShopAppHost`.
- Implement Scenario 1 (single multimodal agent) and Scenario 2 (multi-agent orchestration) demos.

Important requirement from the repository owner

"Important also marked that any new service that is added to the solution need to be registered in the .NET Aspire orchestration. So when an external service needs to call the new one they know how to do it using service discovery that is provided in .NET Aspire. The same with logging and everything else that net aspire services are providing."

High-level intent

- Scenario 1: a single agent that performs photo analysis, customer lookups, skill/tool matching, and inventory enrichment to return a recommendation.
- Scenario 2: a multi-agent orchestration where inventory, matchmaking, location, and navigation agents coordinate to produce structured orchestration steps, product alternatives, and optional in-store navigation instructions returned from a single endpoint.

Assumptions

- .NET 9 SDK is available on the runner.
- .NET Aspire 9.4.1 packages and `Microsoft.SemanticKernel` 1.61.0 are available from NuGet.
- The repository contains `src/eShopAppHost` (AppHost) which is the place to register new services for Aspire.

Plan (work to perform)

1. Create two new Web API projects under `src/`: `SingleAgentDemo` and `MultiAgentDemo`.

1. Add NuGet packages to each project: `Microsoft.SemanticKernel` (1.61.0), Aspire packages (9.4.1), and `Swashbuckle.AspNetCore` (optional for OpenAPI).

1. Implement controller endpoints and typed HTTP clients for the external services used by the agents:

- `SmartPhotoAnalysisService`
- `CustomerInformationService`
- `CustomerWorkService`
- `ZavaInventoryService`

1. Add demo endpoints:

- `POST /api/single-agent/analyze` — multipart/form-data (image + prompt + customerId)
- `POST /api/multi-agent/assist` — JSON (userId, productQuery, optional location)

1. Register the new services in `src/eShopAppHost/Program.cs` so they are discoverable via Aspire (service registration, logging, health checks).

1. Add Store demo pages to `src/Store` that call the demo endpoints for Scenario 1 and Scenario 2.

1. Update user-visible `eShop` -> `Zava` strings (README, display names). Do not rename namespaces unless requested.

Files to create or modify

- `src/SingleAgentDemo/*` — project, `Program.cs`, `Controllers/SingleAgentController.cs`, `Services/*`
- `src/MultiAgentDemo/*` — project, `Program.cs`, `Controllers/MultiAgentController.cs`, `Services/*`
- `src/MockServices/*` (optional) — local mock server for external dependencies with `/health` endpoint
- `src/eShopAppHost/Program.cs` — register services with Aspire
- `src/Store/Pages/Scenario1-SingleAgent.*` and `Scenario2-MultiAgent.*` — demo UI pages
- `README.md` — run & test instructions and required env vars

Execution commands (PowerShell)

```powershell
# Verify dotnet SDK
dotnet --info

# Create projects if missing
if (-not (Test-Path -Path src/SingleAgentDemo)) { dotnet new webapi -n SingleAgentDemo -o src/SingleAgentDemo --framework net9.0 }
if (-not (Test-Path -Path src/MultiAgentDemo))  { dotnet new webapi -n MultiAgentDemo -o src/MultiAgentDemo --framework net9.0 }

# Add to solution (idempotent)
dotnet sln src/eShopLite-Aspire.slnx add src/SingleAgentDemo/SingleAgentDemo.csproj || Write-Host "SingleAgentDemo already in solution or add failed"
dotnet sln src/eShopLite-Aspire.slnx add src/MultiAgentDemo/MultiAgentDemo.csproj || Write-Host "MultiAgentDemo already in solution or add failed"

# Add packages
dotnet add src/SingleAgentDemo/SingleAgentDemo.csproj package Microsoft.SemanticKernel --version 1.61.0
dotnet add src/MultiAgentDemo/MultiAgentDemo.csproj package Microsoft.SemanticKernel --version 1.61.0
dotnet add src/SingleAgentDemo/SingleAgentDemo.csproj package Aspire --version 9.4.1
dotnet add src/MultiAgentDemo/MultiAgentDemo.csproj package Aspire --version 9.4.1
dotnet add src/SingleAgentDemo/SingleAgentDemo.csproj package Swashbuckle.AspNetCore
dotnet add src/MultiAgentDemo/MultiAgentDemo.csproj package Swashbuckle.AspNetCore

dotnet restore src/eShopLite-Aspire.slnx
```

Controller & service outlines

SingleAgentController (summary):

- `POST /api/single-agent/analyze` accepts an image, `prompt`, and `customerId`.
- Calls: `SmartPhotoAnalysisService` -> `CustomerInformationService` -> Semantic Kernel -> `CustomerWorkService` -> `ZavaInventoryService`.
- Returns: reusable tools, missing tools with SKUs and availability, plus reasoning text.

MultiAgentController (summary):

- `POST /api/multi-agent/assist` accepts JSON with `userId`, `productQuery`, optional `location` (a `Location` with `Lat` and `Lon`).
- Behavior implemented in `src/MultiAgentDemo/Controllers/MultiAgentController.cs`:
  - Validates `ProductQuery` and builds an orchestration id.
  - Invokes (typed HTTP client) services in order: `IInventoryAgentService`, `IMatchmakingAgentService`, `ILocationAgentService`.
  - If `location` is provided, invokes `INavigationAgentService` to generate directions and includes `NavigationInstructions` in the response.
  - Returns `MultiAgentResponse` containing:
    - `OrchestrationId` (string)
    - `Steps` (array of `AgentStep` objects describing each agent action and result)
    - `Alternatives` (array of `ProductAlternative`)
    - `NavigationInstructions` (optional `NavigationInstructions` with `Steps`, `StartLocation`, `EstimatedTime`)

Mock services (recommended)

Create an optional `MockServices` Web API with endpoints like `/api/mock/photo-analysis`, `/api/mock/customer/{id}`, `/api/mock/customer-work/match`, `/api/mock/inventory/search` and a `/health` endpoint. Configure demo projects to use the mock base URL during local runs via `appsettings.Development.json` or env vars.

Testing & validation

1. Build the solution:

```powershell
dotnet build src/eShopLite-Aspire.slnx -c Debug
```

1. Run services (example ports):

```powershell
dotnet run --project src/SingleAgentDemo -c Debug --urls "http://localhost:5002"
dotnet run --project src/MultiAgentDemo -c Debug --urls "http://localhost:5003"
dotnet run --project src/eShopAppHost -c Debug
```

1. Smoke test endpoints with `curl` or `Invoke-WebRequest`:

```powershell
curl -X POST "http://localhost:5002/api/single-agent/analyze" -F "image=@./testroom.jpg" -F "prompt=I want to paint this room blue. What tools do I need to do this?" -F "customerId=123"

curl -X POST "http://localhost:5003/api/multi-agent/assist" -H "Content-Type: application/json" -d '{"userId":"123","productQuery":"paint sprayer","location":{"lat":0,"lon":0}}'
```

Semantic Kernel notes

- Use `Microsoft.SemanticKernel` 1.61.0. For multimodal flows, prefer pre-processing images via `SmartPhotoAnalysisService` and pass textual descriptions to the Kernel.

Idempotency & environment

- Use `Test-Path` guards in PowerShell scripts.
- Use explicit default ports (5002 for SingleAgent, 5003 for MultiAgent, 5010 for MockServices) and allow env var overrides.
- Document required env vars: `OPENAI_API_KEY`, `MOCK_SERVICES_PORT`, etc.

Deliverable checklist

- [ ] Create and add projects to the solution
- [ ] Add package references
- [ ] Implement controllers and services
- [ ] Enroll into AppHost Aspire orchestration (service registration, discovery, logging, health)
- [ ] Add Store demo pages for Scenario 1 and Scenario 2
- [ ] Provide run & test instructions in README

Appendix: example service client interface

```csharp
namespace Zava.SingleAgentDemo.Services;

public interface IPhotoAnalysisClient
{
  Task<PhotoAnalysisResult> AnalyzeAsync(Stream imageStream, CancellationToken ct = default);
}

public class PhotoAnalysisResult { public string Description { get; set; } = string.Empty; public string[] DetectedMaterials { get; set; } = Array.Empty<string>(); }
```

End of instructions.
