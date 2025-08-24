# Using GitHub Copilot Cloud Agents to Implement Missing Unit Tests — User Manual

This manual explains how to find repository issues that request missing unit tests, assign GitHub Copilot (cloud-isolated agent) to implement them, and review the resulting pull requests, session logs, and CI runs. Steps map to the demonstration shown in the referenced video.

- Total demonstration length: 00:04:34.320
- Relevant timestamps are included throughout for quick reference.

## Overview

(00:00:00.160 — 00:00:11.849)

This workflow shows two approaches to add unit tests:

- Local, manual development of unit tests — quick and controlled for small tasks.
- Cloud-isolated Copilot agent that performs the work in a cloud session: it clones/forks the repository, runs an environment, makes changes, and opens a pull request automatically. Use the cloud agent when you want automated help or to offload repetitive test implementation.

Use this guide to:
- Search for issues that request missing unit tests
- Enable and allow GitHub tools for Copilot to operate
- Assign Copilot to an issue
- Monitor the cloud session, PR creation, changed files, session log, and CI run

---

## Step-by-step instructions

Follow these numbered procedures in order. Each procedure includes relevant tips and snapshots for reference.

### 1. Enable GitHub tools and perform an issues search
(00:00:11.849 — 00:01:03.008)

1. Open your project repository in the interface that integrates with GitHub Copilot (for example the MCP tool window or the integrated Copilot UI).
2. Enable GitHub tools / Copilot features in the MCP or the provided tools panel. Grant any permission prompts required to allow the tools to access the repository and run searches.
   - Snapshot: GitHub tools enabled dialog (00:00:11.849)
   - Tip: If a permission dialog appears, verify the scope requested (search and repository read access) before allowing.
3. Run an issues search for missing unit tests. Example queries:
   - "missing unit tests"
   - "add unit tests"
   - "unit tests for products store domain"
4. Review the search results and open any candidate issues.

![Enable GitHub tools — 00:00:11.849](./snapshot-00-00-11.849.png)

Warning: Make sure you are authorized to enable tools for the organization/repository. Enabling without proper permission may require an admin approval step.

---

### 2. Open and review the issue (example: Issue #18)
(00:01:00.000 — 00:01:37.040)

1. From the search results, click the issue you want Copilot to address (example in the demo: Issue #18).
2. Read the issue details: scope, acceptance criteria, tasks, labels, and any additional metadata.
   - Confirm that the issue is clear about which entities need tests (e.g., products, store, domain entities) and what acceptance criteria are expected.
3. Decide whether Copilot can handle the task as described or if more details are required.

![Issue #18 details — 00:01:00.000](./snapshot-00-01-00.000.png)

Tip: If acceptance criteria are ambiguous, add a comment requesting clarification before assigning an agent.

---

### 3. Assign Copilot to the issue
(00:01:37.040 — 00:02:02.600)

You can assign Copilot (or the cloud agent) to the issue from either GitHub or from within Visual Studio (or your supported IDE). The demo shows both options.

From GitHub:
1. Open the issue page.
2. Use the "Assignees" field or Copilot integration button to assign Copilot to the issue.
3. Click refresh on the issue page if needed to see assignment status.

From Visual Studio:
1. Use the Copilot / issues command palette or the extension UI to select the issue and issue a command to assign Copilot.
2. Confirm assignment.

![Assign Copilot to issue — 00:01:37.040](./snapshot-00-01-37.040.png)

Tip: Prefer assigning via GitHub when possible to have the assignment visible in the repo for all collaborators.

---

### 4. Observe PR creation and cloud session startup
(00:02:02.600 — 00:02:56.920)

After assignment, Copilot begins a cloud-isolated session that performs the work.

1. Watch for a new pull request created automatically by Copilot.
2. Open the pull request to see session details:
   - The cloud session may show steps such as creating an environment, forking the repository, and interacting with GitHub.
   - Session creation may take some time (minutes), depending on environment provisioning.
3. Use the session details view to monitor progress and see the agent's actions.

![Pull request and cloud session start — 00:02:02.600](./snapshot-00-02-02.600.png)

Warning: The session runs in an isolated cloud environment. Do not expect immediate completion — environment provisioning and CI runs add latency.

---

### 5. Review a previously completed PR to understand typical output
(00:02:56.920 — 00:03:37.040)

1. Open a completed PR produced by Copilot (example: PR #17 in the demo).
2. Go to the Changed files / Diff viewer to see newly added test files and modifications.
   - Example: new unit tests for product and store entities.
3. Read any implementation notes or PR description Copilot provided to understand the approach.

![Previous PR (#17) changed files — 00:02:56.920](./snapshot-00-02-56.920.png)

Tip: Reviewing previously generated PRs helps you set expectations for code style, test structure, and commit messages.

---

### 6. Inspect the session log and recorded steps
(00:03:37.040 — 00:03:55.400)

1. Open the session log / timeline from the PR or session details page.
2. Review the recorded steps performed by Copilot. Typical steps include:
   - Cloning the repository
   - Making a plan
   - Viewing source code and test files
   - Implementing changes
3. Note session duration (example: ~18 minutes in the demo), and use the log for auditing or debugging.

![Session log and timeline — 00:03:37.040](./snapshot-00-03-37.040.png)

Tip: The session log is your primary source for understanding exactly what the cloud agent did and in what order.

---

### 7. Monitor the ongoing PR and GitHub Actions CI
(00:03:55.400 — 00:04:34.480)

1. Open the current PR created by Copilot.
2. Navigate to the GitHub Actions / Checks tab in the PR to monitor CI workflows triggered by the PR.
3. Stream or open the full CI logs to inspect build/test output, which helps validate the correctness of the new tests and find failures to address.
4. If CI fails:
   - Review the failing steps in the logs
   - Open the PR diffs to correlate failures with code changes
   - Leave comments or request further agent runs if supported

![GitHub Actions CI logs — 00:03:55.400](./snapshot-00-03-55.400.png)

Warning: CI run logs can be extensive. Use search or jump-to-error features to locate failure points quickly.

---

## Tips and Best Practices

- Start with clearly written issues that include acceptance criteria and expected behaviors — this improves the cloud agent’s output quality.
- Prefer assigning Copilot via GitHub for transparency with team members.
- Use the session log as the authoritative record of the agent’s actions.
- Review previous Copilot-produced PRs to understand typical test structure and conventions the agent follows.
- If the cloud session takes longer than expected, check for environment provisioning errors in the session details.

---

## Snapshots

- [00:00:00.160]
- [00:00:11.849]
- [00:01:00.000]
- [00:01:37.040]
- [00:02:02.600]
- [00:02:56.920]
- [00:03:37.040]
- [00:03:55.400]