# Infra-only deploy (AI Foundry)

This document contains concise instructions for deploying only the AI Foundry resources located in the repository `infra/` folder. Use this when you want to provision the Cognitive Services (OpenAI) account and its `chat` and `embeddings` deployments for local demos.

Prerequisites

- `pwsh` (PowerShell) available in your shell path
- `az` (Azure CLI) installed and authenticated (`az login`)

Quick deploy (subscription-scoped)

Open a PowerShell prompt in the repository root and run:

```pwsh
# Authenticate if needed
az login

# Deploy AI Foundry only. Replace <location> and <env>.
az deployment sub create --location <location> --template-file infra/main.bicep --parameters environmentName=<env> location=<location>
```

What this provisions

- A resource group named `rg-<env>`
- An Azure Cognitive Services account (OpenAI kind) and two deployments: `chat` and `embeddings`

Post-deploy: retrieve keys (if you plan to use key-based auth for demos)

```pwsh
# Get the account name from the deployment outputs or list accounts in the resource group
az cognitiveservices account keys list -g rg-<env> -n <accountName>
```

Notes and recommendations

- The infra module in `src/ZavaAppHost/infra/aifoundry/aifoundry.module.bicep` may enable local key-based authentication for demos (via `disableLocalAuth: false`). If you need to use key-based access in your containers or local code, provide the key as a secret (for example `AI_KEY`) to the deployment templates.
- For production, prefer `disableLocalAuth: true` and use Azure AD (managed identity) to authenticate to the Cognitive Services resource. This avoids long-lived key management.

If you want me to update the repo to enforce AAD-only auth (managed identity) and modify the products service to use `DefaultAzureCredential`, I can implement that end-to-end (Bicep + code + small smoke test).
