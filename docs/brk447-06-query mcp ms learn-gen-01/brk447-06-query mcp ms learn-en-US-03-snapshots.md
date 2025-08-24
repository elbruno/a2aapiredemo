# Video: [brk447-06-query mcp ms learn.mp4](./REPLACE_WITH_VIDEO_LINK) — 00:03:45

# DB Initializer Review and EF Core 9 Migration Guide (Using GitHub Copilot Agent)

This manual guides you through the exact process demonstrated in the video: using GitHub Copilot agent mode to consult Microsoft Docs, grant tool permissions, request documentation-based recommendations, and apply code changes that align a product backend's database initializer with Entity Framework Core 9 best practices — including switching from EnsureCreatedAsync to MigrateAsync when appropriate.

- Total demo duration: 00:03:44.720
- Language level: suitable for all skill levels
- Includes step-by-step instructions, tips, and snapshots for reference

## Overview
(00:00:01.840 – 00:00:17.920)

This scenario evaluates an existing DB initializer implementation in a products backend using Entity Framework. The goal is to confirm whether the current initializer is appropriate for EF Core 9 and, if not, update code to match EF Core 9 recommendations.

Key topics:
- Use GitHub Copilot agent to consult Microsoft Docs
- Grant and manage tool permissions safely
- Apply agent-driven code modifications or choose to edit manually
- Understand EF Core 9 guidance: prefer MigrateAsync when using migrations; use EnsureCreated* only for simple scenarios without migrations

## Step-by-step Instructions

Follow the steps below to reproduce the demonstrated workflow. Timestamps are provided for reference to the video.

### 1. Prepare to consult Microsoft Docs with GitHub Copilot agent
- Purpose: Let the agent search authoritative docs and analyze your project.
- Video reference: 00:00:23.520 – 00:00:38.838

Steps:
1. Open your project in the environment that supports GitHub Copilot agent mode.
2. Enable **GitHub Copilot agent mode** in the UI.
   - Tip: Agent mode allows Copilot to use external tool access (Microsoft Docs) and read project files when permitted.
3. Grant the agent permission to access **Microsoft Docs** (for the current session).
   - Use the option to allow only this time if you want minimal persistence.

Snapshot: see image at 00:00:23.520 for the Copilot agent UI state.

Warning: Only grant access to third-party tools when you trust the documentation source and the session. Use "Allow (this time)" if unsure.

### 2. Ask the agent to search Microsoft Docs and produce a guidance-based answer
- Video reference: 00:00:38.838 – 00:00:57.240

Steps:
1. Formulate a clear question for the agent, for example:
   - "Is the DB initializer implemented correctly for Entity Framework Core 9? Please search Microsoft Docs and base your answer on the full document content."
2. Specify that the agent should use the complete docs content when forming recommendations.

Tip: Be explicit about which docs or topics to search (e.g., "EF Core 9 database initialization, migrations, EnsureCreatedAsync, MigrateAsync").

Snapshot: see agent search request state around 00:00:38.838.

### 3. Review and respond to the tool permission prompt
- Video reference: 00:00:57.240 – 00:01:14.800

Steps:
1. When the permissions dialog appears, review:
   - Which tools the agent will access
   - The scope of permission (this session vs always)
2. Choose an appropriate permission scope:
   - Select **Allow (this time)** for one-off checks.
   - Select **Always allow** only if you trust the agent and want persistent access.
3. Confirm the selection to proceed.

Warning: Granting "Always allow" increases exposure; prefer scoped session permission for unfamiliar repos or docs.

Snapshot: permissions prompt at 00:00:57.240.

### 4. Wait for the agent analysis and review recommendations
- Video reference: 00:01:24.960 – 00:01:55.560

Steps:
1. Allow the agent time to process the docs and analyze your codebase.
2. Review the agent's recommendation panel which will:
   - Confirm whether the existing initializer works
   - Suggest changes aligned with EF Core 9 (with links to docs)
3. Click any reference links provided to inspect the source docs if desired.

What to expect:
- The agent may confirm current implementation is valid but recommend a pattern change — typically moving to MigrateAsync when using migrations.

Snapshot: agent recommendation panel at 00:01:24.960.

### 5. Trigger an agent session to implement suggested code changes
- Video reference: 00:01:55.560 – 00:02:41.122

Steps:
1. If you want the agent to implement changes, start a new agent session specifically: choose the action to "Apply changes" or similar in the Copilot UI.
2. Select the model/session for the work (sessions are tied to a model). Common choices:
   - GPT-4
   - GPT-5 (if available)
3. Confirm the agent can read your project files (Program.cs, DbContext, DB initializer classes).
4. Let the agent scan project files to identify where to change code.

Tip: Choose a model with the right balance of cost and capability for heavier code edits; sessions persist per model.

Snapshot: model/session selection UI at 00:01:55.560 and agent reading project files ~00:02:05.000.

### 6. Review the agent-applied changes and migration guidance
- Video reference: 00:02:41.122 – 00:03:07.970

Steps:
1. When the agent completes changes, open the modified files:
   - DB initializer
   - DbContext (if modified)
   - Program.cs or application startup logic
2. Verify the specific recommended adjustment:
   - If your project uses EF Core migrations, the agent should replace EnsureCreatedAsync(...) with MigrateAsync(...) in initialization paths that rely on migrations.
   - Keep EnsureCreatedAsync only for scenarios without migrations (e.g., quick demo or in-memory/simple setups).
3. Confirm conditional logic for sync vs async execution is correct:
   - The agent might add branches to call synchronous APIs when running in sync paths, and async ones when in async initialization paths.

Key change:
- Use context.Database.MigrateAsync() when using migrations to apply pending migrations at startup.
- Avoid calling EnsureCreatedAsync when migrations are used — it can bypass migrations metadata.

Snapshot: modified code and migration guidance at 00:02:41.122 and review screen at 00:03:07.970.

### 7. Verify and test the updated initializer
Steps:
1. Build your project.
2. Run the application and observe startup logs to ensure migrations are applied (if configured).
3. Run database-related unit/integration tests:
   - Verify schema matches expected migrations.
   - Ensure initialization logic runs without exceptions.
4. If something fails, inspect the startup initializer and the exact database calls made (MigrateAsync vs EnsureCreatedAsync).

Tip: Run migrations manually in a staging environment first before enabling automatic MigrateAsync on production startup.

### 8. Decide to accept agent changes or edit manually
- Video reference: 00:03:07.970 – 00:03:44.720

Steps:
1. If agent changes look correct, accept/commit them.
2. If you prefer manual control, use the agent's diffs as guidance:
   - Copy the suggested code into your codebase and adjust naming/logic as needed.
3. Consider further refinements based on documentation links the agent provided.

Tip: Keep a review checklist: code compiles, migrations are recognized, startup logs show migrations applied (or no-op if none), and tests pass.

Warning: Automated code changes may need small stylistic or structural tweaks to integrate with your project conventions.

## Inline Snapshots / Image Placeholders

(These images indicate where to capture visuals for a step; replace placeholders with actual frames)

- Enable Copilot agent and grant Microsoft Docs access (00:00:23.520)  
  ![Enable Copilot Agent and grant MS Docs access @ 00:00:23.520](./snapshot-00-00-23-520.png)

- Tool permission prompt dialog (00:00:57.240)  
  ![Permissions dialog - Allow this time / Always allow @ 00:00:57.240](./snapshot-00-00-57-240.png)

- Agent recommendation panel with documentation links (00:01:24.960)  
  ![Agent recommendation with reference links @ 00:01:24.960](./snapshot-00-01-24-960.png)

- Model/session selection and agent session starter (00:01:55.560)  
  ![Model/session selection (GPT-4/GPT-5) @ 00:01:55.560](./snapshot-00-01-55-560.png)

- Agent reading project files (Program.cs, Context, classes) (00:02:05.000)  
  ![Agent scanning project files @ 00:02:05.000](./snapshot-00-02-05-000.png)

- Modified code and guidance note to use MigrateAsync (00:02:41.122)  
  ![Modified DB initializer / switch to MigrateAsync @ 00:02:41.122](./snapshot-00-02-41-122.png)

- Final review and conditional logic explanation (00:03:07.970)  
  ![Review final implementation and sync vs async branches @ 00:03:07.970](./snapshot-00-03-07-970.png)

## Tips and Warnings (quick reference)
- Tip: Use "Allow (this time)" for documentation access until you trust the workflow.
- Tip: Prefer MigrateAsync when your app uses EF Core migrations; EnsureCreatedAsync is for lightweight scenarios without migrations.
- Warning: EnsureCreated bypasses the migrations system and can lead to schema mismatch if you've committed migrations.
- Tip: Use agent diffs as a starting point — review and test before committing to main branches.
- Warning: Sessions are tied to the selected model; switching models may require re-running the agent session.

## Snapshots

[00:00:23.520]  
[00:00:57.240]  
[00:01:24.960]  
[00:01:55.560]  
[00:02:05.000]  
[00:02:41.122]  
[00:03:07.970]