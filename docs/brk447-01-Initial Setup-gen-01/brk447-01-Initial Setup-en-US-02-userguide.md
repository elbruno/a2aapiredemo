# Local Demo User Manual — AI Foundry Integration
This manual guides you through running the local demo shown in the video, from Azure login and resource creation to configuring the local application and verifying Azure OpenAI deployments.

- Total video reference: ~00:00:00 — 00:03:00
- Follow each step in order. Timestamps reference the video for the corresponding action.

---

## Overview
This demo shows how to:
- Log in to Azure using the correct tenant,
- Create and inspect an Azure resource group and deployment outputs,
- Use a local script to obtain a connection string (endpoint + key),
- Configure the application with the AI Foundry parameter (stored in user secrets),
- Start the app and validate semantic search / product browsing,
- Switch resources by editing user secrets and verify Azure OpenAI deployments in the Azure Portal.

Prerequisites (referenced in the video): Visual Studio, Docker, PowerShell, .NET 9, and Azure CLI. (See video 00:00:00 — 00:00:09)

---

## Step-by-step instructions

### 1) Log in to Azure (select correct tenant)
- Video ref: **00:00:13 — 00:00:31**

1. Open a terminal (PowerShell recommended).
2. Log in to Azure with the tenant option to pick the correct subscription/tenant:
   ```
   az login --tenant <TENANT_ID_OR_DOMAIN>
   ```
   - Replace `<TENANT_ID_OR_DOMAIN>` with the tenant used in the demo (for example the Cloud Advocate tenant).
3. If prompted, follow the browser flow to authenticate.
4. Confirm you are in the intended subscription:
   ```
   az account show -o table
   ```
   Tip: If the wrong subscription is active, switch with:
   ```
   az account set --subscription "<SUBSCRIPTION_NAME_OR_ID>"
   ```

Warning: Use the correct tenant to ensure resources are created in the expected subscription.

---

### 2) Create a resource group (example shown in demo)
- Video ref: **00:00:31 — 00:00:41**

1. Create a resource group (example name used in the demo: `RGBR-CAR-447`). Pick a location you need:
   ```
   az group create --name "RGBR-CAR-447" --location "eastus"
   ```
2. The command returns JSON output. Inspect it for resource identifiers.

Tip: Save deployment outputs or resource names — you'll need them for subsequent steps.

---

### 3) Retrieve deployment outputs: endpoints, keys, foundry name
- Video ref: **00:00:41 — 00:00:58**

1. If the deployment produced outputs (e.g., via an ARM/Bicep deployment), fetch them:
   ```
   az deployment group show --resource-group "RGBR-CAR-447" --name <DEPLOYMENT_NAME> --query properties.outputs -o json
   ```
   - Replace `<DEPLOYMENT_NAME>` with the deployment name used in your environment.
2. Inspect the JSON output to find:
   - AI Foundry/account name (copy this for later)
   - Endpoint(s)
   - Keys or other IDs

Tip: Use `-o json` to make copying values easy. In many demos the foundry/account name is a top-level output in the deployment results.

---

### 4) Update and run the local setup script to obtain connection string
- Video ref: **00:01:04 — 00:01:37**

1. Open the local script file used to create/retrieve the connection string (example: `scripts/GetConnection.ps1` or a similarly named script included with the demo).
2. Update the script variables with the resource group and account/foundry name you noted:
   Example PowerShell variables inside the script:
   ```powershell
   $resourceGroup = "RGBR-CAR-447"
   $accountName   = "<COPIED_FOUNDRY_NAME>"
   ```
3. Save the script changes.
4. Run the script in PowerShell. You can copy-paste the script contents into your console or run the file directly:
   ```
   pwsh .\scripts\GetConnection.ps1
   ```
   - After running you'll receive output showing the endpoint and key (the connection string).
5. Copy the returned endpoint and key for use in the app.

Tip: If the demo script prints a connection string (endpoint + key), copy it immediately. Store secrets securely — do not check them into source control.

---

### 5) Configure the AI Foundry parameter and start the application
- Video ref: **00:01:40 — 00:02:05**

1. Start the application (e.g., run from Visual Studio or `dotnet run` in the app host project).
2. On first run, the app dashboard will open and prompt for the AI Foundry parameter:
   - It notifies you that the AI Foundry parameter is missing (video 00:01:45 — 00:01:51).
3. Paste the foundry/account name or connection string into the dashboard input.
4. Save the value to user secrets so it persists and you will not be prompted again:
   - The dashboard may automatically call user secrets, or you can set it manually:
     ```
     dotnet user-secrets set "AiFoundry:Name" "<FOUNDY_NAME_OR_VALUE>"
     ```
5. Once configured, the app will start up and you can:
   - Browse products,
   - Run a semantic search to validate the integration (video 00:02:05).

Tip: Use `dotnet user-secrets list` to verify stored secrets:
```
dotnet user-secrets list
```

Warning: User secrets are local to your machine (development only) — do not treat them as production secret storage.

---

### 6) Switch resources (point the app to a different resource)
- Video ref: **00:02:18 — 00:02:39**

If you need to change which resource/account the app points to (for example to test another foundry or subscription):

1. Open the app host’s user secrets (edit the secrets configuration).
   - You can edit with the `dotnet user-secrets` CLI or open the `secrets.json` file the tool manages.
2. Remove or update the AI Foundry entry:
   - Remove:
     ```
     dotnet user-secrets remove "AiFoundry:Name"
     ```
   - Add/set:
     ```
     dotnet user-secrets set "AiFoundry:Name" "<NEW_FOUNDY_NAME_OR_VALUE>"
     ```
3. Restart the application so it picks up the changed value.

Tip: If you frequently swap between foundry accounts, keep a small script to set the `AiFoundry` secret programmatically.

---

### 7) Verify Azure OpenAI deployments and retrieve keys from Azure Portal
- Video ref: **00:02:39 — 00:02:57**

1. Open the Azure Portal (https://portal.azure.com).
2. Navigate to the Resource Group you created/used (e.g., `RGBR-CAR-447`).
3. Open the Azure OpenAI resource in that resource group.
4. Inspect the Deployments blade to confirm deployed models:
   - Example models shown in the demo: GPT-4o-mini (chat & text-embedding) and ADI O2 (embeddings).
5. Retrieve endpoint and keys from the resource:
   - In the Azure OpenAI resource blade, go to **Keys & Endpoint** (or the equivalent blade) to copy the endpoint and primary/secondary keys.

Warning: Treat these keys as sensitive secrets — rotate them periodically and never expose them in public repositories.

---

## Tips & Warnings (inline collection)
- Tip: Keep a copy of critical values (resource group name, foundry/account name, endpoint, key) in a secure place during setup.
- Tip: If the app prompts for the AI Foundry parameter, you can use the dashboard input or the `dotnet user-secrets` CLI to persist the value.
- Warning: Never commit keys or connection strings to source control.
- Warning: Ensure Docker is running (if the app or demo requires containers) and .NET 9 is installed before running the demo app.

---

If you need a quick-reference checklist:
- [ ] Confirm prerequisites installed: Visual Studio, Docker, PowerShell, .NET 9, Azure CLI.
- [ ] az login --tenant <tenant>
- [ ] az group create --name <RG> --location <LOC>
- [ ] Retrieve deployment outputs (endpoint, keys, foundry name)
- [ ] Edit local script with <RG> and <foundry>, run to get connection string
- [ ] Start app, paste AI Foundry parameter into dashboard, save to user secrets
- [ ] Verify app is running and test semantic search
- [ ] If needed, change user secrets to point to another resource and verify Azure OpenAI deployments in Portal

Reference timestamps: follow along in the video for each major action:
- Intro & prerequisites: **00:00:00 — 00:00:09**
- Azure CLI login & create RG: **00:00:09 — 00:00:41**
- Retrieve outputs: **00:00:41 — 00:00:58**
- Update/run local script: **00:00:58 — 00:01:40**
- Configure AI Foundry & start app: **00:01:40 — 00:02:14**
- Switch resources & verify OpenAI deployments: **00:02:18 — 00:03:00**

This manual should allow you to reproduce the demo setup and validate the AI Foundry + Azure OpenAI integration locally.