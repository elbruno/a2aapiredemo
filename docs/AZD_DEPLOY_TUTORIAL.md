## AZD deploy tutorial

This tutorial explains how to deploy the necessary cloud resources for this repository using the Azure Developer CLI (`azd`) and the `azd up` command from the `src\ZavaAppHost` folder.

Prerequisites

- Windows 10/11 or a supported OS
- PowerShell (pwsh) or Windows PowerShell
- .NET SDK 9.0.x installed
- Visual Studio 2022/2023 with ASP.NET and web development workloads (or Visual Studio Code)
- Azure Developer CLI (`azd`) installed and available in `PATH`
- Docker Desktop installed and running (for containerized services)
- An Azure subscription and appropriate permissions to create resources
- Optional: `OPENAI_API_KEY` or other AI provider environment variables if you plan to enable AI features

Quick checklist

- Verify PowerShell: `pwsh --version` or `powershell --version`
- Verify .NET SDK: `dotnet --version` (should start with `9.`)
- Verify Azure Developer CLI: `azd version`
- Verify Docker: `docker --version` and ensure Docker Desktop is running

Prepare repository

1. Open a PowerShell terminal (pwsh) in the repository root.
2. Ensure you are on the correct branch and the repo is up to date.

Deploy with `azd up`

1. Change directory to the AppHost folder that contains the deployment manifest:

   ```powershell
   cd src\ZavaAppHost
   ```

2. Run the Azure Developer CLI command to provision and deploy resources:

   ```powershell
   azd up
   ```

3. Follow interactive prompts from `azd up`. The command will:

- Create or select an Azure subscription and resource group
- Provision required resources (App Service, Storage, Container Registry, etc.)
- Build and deploy the application

Environment variables and secrets

- If the application requires AI keys or other secrets set these before running `azd up` or provide them through the Azure Key Vault integration that `azd` configures. Example environment variables you may need locally before running the app:

  ```powershell
  $env:OPENAI_API_KEY = "<your-openai-key>"
  $env:AI_ChatDeploymentName = "<chat-deployment-name>"
  $env:AI_embeddingsDeploymentName = "<embeddings-deployment-name>"
  ```

Verification and smoke tests

- After `azd up` completes, note the endpoints and connection strings printed by the CLI.
- Test the main AppHost endpoint (example):

   ```powershell
   Invoke-RestMethod -Uri http://localhost:5000/health
   ```

- If services are deployed to Azure, open the provided App Service URL in your browser and call the same health endpoint.

Troubleshooting

- `azd` asks for login: run `az login` before `azd up` to authenticate.
- Insufficient permissions: ensure your Azure account can create resources or use a service principal with required role assignments.
- Docker not running: start Docker Desktop before running if `azd` needs to build container images.
- .NET SDK mismatch: ensure `dotnet --version` prints a version starting with `9.` to match repository `global.json`.

Notes and best practices

- Keep secrets out of source control. Use Azure Key Vault and `azd` secret management.
- For CI, add a pipeline step to run `azd` in non-interactive mode with service principal credentials.
- If you add new services, update `src\ZavaAppHost/Program.cs` to register them with the Aspire orchestrator and update any deployment manifests used by `azd`.

Next steps

- Run `azd up` from `src\ZavaAppHost` and follow the output.
- After successful deploy, run smoke tests against the `/api/single-agent/analyze` and `/api/multi-agent/assist` endpoints if those services are included.

References

- Azure Developer CLI: <https://learn.microsoft.com/azure/developer/azure-developer-cli>
- Docker Desktop: <https://www.docker.com/products/docker-desktop>

--
Tutorial created for this repository. If you want a CI/CD pipeline or a non-interactive `azd` workflow, I can add an example GitHub Actions workflow next.
