# Copilot instructions for this repository

Purpose

This file gives concise, actionable instructions for a GitHub Copilot Coding Agent working in this repository (Zava sample). Follow these steps exactly and leave inline comments in changed files explaining why you made each change.

Quick overview

- Root solution: `src/Zava-Aspire.slnx`
- AppHost (Aspire orchestrator): `src/ZavaAppHost` (entry point: `src/ZavaAppHost/Program.cs`)
- Store UI: `src/Store`
- New services (to create): `src/SingleAgentDemo`, `src/MultiAgentDemo`

Must-preserve instruction (verbatim)

"Important also marked that any new service that is added to the solution need to be registered in the .NET Aspire orchestration. So when an external service needs to call the new one they know how to do it using service discovery that is provided in .NET Aspire. The same with logging and everything else that net aspire services are providing."

Primary tasks (high-priority, idempotent)

1. Create two Web API projects under `src/` if they do not exist: `SingleAgentDemo` and `MultiAgentDemo` (target `net9.0`). Add both projects to `src/Zava-Aspire.slnx`.

2. Add required NuGet packages to each project (idempotent): `Microsoft.SemanticKernel` (1.61.0), Aspire packages (9.4.1), and `Swashbuckle.AspNetCore` (optional).

3. Enforce .NET 9 across the repository:

- Add or update `global.json` in repository root to pin the SDK to `9.0.x`.
- Ensure any new or modified project files use `TargetFramework` set to `net9.0` (or `net9.0;...` for multi-targeting).
- In PRs, include a check that `dotnet --version` on the runner begins with `9.` and that `dotnet --info` shows .NET 9 SDK installed.

4. Enforce .NET Aspire usage and capabilities for all new services:

- All new services MUST register with .NET Aspire for service discovery, logging, health checks, telemetry, configuration/secrets provisioning, and the Aspire dashboard.
- Update `src/ZavaAppHost/Program.cs` to register the services with Aspire so they are discoverable by other services in the solution and receive centralized logging/health.
- Use Aspire's built-in integrations for Application Insights (telemetry), configuration (secrets / user secrets / keyvault patterns), and local provisioning manifests where applicable.
- Add inline comments in modified files noting where Aspire registration and configuration is performed.

5. Keep existing task numbering for the remaining steps (shifted accordingly).

6. Implement minimal controllers and typed HttpClient service wrappers for the demo external services (`SmartPhotoAnalysisService`, `CustomerInformationService`, `CustomerWorkService`, `ZavaInventoryService`). Add health-check endpoints.

7. Register the new services with the Aspire AppHost in `src/ZavaAppHost/Program.cs` so they are discoverable and receive logging/health by Aspire (see "Must-preserve instruction" above).

8. Add two demo Store pages under `src/Store` for Scenario 1 (`/scenario1-single-agent`) and Scenario 2 (`/scenario2-multi-agent`) that call the API endpoints.

Run / test notes (for local development)

- Verify SDK: `dotnet --info` (requires .NET 9 installed).
- Create projects (PowerShell idempotent examples):

  - `dotnet new webapi -n SingleAgentDemo -o src/SingleAgentDemo --framework net9.0`
  - `dotnet new webapi -n MultiAgentDemo -o src/MultiAgentDemo --framework net9.0`
  - `dotnet sln src/eShopLite-Aspire.slnx add src/SingleAgentDemo/SingleAgentDemo.csproj`
  - `dotnet sln src/eShopLite-Aspire.slnx add src/MultiAgentDemo/MultiAgentDemo.csproj`

- Default local ports (allow env overrides):

  - SingleAgentDemo: `http://localhost:5002`
  - MultiAgentDemo: `http://localhost:5003`
  - MockServices (recommended): `http://localhost:5010`

- Required env vars for AI: `OPENAI_API_KEY` (or Azure OpenAI connection string), `AI_ChatDeploymentName`, `AI_embeddingsDeploymentName`.

Conventions & constraints

- Update user-facing `eShop` → `Zava` strings in README and UI. Do not rename namespaces or public APIs unless the repository owner explicitly requests it.
- Use typed `HttpClient` + DI for external service wrappers. Add lightweight health checks for each service.
- Keep changes incremental and idempotent. Add unit tests for any new public behavior if reasonable.

Files to reference when editing

- `src/ZavaAppHost/Program.cs` — register services with Aspire (service discovery, logging, health checks).
- `src/Zava-Aspire.slnx` — add new projects here.
- `src/Store` — add demo UI pages.
- `COPILOT_CODING_AGENT_INSTRUCTIONS.md` — authoritative step-by-step plan present at repository root.

If you make changes that affect CI or deployment manifests, update `README.md` and leave a short note in the PR description documenting the change and how to run locally.

When done

- Run `dotnet build src/Zava-Aspire.slnx -c Debug` and ensure solution builds.
- Start AppHost and new services locally, then run a smoke test POST to `/api/single-agent/analyze` and `/api/multi-agent/assist`.

If you need clarification at any step, prefer making small, reversible edits and opening a draft PR with clear commit messages explaining intent.
