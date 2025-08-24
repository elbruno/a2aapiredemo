# Video: [brk447-01-Initial Setup.mp4](./REPLACE_WITH_VIDEO_LINK) — 00:03:02

1. Log in to Azure using az login with the tenant option and select the correct subscription. [00:00:13.360]  
2. Run the deployment command to create the resource group and wait ~2 minutes for the provisioning to complete. [00:00:27.640]  
3. Inspect the JSON output and use the CLI get-output command to retrieve keys, endpoints, and the foundry name. [00:00:41.720]  
4. Edit the local script to insert the retrieved resource group and account names, then paste the updated script into the console. [00:01:00.200]  
5. Execute the script to generate the connection string, endpoint, and key; when the dashboard opens paste the AI Foundry parameter and save it to user secrets. [00:01:22.000]  
6. With the parameter saved and the app running, browse the product UI and perform a semantic search to verify functionality. [00:02:05.149]  
7. To switch resources, open the app host user secrets, remove the existing AI Foundry entry, rerun the app, and add the new AI Foundry value. [00:02:18.160]  
8. Open the Azure Portal, navigate to the created resource group, open the Azure OpenAI resource, inspect deployments (e.g., model names), and copy endpoints and keys as needed. [00:02:39.848]

Related guides:

- [Full user manual](./brk447-01-Initial Setup-en-US-02-userguide.md)
- [User manual with snapshots](./brk447-01-Initial Setup-en-US-03-snapshots.md)
