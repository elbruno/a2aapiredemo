# Infra deployment notes

This folder contains the minimal Bicep files to deploy the AI Foundry (Cognitive Services OpenAI) account for local demos.

Optional role assignment

The main Bicep deployment intentionally does not perform an automatic role assignment to a principal. If you need to grant a user or service principal the built-in Cognitive Services OpenAI role used by the demo, run this separate command after deployment:

```pwsh
# Replace with your principalId (object id of user or service principal), resource group and account name
$principalId = '<principalId>'
$rg = 'rg-brk447demo'
$accountName = '<accountName>'

# role definition id for Cognitive Services OpenAI user
$roleDefinitionId = '/subscriptions/<subscriptionId>/providers/Microsoft.Authorization/roleDefinitions/5e0bd9bd-7b93-4f28-af87-19fc36ad61bd'

az role assignment create --assignee-object-id $principalId --role $roleDefinitionId --scope "/subscriptions/$(az account show --query id -o tsv)/resourceGroups/$rg/providers/Microsoft.CognitiveServices/accounts/$accountName"
```

This avoids conditional module invocation issues in Bicep and gives you explicit control over who receives the role.
