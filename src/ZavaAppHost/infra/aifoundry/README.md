
# AI Foundry (Cognitive Services) module

This module configures an Azure Cognitive Services (OpenAI) account used as the "AI Foundry" for local development and demos.

Change made: `disableLocalAuth` was set to `false` to enable key-based (local) authentication. The Products project in this repo currently expects key-based access for embeddings and returned an HTTP 403 with `AuthenticationTypeDisabled` when keys were disabled.

Notes and recommended next steps:

- To apply the change, redeploy the Bicep templates from the repository root (for example using `az deployment sub create` or via your CI):

  ```pwsh
  az deployment sub create --location <location> --template-file src\ZavaAppHost\infra\main.bicep --parameters environmentName=<env> location=<location> principalId=<principalId> sql_password=<pw>
  ```

- After deployment the Cognitive Services resource will allow keys to be created. You can fetch keys using:

  ```pwsh
  az cognitiveservices account keys list -g rg-<env> -n <accountName>
  ```

- For production scenarios, prefer `disableLocalAuth: true` and use AAD + managed identities to authenticate. Update calling services to use the managed identity instead of keys.

If you need help switching the Products service to use managed identity authentication instead, I can update the code and deployment templates to wire that up.
