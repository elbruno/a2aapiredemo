# Video: [brk447-06-query mcp ms learn.mp4](./REPLACE_WITH_VIDEO_LINK) — 00:03:45

# DB Initializer with GitHub Copilot Agent — User Manual

This manual guides you through using GitHub Copilot agent mode to review and update a database initializer for an Entity Framework Core 9 (EF Core 9) project, as demonstrated in the analyzed video sections.

- Total demo duration: 00:03:44.720
- Primary goals: consult Microsoft Docs with an agent, let the agent analyze project code, and implement EF Core 9–aligned DB initialization (notably switching from EnsureCreatedAsync to MigrateAsync when using migrations).

---

## Overview
(Reference timestamps in brackets)

This workflow shows how to:

- Enable GitHub Copilot agent mode and grant access to Microsoft Docs so the agent can consult official guidance for EF Core 9. [00:00:23 — 00:00:38]
- Ask the agent to search documentation and answer based on full document content. [00:00:38 — 00:00:57]
- Review and accept a tool permission prompt for the agent to access external docs (session-only permission recommended). [00:00:57 — 00:01:14]
- Receive agent analysis and recommendations aligned to EF Core 9 (including links to docs). [00:01:24 — 00:01:55]
- Request the agent to implement the recommended changes and select a model/session (e.g., GPT-4 or GPT-5). The agent reads project files, modifies code, and advises switching from EnsureCreatedAsync to MigrateAsync when using migrations. [00:01:55 — 00:03:07]
- Review final implementation and choose whether to accept agent changes or perform manual edits. [00:03:07 — 00:03:44]

Key concepts:
- Use MigrateAsync when you manage schema changes with EF migrations.
- EnsureCreatedAsync is acceptable for quick/no-migrations scenarios but not compatible with migrations.
- Agent sessions are tied to a selected model; choose according to your preferences and available options.

---

## Step-by-step Instructions

Follow these steps to replicate the demonstration and update your DB initializer safely.

### 1 — Prepare the scenario (00:00:01 — 00:00:17)
1. Open your products backend project that uses EF Core.
2. Identify the current DB initializer implementation (e.g., a class or code in Program.cs that calls EnsureCreatedAsync or uses a custom initializer).
3. Note any uncertainty about whether the current approach follows EF Core 9 best practices.

Tip: Have the project opened in your IDE so the agent can inspect files if your Copilot agent supports code access.

---

### 2 — Enable GitHub Copilot agent mode and grant Microsoft Docs access (00:00:23 — 00:00:38)
1. In the Copilot UI, enable "GitHub Copilot agent mode."
2. When prompted, grant the agent permission to access Microsoft Docs (this lets the agent read official EF Core documentation).
   - UI elements: GitHub Copilot agent mode, Microsoft Docs (access permission)

Warning: Granting external tool access may expose your environment or project metadata to the agent or service. Prefer "Allow this time" for a single session instead of permanent access unless you trust the configuration.

---

### 3 — Ask the agent to search documentation and set answer constraints (00:00:38 — 00:00:57)
1. Ask the agent to search Microsoft Docs for EF Core 9 guidance related to DB initialization.
   - Example prompt: "Search Microsoft Docs for EF Core 9 guidance on DB initializers and migrations. Provide an answer based on the full document content and link references."
2. Specify that the agent should base its answer on the complete document(s) it finds, not just a summary.

UI element: Search tool (Microsoft Docs)

Tip: Be explicit about the scope ("EF Core 9" and "DB initializer" or "EnsureCreated/Migrate") to minimize irrelevant results.

---

### 4 — Review and respond to the tool permission dialog (00:00:57 — 00:01:14)
1. A permissions dialog will appear describing what the agent/tool will access.
2. Review the scope and decide:
   - "Always allow" — grants ongoing access to the specified resource; consider only if you trust the tool and workspace.
   - "Allow (this time)" — grants one-session access (recommended for safety).
3. Choose the appropriate option. In the demo, "Allow (this time)" was selected.

Warning: Granting "Always allow" increases persistent access and risk. Use scoped or session-only grants when possible.

---

### 5 — Wait for agent analysis and review recommendations (00:01:24 — 00:01:55)
1. Allow the agent time to process the documentation and your code context.
2. Read the agent's recommendation panel carefully. It should:
   - Confirm whether the existing initializer is acceptable.
   - Recommend changes to align with EF Core 9 best practices.
   - Provide links and references to relevant Microsoft Docs pages.

UI elements: Agent recommendation panel, reference links to documentation

Tip: Open the provided reference links to verify the guidance yourself before applying changes.

---

### 6 — Request the agent to implement changes and choose a model/session (00:01:55 — 00:02:41)
1. Ask the agent to implement the recommended changes in your codebase.
   - Example prompt: "Apply the suggested EF Core 9 changes to the initializer and program files in this project. Update code to follow doc recommendations."
2. When starting a new agent session you may need to select a model (session tied to a model):
   - Choose from available models (e.g., GPT-4, GPT-5) based on accuracy, cost, and speed preferences.
3. Allow the agent to read relevant project files (Program.cs, DbContext class, initializer class, entity classes).
   - UI elements: Model selection, agent session starter

Tip: If you prefer, make a local commit or backup before allowing automated edits so you can revert changes easily.

---

### 7 — Review and accept code changes; key implementation adjustments (00:02:41 — 00:03:07)
1. The agent will apply code modifications and present changed files for review.
2. Key changes to verify:
   - If you use EF migrations: replace EnsureCreatedAsync (and EnsureCreated) with MigrateAsync (and Migrate) in initialization code.
     - Rationale: EnsureCreated bypasses migrations, which can cause conflicts if you later use EF migrations. Migrate applies pending migrations and keeps schema consistent with migration history.
   - Ensure conditional logic handles sync vs async execution properly (e.g., avoid calling .Result or .Wait on async methods in certain contexts).
   - Update DB initializer, Program.cs, and DbContext as needed to match EF Core 9 patterns.
3. Inspect the diff the agent provides and approve or reject changes.

UI elements: Modified code files (DB initializer, context, Program), API methods: EnsureCreatedAsync, MigrateAsync

Warning: Running Migrate/MigrateAsync applies schema changes to the target database. Ensure backups exist or run against a development database before production.

Tip: If your project intentionally does not use migrations (for example, ephemeral local DBs), EnsureCreatedAsync may still be acceptable; choose the approach that matches your workflow.

---

### 8 — Final review and workflow notes (00:03:07 — 00:03:44)
1. Confirm the final implementation aligns with documentation:
   - Agent-based changes should include references and follow the doc guidance.
   - Check conditional branches for sync vs async behavior and ensure they follow safe patterns.
2. Decide whether to keep agent-driven changes or to revert and manually edit:
   - You can prompt the agent for additional refinements, or manually change code if you prefer finer control.
3. Commit changes to your version control system once satisfied.

UI elements: Conditional logic in code (sync vs async branches), agent-driven code modification workflow

Tip: Use small, incremental commits when letting agents modify code. This makes rollbacks straightforward.

---

## Helpful Tips and Warnings

- Tip: Always create a backup branch or commit before allowing automated edits. Agents can make correct suggestions but may not fully account for project-specific constraints.
- Tip: Verify the documentation links the agent provides. Agents can misinterpret or overgeneralize guidance.
- Warning: Granting external access to documentation or repositories increases exposure. Prefer session-only permissions and review permission scopes carefully.
- Warning: Using MigrateAsync will execute migrations against the configured database. Run this in a safe environment when testing.

---

This manual covers the practical steps and considerations to use GitHub Copilot agent mode to consult Microsoft Docs and update a DB initializer to follow EF Core 9 recommendations — including the important distinction between EnsureCreated and Migrate when using migrations.