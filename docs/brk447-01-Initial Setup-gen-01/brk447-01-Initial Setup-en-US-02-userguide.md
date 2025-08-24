# Video: [brk447-01-Initial Setup.mp4](./REPLACE_WITH_VIDEO_LINK) — 00:03:02

# Local Deployment User Manual for Demo Models

This manual guides you through deploying two demo models locally, retrieving keys and endpoints, configuring the local application, and verifying functionality via the app dashboard and Azure Portal. Follow the step-by-step instructions and timestamps to replicate the demo.

---

## Overview
(00:00:00 — 00:00:12)

This demo shows how to:

- Deploy the demo resources and models into an Azure resource group
- Retrieve deployment outputs (endpoints, keys, foundry name)
- Configure a local application with those values (using a small script and user secrets)
- Run the app, supply a required AI Foundry parameter, and test semantic search
- Inspect the deployed Azure OpenAI resources and deployments in the Azure Portal

Required tools (install before starting):
- Visual Studio (for editing and running the app)
- Docker (if containerized components are used)
- PowerShell (or another terminal)
- .NET 9 SDK
- Azure CLI (az)

Tip: Ensure your local environment matches the demo requirements (Visual Studio and .NET version). Keep Azure credentials and keys secure — do not commit them into source control.

---

## Step-by-step Instructions

Follow these steps in order. Timestamps reference the demo video sections.

### 1. Sign in to Azure and pick the correct tenant/subscription
(00:00:13 — 00:00:27)

1. Open a terminal (PowerShell or your preferred shell).
2. Log in to Azure with the tenant option (to access the intended tenant):
   - Example:
     - az login --tenant <TENANT_ID>
3. Confirm you are in the correct subscription/tenant:
   - az account show
   - If needed, switch subscription:
     - az account set --subscription <SUBSCRIPTION_ID>

Warning: Logging into the wrong tenant or subscription may create resources in an unintended account. Double-check subscription details before proceeding.

---

### 2. Create the resource group and deploy demo resources
(00:00:27 — 00:00:46)

1. Run the provided deployment command or script to create a resource group and provision resources.
   - Example resource group name used in the demo: RGBR-CAR-447 (use your naming convention)
   - If you have a deployment script, run it from the repository root, e.g.:
     - ./deploy-demo-resources.ps1 <parameters>
   - If using an ARM/Bicep deployment with Azure CLI, it may look like:
     - az deployment group create --resource-group <RG_NAME> --template-file <template> --parameters <params>
2. Wait ~2 minutes for the deployment to complete.
3. Inspect the command output (typically JSON) for resource and deployment details.

Tip: The command will typically print JSON containing outputs such as endpoints, keys, and a foundry name. Save this output (or copy key fields) for the next steps.

---

### 3. Retrieve deployment outputs: keys, endpoints, and foundry name
(00:00:41 — 00:00:58)

1. Use the deployment output command to view outputs (examples):
   - az deployment group show --resource-group <RG_NAME> --name <DEPLOYMENT_NAME> --query properties.outputs
   - Or use the provided "get output" helper in the repository if available.
2. From the JSON output, copy these values:
   - Foundry name (used by the application)
   - Endpoint URL(s)
   - Key(s) or connection string(s)

Important: Copy the foundry name exactly (you will paste it into the local app’s configuration).

---

### 4. Update and run the local configuration script
(00:01:00 — 00:01:22)

1. Open the small local script provided in the repository (e.g., a PowerShell or shell script) that accepts resource names or accounts.
2. Populate the script with:
   - The resource group name you created
   - The account name or other variables retrieved from the deployment output
3. Copy the updated script contents into your console or save and run the script locally.
4. Clear the console if needed, then run the script.

The script will generate connection strings, endpoints, and keys for the local app to use.

Tip: Save the script in a secure place; it may include sensitive values. Do not commit secrets to source control.

---

### 5. Run the application and supply AI Foundry parameter
(00:01:22 — 00:02:05)

1. Launch the application (e.g., from Visual Studio or via dotnet run).
2. On first run:
   - The application dashboard will open in your browser automatically.
   - You will be prompted to enter an "AI Foundry" parameter (paste the foundry name you copied earlier).
3. Paste the foundry name into the prompt and choose the option to save it to user secrets (so you don’t need to re-enter it on subsequent runs).
   - If prompted to store it in user secrets, confirm/save.

Warning: The AI Foundry parameter and other keys are sensitive. Using user secrets (dotnet user-secrets) is recommended to avoid storing them in source files.

---

### 6. Verify application functionality: browse and semantic search
(00:02:05 — 00:02:14)

1. With the app running and the foundry parameter saved, browse the application UI.
2. Use the application’s semantic search feature to confirm:
   - Embeddings are working
   - Search results return meaningful results from the demo content

Tip: If the semantic search returns no results, ensure embeddings/deployments were provisioned correctly and that the app is pointing to the correct foundry endpoint.

---

### 7. Edit user secrets to switch resources or update configuration
(00:02:18 — 00:02:39)

1. To change which resource the app points to, edit the App host user secrets:
   - In Visual Studio: Right-click project → Manage User Secrets
   - Or use dotnet user-secrets commands:
     - dotnet user-secrets list
     - dotnet user-secrets set "AIFoundry" "<new-foundry-name>"
2. If an existing AI Foundry line exists, delete or update it:
   - Remove the old entry, then save/close.
3. Rerun the application and supply the new AI Foundry value if prompted.

Tip: Use dotnet user-secrets set to automate updating secrets from scripts or CI (for local development only).

---

### 8. Inspect resources and deployments in the Azure Portal
(00:02:39 — 00:03:00)

1. Open the Azure Portal (portal.azure.com).
2. Navigate to:
   - Resource Groups → select the resource group you created
   - Open the Azure OpenAI resource within the resource group
3. On the Azure OpenAI resource page:
   - View the list of deployments (examples from the demo: "gpt-41-mini" for chat/text + "adi-o2" for embeddings)
   - Select a deployment to view details
4. Copy endpoints and keys from the Azure Portal if needed:
   - Keys and Endpoint are available on the resource blade (or under Keys/Endpoint sections)

Warning: Portal keys are sensitive. Use them only in secure local configurations and rotate keys if they are ever exposed.

---

## Tips & Warnings (General)
- Always verify you are using the correct Azure tenant and subscription before creating resources.
- Store secrets in user secrets for local development; never commit them to your repository.
- Monitor deployment logs and JSON outputs for missing or failed resources.
- Allow 1–3 minutes for resource provisioning; larger deployments may take longer.
- Keep track of resource names (resource group, foundry name, deployment names) — you will need them during configuration.
- If something fails, check Azure deployment logs and the console output for detailed error messages.

---

Timestamps reference the demo video sections for quick navigation:
- Intro & Prerequisites: 00:00:00 — 00:00:12  
- Azure CLI Login / Tenant Selection: 00:00:13 — 00:00:27  
- Create Resource Group with Deployment Script: 00:00:27 — 00:00:46  
- Retrieve Outputs: Keys and Endpoints: 00:00:41 — 00:00:58  
- Update Script with Resource/Account Names: 00:01:00 — 00:01:22  
- Run Script => Obtain Connection String, Endpoint, Key; Dashboard Prompt: 00:01:22 — 00:02:05  
- Application Running — Browse & Semantic Search: 00:02:05 — 00:02:14  
- Editing User Secrets / Switching Resources: 00:02:18 — 00:02:39  
- Check Resource Group in Azure Portal & Inspect OpenAI Deployments: 00:02:39 — 00:03:00

This manual provides the actionable steps demonstrated in the video to reproduce the local demo deployment and verification.