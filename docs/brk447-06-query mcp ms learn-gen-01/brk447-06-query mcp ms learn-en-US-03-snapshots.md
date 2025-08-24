# DB Initializer Migration with GitHub Copilot Agent — User Manual

This manual walks you through using a GitHub Copilot Agent to review and update an existing Entity Framework Core (EF Core) DB initializer to align with EF Core 9 recommendations (migrations and Database.MigrateAsync). It covers enabling the agent, searching Microsoft Docs, granting tool permissions, reviewing agent recommendations, triggering an edit session, selecting an agent model, and applying the agent’s code changes.

- Total demo duration referenced: 00:03:44.720

---

## Overview

This workflow demonstrates how to use a GitHub Copilot Agent to:

- Inspect an existing project (DbContext, DB initializer, product list).
- Search Microsoft Docs for EF Core 9 guidance.
- Receive recommendations to migrate from EnsureCreated/EnsureCreatedAsync to Database.MigrateAsync (or Migrate) and improved seeding patterns.
- Instruct the agent to implement the recommended code changes.
- Review and apply the agent’s patch (with conditional async/sync support and better seeding).

Why use this approach?
- EF Core 9 documentation recommends using migrations and Database.MigrateAsync for schema evolution when you use migrations. Using an agent to consult official docs reduces guesswork and speeds up safe refactoring.

---

## Step-by-step instructions

Follow these steps to reproduce the workflow shown in the video.

1. Review the current DB initializer and related files (00:00:01.840–00:00:17.920)
   - Open your project in your IDE and inspect:
     - The products list / seed data class
     - Your DbContext implementation
     - The current DB initializer class (look for Use of EnsureCreated or EnsureCreatedAsync)
   - Tip: Note whether your code uses migrations or EnsureCreated. If you plan to use migrations in production, prefer Database.MigrateAsync.

   ![Project files open — review DbContext and initializer (00:00:01.840)](./%23)

2. Enable GitHub Copilot Agent mode and configure Microsoft Docs tool (00:00:23.520–00:00:51.080)
   - In your Copilot/IDE integration, enable "Copilot Agent" mode.
   - Add or enable the Microsoft Docs tool so the agent can search official documentation.
   - Ask the agent to search Microsoft Docs for "DB initializer patterns EF Core 9" and to answer based on full doc content.
     - Example prompt: "Search Microsoft Docs for EF Core 9 DB initializer recommendations and provide guidance for moving from EnsureCreated to migrations."

   ![Enable Copilot Agent and select Microsoft Docs (00:00:23.520)](./%23)

   Tip: Selecting Microsoft Docs ensures recommendations reference authoritative, up-to-date guidance.

3. Grant the agent permission to use tools (00:00:57.240–00:01:14.800)
   - A permission dialog will appear when the agent tries to access external tools or run commands.
   - Choose the permission scope:
     - "Allow only this time" — safer for demos / single sessions (recommended for testing).
     - "Always allow" — convenient but grants persistent access; use only if you trust the environment.
   - For the demo, "Allow only this time" was selected.

   ![Permissions dialog — allow tools to access disk/run commands (00:00:57.240)](./%23)

   Warning: Granting agents the ability to run commands or access disk can alter your repository. Only allow what you trust.

4. Wait for the agent to analyze docs and project files; review recommendations (00:01:24.960–00:01:58.680)
   - The agent will process the documentation and scan your project. This can take a few minutes depending on the tools and project size.
   - Review the returned recommendations. Typical recommendations for EF Core 9:
     - Current initializer may work, but EF Core 9 recommends using migrations.
     - Use Database.MigrateAsync (async) or Database.Migrate (sync) when applying migrations programmatically.
     - Improve seeding practices and conditionally handle async vs sync call paths.

   ![Agent recommendations and reference links (00:01:24.960)](./%23)

   Tip: Open the documentation links the agent provides to verify the rationale and any edge cases that might affect your app.

5. Request the agent to implement recommended changes and start a new session (00:02:09.320–00:02:17.320)
   - Instruct the agent, for example: "Please implement the recommended DB initializer changes based on EF Core 9 docs and my project files."
   - The UI will create a new agent session tied to a chosen model (select from options such as GPT-4.1 or GPT-5).
   - Choose the model appropriate for your needs:
     - GPT-4.1: Stable and reliable for code changes.
     - GPT-5: May provide more advanced reasoning if available.

   ![Start new agent session & choose model (00:02:09.320)](./%23)

   Tip: Picking a more capable model can improve the quality of code edits, but it may cost more or take longer.

6. Let the agent read relevant files and prepare edits (00:02:25.738–00:02:41.122)
   - The agent reads Program.cs, your DbContext, the initializer class, and other relevant files to build a safe patch plan.
   - Wait for the agent to finish analysis before accepting edits.

   ![Agent reading Program.cs and DbContext (00:02:25.738)](./%23)

7. Review the agent's code changes and recommendations (00:02:41.122–00:03:44.720)
   - The agent will modify classes to implement the recommended approach:
     - Replace EnsureCreated/EnsureCreatedAsync with Database.Migrate or Database.MigrateAsync, depending on sync/async context.
     - Add conditional handling so the initializer can be called from sync or async application startup flows.
     - Improve sample-data seeding to avoid duplicate inserts and to use context-aware seeding.
   - Inspect the patch preview panel carefully. Verify:
     - The migrations call is used (Migrate/MigrateAsync).
     - Seeding code uses checks (e.g., AnyAsync/Any) before adding seed data.
     - Transactions or retries are included if needed in your environment.

   ![Code changes/patch preview — MigrateAsync and updated seeding (00:02:41.122)](./%23)

   Tip: If your app uses an async startup pipeline (e.g., WebApplication.CreateBuilder/Build async flows), prefer MigrateAsync to avoid blocking threads.

8. Apply or commit the agent’s changes
   - After reviewing, use the "Apply/commit changes" button in the agent UI to accept the changes into your repository.
   - Run your test suite or start the application locally to validate that migrations and seeding execute correctly.
   - If you use CI/CD, run migrations in a controlled manner (see warning below).

   ![Apply/commit changes button visible in UI (00:03:07.970)](./%23)

   Warning: Running Database.Migrate (or MigrateAsync) will apply pending migrations to the target database. Always:
   - Back up production data before applying migrations.
   - Test migrations on staging environments prior to production.

---

## Best practices, tips & warnings

- Always back up databases before running migrations in production.
- Prefer Database.MigrateAsync in async application startup paths; use Database.Migrate in synchronous paths.
- Use seed idempotency patterns:
  - Check for existing records (Any / AnyAsync) before inserting.
  - Consider unique constraints that will prevent duplicate data.
- When granting agent permissions, choose the least-privileged option for your use case ("Allow only this time" for experimentation).
- Review all agent-suggested code changes manually before committing.

---

## Snapshots/images (inline references)

The following inline snapshot placeholders correspond to the key UI states mentioned above and are referenced at the relevant steps. Replace each placeholder with the extracted frame for documentation or tutorials.

- Project files open — review DbContext and initializer: ![Snapshot at 00:00:01.840](./%23)
- Enable Copilot Agent and select Microsoft Docs: ![Snapshot at 00:00:23.520](./%23)
- Permissions dialog — allow tools to access disk/run commands: ![Snapshot at 00:00:57.240](./%23)
- Agent recommendations and reference links: ![Snapshot at 00:01:24.960](./%23)
- Start new agent session & choose model: ![Snapshot at 00:02:09.320](./%23)
- Agent reading Program.cs and DbContext: ![Snapshot at 00:02:25.738](./%23)
- Code changes/patch preview — MigrateAsync and updated seeding: ![Snapshot at 00:02:41.122](./%23)
- Apply/commit changes button visible in UI: ![Snapshot at 00:03:07.970](./%23)
- Agent session finished indicator / final status: ![Snapshot at 00:03:44.720](./%23)

---

## Snapshots

[00:00:01.840]  
[00:00:23.520]  
[00:00:57.240]  
[00:01:24.960]  
[00:02:09.320]  
[00:02:25.738]  
[00:02:41.122]  
[00:03:07.970]  
[00:03:44.720]