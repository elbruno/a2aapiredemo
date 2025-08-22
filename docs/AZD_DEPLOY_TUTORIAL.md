# Infra-only deploy (AI Foundry)

This document contains concise instructions for deploying only the AI Foundry resources located in the repository `infra/` folder. Use this when you want to provision the Cognitive Services (OpenAI) account and its `chat` and `embeddings` deployments for local demos.

Prerequisites

- `pwsh` (PowerShell) available in your shell path
- `az` (Azure CLI) installed and authenticated (`az login`)

Quick deploy (subscription-scoped)

Open a PowerShell prompt in the repository root and run.

Example (East US 2, environment name `brk447demo`):

```pwsh
# Authenticate if needed
az login

# Example deploy for East US 2 and env 'brk447demo'
az deployment sub create --location eastus2 --template-file infra/main.bicep --parameters environmentName=brk447demo location=eastus2
```

What this provisions

- A resource group named `rg-<env>`
- An Azure Cognitive Services account (OpenAI kind) and two deployments: `chat` and `embeddings`

Post-deploy: retrieve keys (if you plan to use key-based auth for demos)

```pwsh
# List keys for the Cognitive Services account. Replace <env> and <accountName>.
az cognitiveservices account keys list -g rg-brk447demo -n <accountName>

# Produce a full connection string in the format: [Endpoint=https://<resource>.openai.azure.com/;Key=<key>;]
# Example: fetch the primary key and print the connection string (PowerShell)
$accountName = '<accountName>'
$rg = 'rg-brk447demo'
$keys = az cognitiveservices account keys list -g $rg -n $accountName --query "{key:primaryKey}" -o tsv
$endpoint = az cognitiveservices account show -g $rg -n $accountName --query "properties.endpoint" -o tsv
"[Endpoint=$endpoint;Key=$keys;]"

Copy values from the deployment outputs


After you run the subscription deployment, Azure returns a deployment record that includes `outputs` and `outputResources` which contain the resource group and the Cognitive Services account name. You can view them with:

```pwsh
# Show the subscription deployment named 'main' and its outputs (adjust name if different)
az deployment sub show --name main --query "properties.outputs" -o json

```

- From the JSON output copy the resource group (for example `rg-brunobrk44704`) and the account name (for example `aifoundry-wdjty2jk3oiw2`).

Example using the actual deployment output values

```pwsh

# Using values from the deployment output shown in your message
$rg = 'rg-brunobrk44704'
$accountName = 'aifoundry-wdjty2jk3oiw2'

# Now list keys
az cognitiveservices account keys list -g $rg -n $accountName

# And print the formatted connection string (PowerShell)
$primaryKey = az cognitiveservices account keys list -g $rg -n $accountName --query "primaryKey" -o tsv
$endpoint = az cognitiveservices account show -g $rg -n $accountName --query "properties.endpoint" -o tsv
"[Endpoint=$endpoint;Key=$primaryKey;]"

```

Important: do NOT type angle-bracket placeholders like `<accountName>` directly into PowerShell — PowerShell treats `<` as a redirection operator. Replace placeholders with the real names (as shown above) before running the commands.

```

Notes and recommendations

- The infra module in `src/ZavaAppHost/infra/aifoundry/aifoundry.module.bicep` may enable local key-based authentication for demos (via `disableLocalAuth: false`). If you need to use key-based access in your containers or local code, provide the key as a secret (for example `AI_KEY`) to the deployment templates.
- For production, prefer `disableLocalAuth: true` and use Azure AD (managed identity) to authenticate to the Cognitive Services resource. This avoids long-lived key management.

## Cleanup (remove created resources)

If you want to tear down the resources created by the subscription deployment, follow these steps. Deleting the resource group will remove the Cognitive Services account and its deployments. The examples below use the `rg-demobrk447` resource group created in the example output — replace with your actual resource group name.


1. Delete the resource group (this deletes all resources inside it):

```pwsh
az group delete -n rg-demobrk447 --yes --no-wait
```

Use `--no-wait` to return immediately, or omit it to wait for the operation to complete. If you prefer to confirm the deletion interactively, remove `--yes`.
