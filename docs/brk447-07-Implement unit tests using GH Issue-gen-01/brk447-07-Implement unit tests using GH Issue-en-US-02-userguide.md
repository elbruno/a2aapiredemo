# Implementing Missing Unit Tests with GitHub Copilot (Cloud Agent)
A step-by-step user manual based on the demonstrated workflow for assigning GitHub Copilot to implement missing unit tests in a repository, reviewing the automated pull request and monitoring the cloud-isolated session and CI.

---

## Overview
(00:00:00 — 00:04:34)

This guide shows how to:
- Decide between a quick local unit test implementation and using a cloud agent to implement missing tests.
- Enable GitHub tools (via MCP) and search repository issues for missing unit tests.
- Assign GitHub Copilot (from GitHub or Visual Studio) to an issue.
- Monitor Copilot as it creates a fork, runs in a cloud-isolated session, and opens a pull request (PR).
- Inspect prior/completed PRs, review changed files and newly added tests.
- Read the session log and track GitHub Actions CI runs for the automated PR.

Use this manual to reproduce the same Copilot-driven workflow shown in the video.

---

## Step-by-step instructions

### 1. Choose local vs. cloud agent approach
(00:00:00 — 00:00:11)
1. Consider whether the missing unit tests can be implemented quickly on your machine:
   - If a simple, one-off test is needed, implement locally.
   - If you want an automated agent to generate multiple tests across repository boundaries, use a cloud-isolated Copilot session.
Tip: Use cloud agent when you want a reproducible, recorded session and automated PR creation.

### 2. Enable GitHub tools via MCP and search issues
(00:00:11 — 00:01:03)
1. Open the MCP (Microsoft Copilot Platform or relevant tool menu in your environment).
2. Enable the GitHub tools integration.
3. When prompted, grant permission for the tool to run actions on your behalf (allow execution).
   - Warning: This grants the tool scoped permissions — confirm the requested scopes before allowing.
4. Use the Issues search tool to query the repository for terms like "missing unit tests", "unit tests", or specific entity names (products, store, domain).
5. Review search results for open issues that request unit tests.

Relevant UI elements:
- GitHub tools (MCP)
- Issues search tool
- Permission / allow execution dialog

### 3. Open and review the identified issue (example: Issue #18)
(00:01:00 — 00:01:37)
1. Open the issue identified by the search (example: issue #18).
2. Read the issue details:
   - Scope — which entities need tests (e.g., products, store, domain).
   - Acceptance criteria — what constitutes completion.
   - Tasks and labels — work breakdown and metadata.
3. Confirm the issue accurately describes the missing tests before assigning a bot.

Timestamps for reference: Issue discovery at ~00:01:00.

### 4. Assign GitHub Copilot to the issue
(00:01:37 — 00:02:02)

You have two assignment options:

Option A — Assign via GitHub (preferred)
1. On the issue page, use GitHub's assignee field or Copilot integration UI to assign Copilot to the issue.
2. Refresh the issue page if needed to see assignment changes and any follow-up status.

Option B — Assign via Visual Studio
1. In Visual Studio, use the Copilot or GitHub command palette integration that supports assigning Copilot to an issue.
2. Issue the assign command targeting the issue number (e.g., assign Copilot to issue #18).

Tips:
- Use GitHub if you want visibility directly on the issue page.
- Use Visual Studio if you already work there and prefer IDE-based commands.

### 5. Observe automated pull request creation and cloud session start
(00:02:02 — 00:02:56)
1. After assignment, Copilot will:
   - Create a fork of the repository (if required).
   - Create a cloud-isolated environment/session to perform code changes.
   - Begin creating a pull request with proposed changes.
2. Open the new PR details page to monitor progress.
3. In the PR or session details view, watch for indicators: fork creation, environment provisioning, and session status.
   - Note: Environment creation and the overall session may take several minutes.

Warnings:
- Cloud sessions can take time (~minutes). Do not assume an immediate PR — allow Copilot to finish the provisioning and commit flow.

### 6. Review a previous completed PR for reference (example: PR #17)
(00:02:56 — 00:03:37)
1. Open a recently completed PR produced by Copilot (example: PR #17).
2. Navigate to the Changed files / Diff viewer tab.
3. Inspect the file tree and look for newly added test files (e.g., products tests, store tests).
4. Read any implementation notes included in the PR description to understand the testing approach Copilot used.

Tip: Reviewing a prior PR helps set expectations for file layout and test patterns Copilot produces.

### 7. Read the session log and duration details
(00:03:37 — 00:03:55)
1. In the PR or session details, open the session log or timeline.
2. Read the recorded steps — typical entries include:
   - Cloning the repository
   - Making a plan
   - Viewing source code
   - Applying changes
3. Note the session duration (example shown: ~18 minutes) to understand total compute time spent.

Tip: The session log is useful for auditing decisions the agent made and for debugging unexpected changes.

### 8. Monitor the current PR and GitHub Actions CI run
(00:03:55 — 00:04:34)
1. Open the current/ongoing PR details page.
2. Go to the GitHub Actions / Checks tab for the PR to monitor CI workflow runs triggered by the PR.
3. Stream or open the action logs (console output) to:
   - Watch test execution
   - See build output and failures
   - Find clues for debugging or acceptance verification
4. If the CI run fails, review logs and the PR diffs to determine required fixes; you can:
   - Make manual edits in the fork/branch
   - Reassign or re-run the Copilot agent session (if supported)

Tip: The continuous integration logs provide full traceability for what happened during the automated test run and are essential for validating generated tests.

---

## Helpful tips and warnings
- Tip: Use meaningful issue descriptions and acceptance criteria — the agent relies on clear instructions to generate correct tests.
- Warning: Granting permissions to GitHub tools allows automated actions (forks, PR creation). Verify scopes before allowing.
- Tip: Refresh issue and PR pages after assignment to see updated status and session links.
- Warning: Cloud sessions and CI runs may incur costs and take time — plan for the associated compute and runtime.
- Tip: Review prior Copilot-produced PRs to learn the agent’s typical patterns for tests and code structure.

---

End of manual.