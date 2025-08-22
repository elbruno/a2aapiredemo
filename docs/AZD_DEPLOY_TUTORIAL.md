<!--
  Canonical infra-only deployment guide for AI Foundry (Cognitive Services / OpenAI)
  Created: 2025-08-22
  This file provides prerequisites (including Visual Studio 2022 and Docker Desktop) and
  the minimal subscription-scoped deployment steps for demo usage.
-->

<!--
  Canonical infra-only deployment guide for AI Foundry (Cognitive Services / OpenAI)
  Created: 2025-08-22
  This file provides prerequisites (including Visual Studio 2022 and Docker Desktop) and
  the minimal subscription-scoped deployment steps for demo usage.
-->

# Infra-only deploy (AI Foundry) — step-by-step

This document describes the prerequisites needed to run the demo and the subscription-scoped steps to deploy the AI Foundry resources used by the sample apps.

## Prerequisites

Ensure you have the following installed before continuing (official download pages provided):

- Visual Studio 2022 (recommended for opening the full solution): [https://visualstudio.microsoft.com/vs/](https://visualstudio.microsoft.com/vs/)
- Docker Desktop (for building images or running container tools locally): [https://www.docker.com/products/docker-desktop/](https://www.docker.com/products/docker-desktop/)
- PowerShell (pwsh) — cross-platform shell: [https://learn.microsoft.com/powershell/](https://learn.microsoft.com/powershell/)
- Azure CLI (`az`) — used for deployment and resource operations: [https://learn.microsoft.com/cli/azure/install-azure-cli](https://learn.microsoft.com/cli/azure/install-azure-cli)
- .NET 9 SDK — required to build/run the .NET services in this repo: [https://dotnet.microsoft.com/en-us/download/dotnet/9.0](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)

Authenticate to Azure before running subscription-scoped deployments (required for the quick-run script and manual steps):

```pwsh
az login
```

or for a specific tenant use:

```pwsh
az login --tenant <tenantid>
```

## Modes (Quick vs Step-by-step)

You have two ways to deploy the AI Foundry resources:

- Quick mode (recommended): run the bundled PowerShell script which performs the deployment and prints a single-line connection string in the format `[Endpoint=<endpoint>;Key=<Key>;]`.
- Step-by-step mode: run the `az` commands manually (the previous sections in this document show the step-by-step commands).

Important: before using Quick mode you MUST run `az login` (see above) so the script can run subscription-scoped deployments and read outputs/keys.

### Quick mode (run the script)

From the repository root run:

```pwsh
pwsh -File scripts/deploy-ai-foundry.ps1 -Location eastus2 -EnvironmentName brk447demo
```

The script will:

- Deploy `infra/main.bicep` at subscription scope.
- Inspect the deployment outputs to find the resource group and Cognitive Services account.
- Retrieve the key and endpoint and print a single-line connection string: `[Endpoint=<endpoint>;Key=<Key>;]`.

If the script cannot auto-detect the outputs, it prints the raw deployment outputs and instructions to re-run with explicit values.

### Step-by-step mode (manual)

If you prefer to run each step manually, use the commands below.

## Deploy AI Foundry (subscription-scoped)

Run from the repository root (example uses `eastus2` and environment `brk447demo`):

```pwsh
az deployment sub create --location eastus2 --template-file infra/main.bicep --parameters environmentName=brk447demo location=eastus2
```

This creates a resource group `rg-<env>` and an Azure Cognitive Services account (OpenAI kind) with deployments `chat` and `embeddings`.

## Inspect outputs and retrieve keys

Get the subscription deployment outputs to locate the resource group and account name:

```pwsh
az deployment sub show --name main --query "properties.outputs" -o json
```

Use the account name and resource group in the following snippet to print a connection string in the format `[Endpoint=...;Key=...;]`:

```pwsh
$rg = 'rg-yourenv'
$accountName = 'aifoundry-xxxx'

$keysJson = az cognitiveservices account keys list -g $rg -n $accountName -o json | ConvertFrom-Json
if ($null -ne $keysJson.primaryKey) { $primaryKey = $keysJson.primaryKey } elseif ($null -ne $keysJson.key1) { $primaryKey = $keysJson.key1 } else { Write-Error 'No key found'; exit 1 }
$endpoint = az cognitiveservices account show -g $rg -n $accountName --query "properties.endpoint" -o tsv
Write-Output "[Endpoint=$endpoint;Key=$primaryKey;]"
```

There's a helper script at `infra/get-ai-connection.ps1` that automates the above steps and prints the connection string.

## (Optional) Assign a role to a principal

If you need to grant a principal access to the Cognitive Services account, run:

```pwsh
$principalId = '<principalId>'
$subscriptionId = az account show --query id -o tsv
$scope = "/subscriptions/$subscriptionId/resourceGroups/$rg/providers/Microsoft.CognitiveServices/accounts/$accountName"
$roleDefinitionId = '/subscriptions/' + $subscriptionId + '/providers/Microsoft.Authorization/roleDefinitions/5e0bd9bd-7b93-4f28-af87-19fc36ad61bd'
az role assignment create --assignee-object-id $principalId --role $roleDefinitionId --scope $scope
```

## Cleanup

To remove the created resource group and deployment record:

```pwsh
az group delete -n $rg --yes --no-wait
az deployment sub delete --name main
```

## Notes and recommendations

- For demo convenience the repo enables key-based auth in the Bicep templates; for production prefer `disableLocalAuth: true` and use managed identity (AAD).
- If you plan to run or debug services locally, ensure Visual Studio 2022 or VS Code plus .NET 9 SDK are installed.

If you'd like, I can parameterize `disableLocalAuth` in Bicep and/or add a post-deploy script to apply role assignments and inject the AI key into the Container App secret.

---

Last edited: 2025-08-22
