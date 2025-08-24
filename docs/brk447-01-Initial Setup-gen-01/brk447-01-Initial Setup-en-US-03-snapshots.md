# Video: [brk447-01-Initial Setup.mp4](./REPLACE_WITH_VIDEO_LINK) — 00:03:02

# Deploying Demo Models Locally — User Manual

This manual walks you through deploying two demo models locally, retrieving the deployment outputs (endpoints, keys, foundry name), configuring the local application, and verifying functionality (including semantic search). Follow the steps in order and use the provided timestamps to reference the corresponding video views.

---

## Overview

This guide covers:
- Logging into Azure and selecting the correct tenant/subscription
- Running the deployment to create a resource group and provision resources
- Retrieving deployment outputs (connection string, endpoint, keys, foundry name)
- Updating and running a local script to populate app configuration and save secrets
- Running the application dashboard, entering the AI Foundry parameter, and testing semantic search
- Editing user secrets to switch the app to a different resource
- Inspecting the created resources and OpenAI deployments in the Azure Portal

Estimated video run-through duration: ~3 minutes (00:00:00.360 — 00:03:00.480).

---

## Step-by-step instructions

Note: Replace placeholder names in angle brackets (<...>) with your actual values.

### 1) Prerequisites (brief)
- Visual Studio
- Docker
- PowerShell
- .NET 9 (SDK/runtime)
- Azure CLI
These are the same prerequisite tools demonstrated in the video (00:00:00.360 — 00:00:12.480).

Snapshot: see [00:00:05.000] for the tools list.

---

### 2) Log in to Azure and select the correct tenant/subscription (00:00:13 — 00:00:27)

1. Open a terminal or PowerShell window.
2. Log in to Azure using the Cloud Advocate tenant (or your target tenant):

   ```
   az login --tenant <TENANT_ID>
   ```

3. Verify and set the target subscription (if required):

   ```
   az account show
   az account set --subscription <SUBSCRIPTION_ID>
   ```

Tip: Ensure you are in the Cloud Advocate tenant (or the tenant shown in the demo) before deploying resources. Snapshot: [00:00:20.000]

Warning: Running commands in the wrong subscription or tenant can create resources you do not intend to provision.

---

### 3) Create the resource group and run the deployment script (00:00:27 — 00:00:46)

The demo runs a provided deployment command or script that creates a resource group and provisions the necessary resources (OpenAI resource, storage, etc.). In the video they create an example resource group (like `RGBR-CAR-447`).

1. From the CLI, run the provided deployment command/script. Example (replace with the actual script/parameters you have):

   ```
   ./deploy-resources.sh --resource-group <RESOURCE_GROUP_NAME> --location <LOCATION>
   ```

   Or using az deployment (example):

   ```
   az deployment group create \
     --resource-group <RESOURCE_GROUP_NAME> \
     --template-file ./arm-template.json \
     --parameters @parameters.json
   ```

2. Wait ~2 minutes for the deployment to complete. The CLI will output JSON describing the deployed resources. (00:00:27 — 00:00:46)

Snapshot: the JSON output appears in the console. Capture [00:00:33.000] and [00:00:45.000].

---

### 4) Retrieve deployment outputs: keys, endpoint, foundry name (00:00:41.720 — 00:00:58)

1. Use the deployment output query to inspect the outputs. Example (replace deployment name and resource group):

   ```
   az deployment group show \
     --name <DEPLOYMENT_NAME> \
     --resource-group <RESOURCE_GROUP_NAME> \
     --query properties.outputs
   ```

2. In the JSON output, locate:
   - keys (API key value)
   - endpoint (OpenAI endpoint)
   - foundryName (AI Foundry/instance name)

3. Copy the foundry name and endpoint for use in the local script and configuration.

Tip: The CLI JSON output lists each output with its name and value; copy them into a secure store. Snapshot: [00:00:50.000]

---

### 5) Update the local script with resource & account names (00:01:00 — 00:01:22)

1. Open the small script or console app provided with the demo (this script prepares connection strings and local configuration).
2. Populate the script variables with:
   - Resource group name (`<RESOURCE_GROUP_NAME>`)
   - Account/foundry name (`<FOUNDRY_NAME>` or `<ACCOUNT_NAME>`)
   - Any other parameters required (endpoint, region, etc.)

Example snippet in the script (pseudo):

```powershell
$resourceGroup = "<RESOURCE_GROUP_NAME>"
$accountName   = "<ACCOUNT_NAME>"   # or foundry name
```

3. Copy the updated script into your console or run it from the project.

Snapshot: the editor/console with pasted values is shown at [00:01:10.000].

Warning: Ensure no secrets (keys) are committed to source control—use user secrets or environment variables instead.

---

### 6) Run the script to obtain connection string, endpoint, key (and respond to dashboard prompt) (00:01:22 — 00:02:05)

1. Execute the script. It will return:
   - Connection string
   - Endpoint
   - API key(s)

2. On first run, the local application dashboard will open automatically and prompt for an AI Foundry parameter (a single string value). Paste the foundry parameter into the prompted field in the dashboard.

3. Save this AI Foundry parameter to user secrets when prompted so you will not need to paste it on subsequent runs.

Example command to set a user secret via dotnet CLI (if you want to set manually):

```
dotnet user-secrets set "AI_FOUNDRY" "<AI_FOUNDRY_PARAM>"
```

Or to remove:

```
dotnet user-secrets remove "AI_FOUNDRY"
```

Tip: Saving the AI Foundry parameter to user secrets prevents repeated prompting and keeps the secret local to your developer environment.

Snapshot: dashboard prompt to paste AI Foundry param at [00:01:30.000] and confirmation of saving to user secrets at [00:01:50.000].

---

### 7) Application running: browse and test semantic search (00:02:05 — 00:02:14)

1. With the parameter saved and the app running, open the application UI in your browser (the dashboard opened automatically earlier).
2. Browse the product pages and items to verify the app is working.
3. Perform a semantic search in the UI to confirm embeddings and search functionality are operational.

Tip: If semantic search returns expected results, embeddings and model endpoints are configured correctly.

Snapshot: app UI and semantic search demonstration at [00:02:08.000].

---

### 8) Editing user secrets to switch resources (00:02:18 — 00:02:39)

To point the app to a different resource/foundry:

1. Open the App host user secrets file (via Visual Studio or the dotnet user-secrets CLI).
2. Delete the existing AI Foundry line or change its value:

   ```
   dotnet user-secrets remove "AI_FOUNDRY"
   dotnet user-secrets set "AI_FOUNDRY" "<NEW_AI_FOUNDRY_PARAM>"
   ```

3. Rerun the application. If the AI Foundry value is missing, the dashboard will prompt for it again — paste the new value and save.

Snapshot: editing user secrets and deleting the AI Foundry entry at [00:02:25.000].

Warning: Only store local developer secrets in user-secrets. For production, use a secure secrets store (Key Vault).

---

### 9) Verify in Azure Portal & inspect OpenAI deployments (00:02:39 — 00:03:00)

1. Open the Azure Portal (https://portal.azure.com).
2. Navigate to Resource Groups, and select the resource group you created.
3. Open the Azure OpenAI resource in that group.
4. Inspect the "Deployments" list — you'll see model deployments (example names from the demo: `gpt-41-mini` for chat/text and `adi-o2` for embeddings).
5. From the OpenAI resource page, copy the endpoint and keys as needed (for cross-checking with CLI outputs).

Snapshot: Azure Portal Resource Group blade at [00:02:45.000] and OpenAI deployments list at [00:02:50.000].

Tip: If you need to regenerate keys, do it from the OpenAI resource's Keys section in the portal, then update your local secrets.

---

## Inline snapshots / image placeholders

(These images are placeholders showing key UI states from the video. Replace with actual images captured at the timestamps listed in the Snapshots section.)

- Tools and prerequisites list (00:00:05):  
  ![Tools and prerequisites - Visual Studio, Docker, PowerShell, .NET 9, Azure CLI](./snapshot_00-00-05.png)

- Azure CLI tenant login (00:00:20):  
  ![Azure CLI: az login with tenant option](./snapshot_00-00-20.png)

- Deployment JSON output in CLI (00:00:33 & 00:00:45):  
  ![Deployment command output (JSON) - resource group creation](./snapshot_00-00-33.png)  
  ![Deployment output JSON showing resources and outputs](./snapshot_00-00-45.png)

- Retrieving outputs: foundry, endpoint, keys (00:00:50):  
  ![CLI output showing keys, endpoint, foundry name](./snapshot_00-00-50.png)

- Script edited with resource and account names before running (00:01:10):  
  ![Local script populated with resource/group and account names](./snapshot_00-01-10.png)

- Application dashboard prompting for AI Foundry value (00:01:30):  
  ![App dashboard prompt to paste AI Foundry parameter](./snapshot_00-01-30.png)

- Confirmation of saving value to user secrets (00:01:50):  
  ![User secrets saved confirmation or UI showing saved secret](./snapshot_00-01-50.png)

- App UI browsing and semantic search results (00:02:08):  
  ![Application browsing UI and semantic search demo](./snapshot_00-02-08.png)

- Editing user secrets to switch resources (00:02:25):  
  ![App host user secrets being edited; AI Foundry entry removed](./snapshot_00-02-25.png)

- Azure Portal: resource group and OpenAI deployments (00:02:45 & 00:02:50):  
  ![Azure Portal resource group blade showing created resources](./snapshot_00-02-45.png)  
  ![Azure OpenAI resource deployment list (gpt-41-mini, adi-o2)](./snapshot_00-02-50.png)

---

## Snapshots

Use these timestamps to capture frames for the inline images above. Each timestamp corresponds to a meaningful UI state shown in the video.

[00:00:05.000]  
[00:00:20.000]  
[00:00:33.000]  
[00:00:45.000]  
[00:00:50.000]  
[00:01:10.000]  
[00:01:30.000]  
[00:01:50.000]  
[00:02:08.000]  
[00:02:25.000]  
[00:02:45.000]  
[00:02:50.000]