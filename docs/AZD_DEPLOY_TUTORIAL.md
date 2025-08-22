# AZD deploy tutorial

This tutorial explains how to provision the cloud resources required for this repository using the Azure Developer CLI (`azd`) from the `src\ZavaAppHost` folder. The recommended primary command for this demo is `azd provision` which creates resources only; deploying the application (`azd up`) is optional and documented as an extra step.

## Prerequisites

- Windows 10/11 or a supported OS
- PowerShell (pwsh) — install instructions: [Install PowerShell](https://learn.microsoft.com/powershell/scripting/install/installing-powershell)
- .NET SDK 9.0.x (required) — download: [.NET 9 downloads](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- Visual Studio 2022 **Enterprise** or **Preview** (with ASP.NET and web development workload) — downloads:
  - [Visual Studio 2022 Enterprise](https://visualstudio.microsoft.com/vs/enterprise/)
  - [Visual Studio Preview](https://visualstudio.microsoft.com/vs/preview/)
- Azure Developer CLI (`azd`) — install: [azd install docs](https://learn.microsoft.com/azure/developer/azure-developer-cli/install-azd)
- Docker Desktop (for containerized builds) — download: [Docker Desktop](https://www.docker.com/products/docker-desktop)
- An Azure subscription (create a free account if needed): [Azure free account](https://azure.microsoft.com/free/)

## Quick checklist

- Verify PowerShell: `pwsh --version` or `powershell --version`
- Verify .NET SDK: `dotnet --version` (should start with `9.`)
- Verify Azure Developer CLI: `azd version`
- Verify Docker: `docker --version` and ensure Docker Desktop is running

## Prepare repository

1. Open a PowerShell terminal (pwsh) in the repository root.
2. Ensure you are on the correct branch and the repo is up to date.

## Provision resources (primary step)

1. Change directory to the AppHost folder that contains the `azd` project configuration:

   ```powershell
   cd src\ZavaAppHost
   ```

2. Run the Azure Developer CLI command to provision cloud resources (creates resource group, storage, container registry, etc., but does not deploy the application code):

   ```powershell
   azd provision
   ```

3. Follow the interactive prompts. `azd provision` will:

- Create or select an Azure subscription and resource group
- Provision required cloud resources for the demo (App Service, Storage, Container Registry, Key Vault, etc., depending on the template)

Note: `azd provision` prepares infrastructure only; it does not build or deploy application artifacts.

## Optional: deploy the application

If you want to build and deploy the demo application after provisioning, run the following (optional):

```powershell
azd up
```

`azd up` will build container images or .NET projects and deploy them into the provisioned resources — use this only when you want a full end-to-end deploy.

## Troubleshooting

- `azd` asks for login: run `az login` before `azd provision` to authenticate.
- Insufficient permissions: ensure your Azure account can create resources or use a service principal with required role assignments.
- Docker not running: start Docker Desktop before running if `azd` needs to build container images.
- .NET SDK mismatch: ensure `dotnet --version` prints a version starting with `9.` to match the repository requirement.

## Notes and best practices

- Keep secrets out of source control. Use Azure Key Vault and `azd` secret management.
- For CI, run `azd` in non-interactive mode using a service principal and provide required inputs through environment variables or pipeline secrets.
- If you add new services, update `src\ZavaAppHost/Program.cs` to register them with the Aspire orchestrator and update any `azd` manifests used by the project.

## Next steps

- Run `azd provision` from `src\ZavaAppHost` to create required cloud resources.
- Optionally run `azd up` to build and deploy the application artifacts into the provisioned resources.

## References

- Azure Developer CLI (azd) install & docs: [azd docs](https://learn.microsoft.com/azure/developer/azure-developer-cli)
- PowerShell install: [Install PowerShell](https://learn.microsoft.com/powershell/scripting/install/installing-powershell)
- .NET 9 downloads: [.NET 9 downloads](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- Visual Studio 2022 Enterprise: [Visual Studio 2022 Enterprise](https://visualstudio.microsoft.com/vs/enterprise/)
- Visual Studio Preview: [Visual Studio Preview](https://visualstudio.microsoft.com/vs/preview/)
- Docker Desktop: [Docker Desktop](https://www.docker.com/products/docker-desktop)
- Create an Azure free account: [Azure free account](https://azure.microsoft.com/free/)
