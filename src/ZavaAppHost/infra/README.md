# Infra deployment notes

This folder contains Bicep modules and Container Apps templates for deploying the Zava AppHost environment.

Change made for demo compatibility:

- The AI Foundry Cognitive Services module (`aifoundry/aifoundry.module.bicep`) has `disableLocalAuth` set to `false` so key-based authentication is available for demos and local development. This enables existing code that expects an API key to call the embeddings/chat deployments.

Templates:

- `products.tmpl.yaml` now includes an optional secret `ai-key` which will be injected into the `products` container as `AI_Key` when present. This is intended for demo/dev deployments where key-based auth is used.

Security recommendation:

- For production, set `disableLocalAuth: true` in `aifoundry.module.bicep` and update services to use Azure AD (managed identity) instead of keys. I can help implement the managed identity flow end-to-end if you want.

Redeploy example (subscription deployment):

```pwsh
az deployment sub create --location <location> --template-file src\ZavaAppHost\infra\main.bicep --parameters environmentName=<env> location=<location> principalId=<principalId> sql_password=<pw>
```

After deploy, retrieve keys (if using key auth):

```pwsh
az cognitiveservices account keys list -g rg-<env> -n <accountName>
```

If you'd rather switch everything to managed identity now, tell me and I will prepare the Bicep and code updates.
