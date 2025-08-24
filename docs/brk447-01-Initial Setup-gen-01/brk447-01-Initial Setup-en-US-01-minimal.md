1. Install and verify prerequisites: Visual Studio, Docker, PowerShell, .NET 9, and Azure CLI. [00:00:00.360]

2. Log into Azure CLI with the tenant option to select the correct subscription (az login --tenant ...). [00:00:13.360]

3. Create a new resource group with az group create and note the returned JSON with resource IDs. [00:00:31.316]

4. Run the CLI command to retrieve deployment outputs and copy the foundry/account name from the output. [00:00:41.720]

5. Edit the local setup script to insert the resource group and account/foundry name, then paste the updated script into the console. [00:01:04.000]

6. Execute the script to provision or obtain the connection string, endpoint, and key, and capture the terminal output. [00:01:32.000]

7. Paste the AI Foundry parameter into the dashboard input and save it to user secrets, then confirm the application starts. [00:01:56.960]

8. To point the app to a different resource, open the app host user secrets and delete or replace the AI Foundry entry, then save. [00:02:23.377]

9. Open the Resource Group in the Azure Portal, view the Azure OpenAI resource deployments, and copy the endpoint and key values from the portal. [00:02:39.848]

Related guides:

- [Full user manual](./brk447-01-Initial Setup-en-US-02-userguide.md)
- [User manual with snapshots](./brk447-01-Initial Setup-en-US-03-snapshots.md)
