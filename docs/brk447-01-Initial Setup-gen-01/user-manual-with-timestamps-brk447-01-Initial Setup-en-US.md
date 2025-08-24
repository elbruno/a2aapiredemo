# AI Foundry Local Demo — User Manual

This manual guides you through the exact steps shown in the demo to run a local application that integrates with Azure AI Foundry and Azure OpenAI resources. Follow the steps in order to reproduce the setup, configure the app, and verify deployed models.

---

## Overview

This demo shows how to:
- Log into Azure with the correct tenant and create a resource group.
- Retrieve deployment outputs (keys, endpoints, foundry/account name).
- Update and run a local script to obtain a connection string and keys.
- Configure the application (store the AI Foundry parameter in user secrets) and run the app.
- Switch which Azure resource the app points to and verify Azure OpenAI deployments in the Azure Portal.

Total demo runtime: ~3 minutes. Key timestamps are included throughout for reference and snapshots.

---

## Step-by-step Instructions

Prerequisite reminder: Ensure the following are installed and available before starting:
- Visual Studio (or another IDE compatible with .NET)
- Docker (running)
- PowerShell (or your preferred shell)
- .NET 9 SDK
- Azure CLI (az)

### 1. Azure CLI: Sign in to the correct tenant (00:00:13.360)
1. Open a terminal or PowerShell.
2. Sign in to Azure and specify the tenant used by the demo environment:
   - Example:
     az login --tenant <TENANT_ID>
   - Replace `<TENANT_ID>` with the demo tenant (Cloud Advocate tenant in the walkthrough).
3. Confirm you are in the expected subscription/tenant before proceeding.

Tip: Use az account show to confirm the currently active subscription and tenant.

Snapshot: Azure CLI login screen (00:00:13.360)  
![Azure CLI login - 00:00:13.360](./snapshot_00_00_13.360.png)

Warning: Signing into the wrong tenant will cause resource creation and lookups to fail.

---

### 2. Create a Resource Group (00:00:31.316)
1. Create a resource group for the demo resources:
   - Example:
     az group create --name "RGBR-CAR-447" --location "eastus"
   - Replace name and location as needed.
2. Inspect the JSON output. The command returns resource metadata (IDs, locations).

Snapshot: Resource group creation command output (00:00:31.316)  
![Resource group creation - 00:00:31.316](./snapshot_00_00_31.316.png)

Tip: Keep the resource group name handy — you will need it for local scripts and configuration.

---

### 3. Retrieve Deployment Outputs (keys, endpoints, foundry name) (00:00:41.720)
1. Run the CLI command used to retrieve the deployment outputs. Depending on your deployment, this might be:
   - az deployment group show --name <deployment-name> --resource-group <rg> --query properties.outputs
   - Or another CLI command used by your deployment tooling that prints outputs.
2. Locate and copy the foundry/account name from the output. Also note endpoint and key values shown.

Snapshot: CLI output showing keys, endpoint, and foundry name (00:00:41.720)  
![Deployment outputs - 00:00:41.720](./snapshot_00_00_41.720.png)

Warning: Treat keys and endpoints as secrets. Do not commit them to source control.

---

### 4. Update and Run Local Setup Script to Obtain Connection String (00:00:58.409 → 00:01:37.120)
1. Open the local script file used by the demo (the script contains placeholders for resource group and account/foundry).
2. Edit the script to set the resource group and account/foundry values you obtained earlier.
   - Example edit lines:
     - RESOURCE_GROUP="RGBR-CAR-447"
     - FOUNDRY_NAME="<your-foundry>"
3. Copy the updated script into your console and run it.
   - The script will provision or retrieve a connection string, and will output an endpoint and key.
4. Note the connection string and the key in the script output.

Snapshots:
- Open script and edit resource/account values (00:00:58.409 / 00:01:04.000)  
  ![Edit local script - 00:01:04.000](./snapshot_00_01_04.000.png)
- Copy script into console and run (00:01:30.200 / 00:01:32.000)  
  ![Run local setup script - 00:01:32.000](./snapshot_00_01_32.000.png)
- Output showing endpoints and key (00:01:37.120)  
  ![Script output - 00:01:37.120](./snapshot_00_01_37.120.png)

Tip: If the script provisions resources, allow a few moments for operations to complete. Watch the console for returned values.

---

### 5. Configure AI Foundry Parameter and Start the Application (00:01:40.996 → 00:02:05.149)
1. Start the application (for example, via Visual Studio or dotnet run). On first run the app opens a dashboard and prompts for the AI Foundry parameter.
2. In the dashboard, paste the foundry/account name you copied earlier into the AI Foundry input field.
   - Dashboard indicates missing AI Foundry parameter when not set (00:01:45.590 / 00:01:51.045).
3. Save the parameter to user secrets so the app won’t prompt again:
   - The dashboard may offer a "Save to user secrets" action.
   - Alternatively, set it manually using dotnet user-secrets:
     - dotnet user-secrets set "AIFoundry:AccountName" "<your-foundry-value>"
4. After saving, the application should start fully and the UI will be available.
5. Test the application:
   - Browse the product UI.
   - Run a semantic search to validate the AI Foundry / Azure OpenAI integration.

Snapshots:
- Dashboard requesting AI Foundry parameter (00:01:45.590)  
  ![Dashboard missing AI Foundry - 00:01:45.590](./snapshot_00_01_45.590.png)
- Paste foundry value and save to user secrets (00:01:56.960)  
  ![Save AI Foundry to user secrets - 00:01:56.960](./snapshot_00_01_56.960.png)
- App up and running (00:02:05.149)  
  ![App running - 00:02:05.149](./snapshot_00_02_05.149.png)

Tip: If the dashboard does not accept the value, confirm the exact foundry string (copy/paste) and that the user secrets store is bound to the running app environment.

Warning: Do not share user secrets or keys. Use secure storage for production.

---

### 6. Switch Resources and Verify Azure OpenAI Deployments (00:02:18.160 → 00:02:57.280)
1. To change which Azure resource the app points to:
   - Open the app host's user secrets (the app's secrets store).
   - Delete the AI Foundry entry, then add the new value for the other resource.
   - Example: remove "AIFoundry:AccountName" and set to a new account.
2. Alternatively, update the same field via the dashboard and save to user secrets again (as done previously).
3. Open the Azure Portal and navigate to the resource group you created earlier.
   - Open the Azure OpenAI resource and view the deployments blade.
4. Confirm the deployed models:
   - Chat deployment (e.g., gpt-4o-mini-chat) and embedding model (e.g., ADI O2 / text-embedding-ada variants) should be listed.
5. Retrieve the endpoint and keys from the Azure Portal if needed:
   - From the Azure OpenAI resource, open "Keys and Endpoint" to copy the endpoint and keys.

Snapshots:
- Open APP host user secrets (00:02:23.377)  
  ![Open user secrets - 00:02:23.377](./snapshot_00_02_23.377.png)
- Locate AI Foundry entry in user secrets (00:02:31.440)  
  ![AI Foundry entry - 00:02:31.440](./snapshot_00_02_31.440.png)
- Open Resource Group in Azure Portal (00:02:39.848)  
  ![Azure Portal Resource Group - 00:02:39.848](./snapshot_00_02_39.848.png)
- View Azure OpenAI deployments (00:02:49.138)  
  ![Azure OpenAI deployments - 00:02:49.138](./snapshot_00_02_49.138.png)
- Retrieve endpoint and key values from portal (00:02:57.280)  
  ![Azure OpenAI Keys and Endpoint - 00:02:57.280](./snapshot_00_02_57.280.png)

Tip: When pointing to a different resource, ensure that resource has the required OpenAI model deployments and that its keys/endpoints are valid.

Warning: Changing the resource will alter which models and data the app uses. Double-check which resource you are switching to.

---

## Snapshots (inline images)

The images embedded above are placeholders referencing the frames you should capture. Each corresponds to a key moment in the demo (login, resource creation, script edit & run, dashboard prompts, user secrets, and Azure Portal verifications). Replace the placeholder image filenames with the frames extracted at the matching timestamps.

---

## Snapshots

[00:00:13.360]  
[00:00:31.316]  
[00:00:41.720]  
[00:00:58.409]  
[00:01:04.000]  
[00:01:30.200]  
[00:01:32.000]  
[00:01:37.120]  
[00:01:45.590]  
[00:01:56.960]  
[00:02:05.149]  
[00:02:23.377]  
[00:02:31.440]  
[00:02:39.848]  
[00:02:49.138]  
[00:02:57.280]