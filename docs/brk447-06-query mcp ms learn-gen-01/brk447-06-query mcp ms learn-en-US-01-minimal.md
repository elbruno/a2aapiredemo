1. Open the project and review the products list, DbContext, and current DB initializer implementation. [00:00:01.840]  
2. Enable GitHub Copilot agent mode and select Microsoft Docs as a tool. [00:00:23.520]  
3. Ask the agent to search Microsoft Docs for DB initializer patterns for EF Core 9 and to answer based on the full doc content. [00:00:31.560]  
4. Read the permissions prompt and choose "Allow only this time" to grant the agent tool access for the session. [00:01:02.360]  
5. Wait for the agent to run and analyze the documentation and project files. [00:01:24.960]  
6. Review the agent's recommendations noting that EF Core 9 prefers a migration-based initializer approach and includes reference links. [00:01:29.888]  
7. Request the agent to implement the recommended code changes in the project. [00:02:09.320]  
8. Start the new agent session and select the desired model (e.g., GPT-4.1 or GPT-5). [00:02:12.400]  
9. Let the agent read Program.cs, DbContext, and related files to prepare the edits. [00:02:25.738]  
10. Review the agent's code-change preview that refactors initialization to use Database.MigrateAsync (or Migrate) instead of EnsureCreated/EnsureCreatedAsync. [00:02:41.122]  
11. Apply and commit the agent's changes, ensuring conditional sync/async handling and improved sample-data seeding are included. [00:03:44.720]

Related guides:

- [Full user manual](./brk447-06-query mcp ms learn-en-US-02-userguide.md)
- [User manual with snapshots](./brk447-06-query mcp ms learn-en-US-03-snapshots.md)
