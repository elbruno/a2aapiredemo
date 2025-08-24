# DB Initializer with GitHub Copilot Agents and EF Core 9 — User Manual

This manual walks you through the workflow shown in the video: using a GitHub Copilot Agent to consult Microsoft Docs and update an Entity Framework Core 9 DB initializer implementation. It is written for developers of all experience levels and includes step-by-step instructions, relevant timestamps, and helpful tips and warnings.

---

## Overview

This scenario demonstrates how to:

- Enable and use a GitHub Copilot Agent to search Microsoft Docs for EF Core 9 guidance (00:00:23.520–00:00:51.080).
- Grant tool permissions for the session so the agent can read project files (00:00:57.240–00:01:14.800).
- Receive agent recommendations that EF Core 9 prefers migrations + Migrate/MigrateAsync over EnsureCreated (00:01:24.960–00:01:58.680).
- Trigger the agent to implement code changes and select the model used for the agent session (00:02:09.320–00:02:41.122).
- Apply the agent’s changes: refactor DB initialization to use Database.MigrateAsync / Migrate and improved seeding with conditional sync/async handling (00:02:41.122–00:03:44.720).

Timestamps in parentheses point to the video timeline for quick reference.

---

## Step-by-step instructions

Follow these ordered steps to reproduce the workflow in the video. Each major step includes smaller tasks and optional tips.

### 1. Review the existing project (00:00:01.840–00:00:23.520)

1. Open your project that uses Entity Framework (example: a "products" project).
2. Inspect:
   - Your DbContext implementation.
   - The current DB initializer (the class or code path that seeds or ensures database creation).
   - Program.cs / startup code where the initializer is invoked.

Tip: Note whether the initializer uses EnsureCreated/EnsureCreatedAsync or Database.Migrate/MigrateAsync before proceeding.

### 2. Enable GitHub Copilot Agent and select Microsoft Docs (00:00:23.520–00:00:51.080)

1. Open the GitHub Copilot Agents control in your IDE or platform.
2. Enable agent mode.
3. Add or enable the Microsoft Docs tool as a data source for the agent (so it can search official docs).
4. In the agent prompt, ask it to search Microsoft Docs for DB initializer patterns for EF Core 9 and to answer based on full doc content.

Tip: Phrase the request clearly, e.g., “Search Microsoft Docs for recommended DB initialization approaches in EF Core 9 and compare patterns using EnsureCreated vs migrations.”

### 3. Grant permissions for tools (00:00:57.240–00:01:14.800)

1. A permissions dialog will appear warning that the agent’s tools may access your disk or run commands.
2. Choose the appropriate permission scope:
   - For demos or single runs, choose **Allow (this session)** (the presenter’s choice).
   - For trusted projects or frequent use, you may choose **Always allow** (be cautious).
3. Confirm the permission choice to proceed.

Warning: Granting persistent permissions can expose your repository and environment to external actions. Use session-only permission for one-off operations or untrusted code.

### 4. Wait for the agent to analyze docs and project (00:01:24.960–00:01:58.680)

1. The agent will fetch and analyze the Microsoft Docs content and your project files.
2. Wait for the agent to return recommendations. This may take a few minutes.

What to expect in the agent response:
- A statement that the existing initializer likely works.
- A recommendation that EF Core 9 prefers migration-based initialization using Database.MigrateAsync / Database.Migrate instead of EnsureCreated/EnsureCreatedAsync when using migrations.
- Links to the relevant Microsoft Docs pages (review them).

Tip: Use the provided doc links to verify the guidance and reasoning behind the recommended patterns.

### 5. Ask the agent to implement the recommended changes (00:02:09.320–00:02:41.122)

1. Request the agent to apply the recommended changes to your project, e.g., “Please refactor the DB initializer to follow EF Core 9 recommendations and use migrations and seeding patterns from the docs.”
2. When prompted, start a new agent session tied to a model. Choose a model from the dropdown (examples: GPT-4.1, GPT-5).
   - Reasonable choice: GPT-4.1 for stable usage or GPT-5 if available and desired for more aggressive rewriting.
3. The agent will read key files (Program.cs, DbContext, initializer class, etc.) to plan the edits.

Tip: Selecting a stronger model (e.g., GPT-5, if available) can yield more thorough changes, but validate carefully.

### 6. Review the agent’s proposed code changes and rationale (00:02:41.122–00:03:07.970)

1. The agent will produce a patch preview showing changes to files.
2. Review the preview carefully:
   - Look for replacement of EnsureCreated/EnsureCreatedAsync with Database.MigrateAsync or Database.Migrate.
   - Look for added or updated seeding logic that checks for existing data, then inserts sample data.
   - Look for conditional handling allowing sync and async execution based on calling context.

Tip: Use the preview UI to inspect each change line-by-line before applying.

Warning: Automated changes may not match your exact conventions or environment—review before committing.

### 7. Apply the changes and commit (00:02:51.653–00:03:44.720)

1. Use the code changes/patch preview panel to apply or commit changes.
2. Confirm the agent session finishes and reports alignment with EF Core 9 guidance.
3. After applying changes:
   - Run local builds and tests.
   - Execute migrations if applicable (dotnet ef migrations add/updated, if needed).
   - Start the application and validate that the database schema has been migrated and sample data seeded.

Essential commands (examples — run from your project root):
- To apply pending migrations at runtime (async):
  - Use code: await context.Database.MigrateAsync();
- To apply pending migrations synchronously:
  - Use code: context.Database.Migrate();

Example seeding pattern (conceptual)
- In an async initializer:
  - await context.Database.MigrateAsync();
  - if (!context.Products.Any()) { add sample products; await context.SaveChangesAsync(); }
- In a sync initializer:
  - context.Database.Migrate();
  - if (!context.Products.Any()) { add sample products; context.SaveChanges(); }

Tip: Prefer calling MigrateAsync in async application startup flows and Migrate in synchronous flows. Keep initializer calls consistent with your hosting startup lifecycle.

---

## Tips & Warnings (general)

- Tip: Use migrations in development and production for predictable schema evolution. Ensure your CI/CD process can apply migrations safely.
- Tip: Always back up production databases before applying migrations.
- Warning: Avoid EnsureCreated/EnsureCreatedAsync if you plan to use migrations, because EnsureCreated bypasses the migrations pipeline and can produce schema inconsistencies.
- Warning: Grant tool permissions conservatively. Choose session-only permissions when experimenting with an agent you don’t fully trust.
- Tip: After agent-driven changes, run your full test suite and validate expected behaviors before merging the code.

---

This manual reproduces the practical steps in the video timeline to help you use a GitHub Copilot Agent to consult docs and implement EF Core 9–recommended DB initialization and seeding patterns. Follow the review and safety guidance to ensure changes are correct for your application.