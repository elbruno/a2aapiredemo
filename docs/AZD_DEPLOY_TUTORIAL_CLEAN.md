# Infra-only deploy (AI Foundry) — step-by-step

Follow these numbered steps to deploy the AI Foundry (Azure Cognitive Services / OpenAI) resources and retrieve keys for local demos. Replace the example values with the values shown in your deployment outputs.

1. Prerequisites

- Install PowerShell (`pwsh`) and the Azure CLI (`az`) and ensure they are available in your PATH.
- Authenticate with Azure:

```pwsh
az login
```

1. Deploy AI Foundry (subscription-scoped)

Run the subscription-scoped deployment from the repository root. Example for `eastus2` and environment `brk447demo`:

```pwsh
az deployment sub create --location eastus2 --template-file infra/main.bicep --parameters environmentName=brk447demo location=eastus2
```

The deployment will create a resource group named `rg-<env>` (for example `rg-brunobrk44704`) and an Azure Cognitive Services account with deployments `chat` and `embeddings`.

1. Inspect deployment outputs and find the account name

After the deployment completes, view the subscription deployment outputs to get the resource group and account names:

```pwsh
az deployment sub show --name main --query "properties.outputs" -o json
```

Copy the values for the account name and resource group from the JSON output (for example, `aifoundry-wdjty2jk3oiw2` and `rg-brunobrk44704`).

1. List keys and print a full connection string

Replace `$rg` and `$accountName` with the values you copied from step 3. The snippet below handles different API shapes (older `key1`/`key2` or newer `primaryKey`) and prints the connection string in this format:

```pwsh
# Replace these with the names from your deployment outputs
$rg = 'rg-brunobrk44704'
$accountName = 'aifoundry-wdjty2jk3oiw2'

# Get the keys JSON
$keysJson = az cognitiveservices account keys list -g $rg -n $accountName -o json | ConvertFrom-Json

# Choose the available key
if ($null -ne $keysJson.primaryKey) {
    $primaryKey = $keysJson.primaryKey
} elseif ($null -ne $keysJson.key1) {
    $primaryKey = $keysJson.key1
} else {
    Write-Error 'No key found in response. Inspect full JSON: az cognitiveservices account keys list -g $rg -n $accountName -o json'
    exit 1
}

# Get the endpoint
$endpoint = az cognitiveservices account show -g $rg -n $accountName --query "properties.endpoint" -o tsv

# Print the connection string in the requested format
Write-Output "[Endpoint=$endpoint;Key=$primaryKey;]"
```

1. (Optional) Assign a role to a principal

If you need to assign the Cognitive Services OpenAI role to a user or service principal, run this after deployment (replace `<principalId>` with the object id of the principal):

```pwsh
$principalId = '<principalId>'
$subscriptionId = az account show --query id -o tsv
$scope = "/subscriptions/$subscriptionId/resourceGroups/$rg/providers/Microsoft.CognitiveServices/accounts/$accountName"

# Create the role assignment using the built-in role definition id used in this repo
$roleDefinitionId = '/subscriptions/' + $subscriptionId + '/providers/Microsoft.Authorization/roleDefinitions/5e0bd9bd-7b93-4f28-af87-19fc36ad61bd'
az role assignment create --assignee-object-id $principalId --role $roleDefinitionId --scope $scope
```

1. Cleanup (remove created resources)

To remove the demo resources (delete the resource group and subscription deployment):

```pwsh
# Delete the resource group (this will remove the Cognitive Services account and deployments)
az group delete -n $rg --yes --no-wait

# Optionally delete the subscription-level deployment record (named 'main')
az deployment sub delete --name main
```

If you created a role assignment and want to delete it, list assignments for the account scope and delete by id:

```pwsh
$scope = "/subscriptions/$(az account show --query id -o tsv)/resourceGroups/$rg/providers/Microsoft.CognitiveServices/accounts/$accountName"
az role assignment list --scope $scope -o json
az role assignment delete --ids <roleAssignmentId>
```

Notes and recommendations

- Replace placeholders (like `$rg`, `$accountName`, and `<principalId>`) with your actual values from the deployment outputs. Do NOT type angle-bracket placeholders such as `<accountName>` directly into PowerShell.
- For local demos you may allow key-based authentication (`disableLocalAuth: false`), but for production prefer `disableLocalAuth: true` and use managed identity (AAD) authentication.

If you'd like, I can add a helper PowerShell script (`infra/get-ai-connection.ps1`) that automates steps 3–4 and prints the connection string for you.
