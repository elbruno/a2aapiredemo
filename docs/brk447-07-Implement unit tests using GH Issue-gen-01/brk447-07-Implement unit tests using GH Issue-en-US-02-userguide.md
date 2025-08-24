# Video: [brk447-07-Implement unit tests using GH Issue.mp4](./REPLACE_WITH_VIDEO_LINK) — 00:04:35

# User Manual — Using GitHub Copilot (Cloud Agent) to Add Missing Unit Tests

This manual shows how to delegate missing unit tests to GitHub Copilot running as a cloud agent, inspect the resulting pull request, and follow the execution using GitHub Actions logs. Steps and timestamps reference the demonstrated video sections.

## Overview
(00:00:00 — 00:04:34)

This workflow demonstrates the difference between implementing unit tests locally (fast, manual) and delegating the task to an isolated cloud agent (GitHub Copilot) that creates a fork, runs in a cloud environment, modifies the repository, and opens a pull request. You will:

- Search repository issues to find the missing-tests task (00:00:18 — 00:01:07).
- Inspect an issue and assign it to Copilot (00:01:07 — 00:02:02).
- Trigger Copilot to run in the cloud and create a PR (00:02:02 — 00:03:02).
- Review changed files and session details (00:03:02 — 00:04:07).
- Monitor the running PR and inspect GitHub Actions logs (00:04:07 — 00:04:34).

Follow the step-by-step instructions below to replicate the process.

## Step-by-step Instructions

### 1. Enable GitHub tooling and search for repository issues (00:00:18 — 00:01:07)
1. Open the repository on GitHub.
2. Ensure GitHub tooling / Copilot integration is enabled for the repository and that you have the required permissions:
   - If prompted, *allow tool execution* or enable the GitHub Copilot action for the repository.
3. Use the repository Issues search to look for missing unit tests. Example search text: `missing unit tests` or search by keywords like `unit test`, `tests`, `coverage`.
4. Locate the relevant issue (example: Issue #18 — "add missing unit tests for products, store and domain entity").

Tips:
- If you cannot find an issue, open a new Issue with a clear scope and acceptance criteria so Copilot can act on it.
- You must have permissions to assign issues and allow Copilot to create forks/PRs.

Warning:
- Enabling tool execution may grant the agent permission to read/write code and create forks — confirm organization policies before enabling.

### 2. Inspect the issue and decide how to assign work (00:01:07 — 00:02:02)
1. Open the issue detail page (example: Issue #18).
2. Review:
   - Scope and description
   - Acceptance criteria
   - Task checklist and labels
3. Choose how to assign the work:
   - Preferred: Use the GitHub UI to run Copilot from the issue page (assign to Copilot).
   - Alternative: Assign to Copilot from an IDE such as Visual Studio (if integrated).
4. Execute the "Assign to Copilot" action.

What to expect:
- The issue shows an assignment to Copilot and a status indicating an agent has started work.

Tip:
- Include specific acceptance criteria and example tests in the issue to improve the accuracy of Copilot's changes.

### 3. Trigger Copilot to create a pull request (cloud execution) (00:02:02 — 00:03:02)
1. After assigning Copilot, refresh the issue list or issue page.
2. Look for a new pull request entry created by Copilot (the UI shows a Copilot PR starting).
3. Open the PR details to see status updates.
   - The PR workflow typically shows steps such as: create environment, fork repository, make changes, run tests, create PR.
4. Note that work runs isolated in the cloud and a fork is created for the agent to push changes.

Expected outcome:
- A pull request appears in the repository (or in a forked repo) with a reference to the issue and details of the agent session.

Warning:
- The agent will create a fork and push commits automatically. Always review the generated code before merging.

### 4. Review completed PRs and changed files (example PR #17) (00:03:02 — 00:04:07)
1. Open an example completed PR (e.g., PR #17) to inspect the results.
2. Go to the "Files changed" tab:
   - Review new unit test files (e.g., tests for product and store entities).
   - Inspect modifications to existing files.
3. Open the session summary / timeline or agent session view:
   - Review the steps performed by the agent (e.g., "get repository", "make a plan", "view source code", "implement tests").
   - Note the session runtime (example shown ~18 minutes).
4. Ensure the tests address the issue's acceptance criteria and that code style and structure follow your repo conventions.

Tips:
- Run the tests locally (or trigger CI) against the PR branch to confirm behavior before merging.
- Use code review checklist items (security, secrets, performance) when validating agent-created code.

### 5. Monitor current PR run and examine GitHub Actions logs (00:04:07 — 00:04:34)
1. Open the current PR details page.
2. Navigate to the GitHub Actions tab or the workflow run linked from the PR.
3. Open the relevant workflow run and inspect the full logs:
   - Follow individual job steps and logs to see progress and results.
   - Look for failures or warnings in test execution, build steps, or deployment steps.
4. If a workflow fails, open the logs to identify failing tests or errors introduced by the changes and address them:
   - Comment on the PR with required changes or push fixes to the branch (if allowed).

Tip:
- Use the action logs to verify that the cloud environment executed tasks as expected (dependencies installed, tests run, results reported).

## Helpful Tips and Warnings
- Tip: Provide clear, itemized acceptance criteria and sample inputs/outputs in the issue to get better results from Copilot.
- Tip: Use the PR session summary to understand what the agent read and the plan it executed.
- Warning: Always perform a manual code review. The cloud agent may introduce unexpected code or miss repository-specific conventions.
- Warning: Verify that no secrets, credentials, or sensitive info are committed by the agent. Use repository secret scanning and review diffs carefully.
- Warning: Ensure organization policies allow automated agents to create forks and run actions.

---

Follow these steps to delegate test implementation to GitHub Copilot running in a cloud agent, track progress, and validate the generated changes using PR review and CI logs.