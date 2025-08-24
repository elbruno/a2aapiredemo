# Video: [brk447-04 Add single unit Test for AI Search.mkv](./REPLACE_WITH_VIDEO_LINK) — 00:04:02

# Implementing Unit Tests for AI Search Using Copilot — User Manual

Version: 1.0  
Derived from video walkthrough (total duration ~00:03:58)

---

## Overview

This manual shows how to use Copilot (agent mode) to automatically implement and run unit tests for AI search endpoints. It covers identifying missing tests, configuring Copilot for the AI Search context, running a task to generate tests, resolving build issues, and verifying tests in Test Explorer.

Key outcomes demonstrated:
- Use Copilot agent mode to implement unit tests
- Target the AI Search context and choose a model (e.g., GPT-5)
- Let Copilot analyze the solution, add test files, and iterate on build issues
- Run and verify newly created unit tests in Test Explorer

Relevant video timestamps:
- Introduction & task overview: 00:00:02 — 00:00:12  
- Identify missing tests: 00:00:13 — 00:00:34  
- Switch to agent and start chat: 00:00:34 — 00:00:47  
- Select context & model: 00:00:47 — 00:00:59  
- Trigger task and start analysis: 00:01:09 — 00:01:31  
- Copilot analysis & iterative builds: 00:01:31 — 00:03:04  
- Fix dependencies & successful build: 00:02:56 — 00:03:24  
- Accept changes & run tests: 00:03:24 — 00:03:58

---

## Step-by-step Instructions

Follow these steps to reproduce the workflow.

### 1. Inspect the codebase and identify missing tests (00:00:13 — 00:00:34)
1. Open your project in your IDE.
2. Review the endpoints or components related to AI search (and any formal search endpoints you want covered).
3. Note which endpoints lack unit test coverage.

Tips:
- Look for controllers, services, or API endpoints named with "search", "AI", or similar keywords.
- Use your IDE’s search or the Code/Endpoints view to list endpoints quickly.

Warning:
- Do not start automated changes before confirming the scope of what you want Copilot to modify.

### 2. Switch Copilot to agent/assistant mode and create a new chat (00:00:34 — 00:00:47)
1. Open Copilot in your IDE.
2. Switch to *agent/assistant mode* (sometimes labeled “Agent mode”).
3. Click *New chat* to start a fresh session for this task.

Notes:
- You can return to previous chats if needed (previous chat list is usually available).

### 3. Select the AI Search context and choose the model (00:00:47 — 00:00:59)
1. In the new Copilot chat, set the context to **AI Search** using the context selector.
2. Choose the model you want (the video used **GPT-5**). You can switch models later if required.

Tip:
- Selecting the correct context focuses Copilot’s analysis and generation on AI search-related code patterns.

### 4. Trigger the unit-test implementation task (00:01:09 — 00:01:31)
1. In the chat input, type a task trigger using the hashtag or task autocomplete (e.g., `#implement unit tests`).
2. Select the task entry for *implement unit tests* and confirm the scope as **AI Search**.
3. Submit the task so Copilot begins analyzing the solution.

What happens:
- Copilot will read the solution, identify missing tests, generate new test files and any required code, attempt to build, and iterate on failures.

### 5. Let Copilot analyze and make iterative changes (00:01:31 — 00:03:04)
1. Wait while Copilot reads the entire project. This may take time depending on project size.
2. Monitor output logs or progress updates in the Copilot UI.
3. Allow Copilot to:
   - Create new unit test files
   - Modify code where necessary
   - Run a build and report errors

Tip:
- Be patient — Copilot performs automated multi-step work (read → modify → build → iterate).

### 6. Address build failures and missing dependencies (00:02:56 — 00:03:24)
1. If the initial build fails, check the build output or error log shown by Copilot or your IDE.
2. Accept or review Copilot’s proposed dependency or configuration changes. Copilot may:
   - Add missing NuGet/npm/package references
   - Update project files to include new test frameworks or helpers
3. Allow Copilot to re-run the build after dependencies are added.

Expected result:
- The build should succeed after dependencies are added and code is adjusted.
- In the demonstrated run, Copilot created two new unit test files for the AI products component.

Warning:
- Review automated dependency changes before fully accepting them, especially in production-critical projects.

### 7. Accept code changes and run tests in Test Explorer (00:03:24 — 00:03:58)
1. Use the *Accept Changes* (or equivalent) action to apply Copilot’s modifications to your workspace.
2. Open your IDE’s Test Explorer (or test runner view).
3. Locate the newly added tests (e.g., “Products AI” test entries).
4. Run the tests:
   - Click *Run All* to execute the full test suite, or
   - Run the specific new tests only, if preferred.

Verify:
- Tests appear in Test Explorer.
- Newly added tests pass (green/passing status) after a successful build.

Tip:
- If you use CI, commit the changes and let CI run the tests to validate behavior across environments.

---

## Tips & Warnings (General)

- Tip: Use small, clearly defined task scopes (like only AI Search) when first using automated agents to limit unintended changes.
- Tip: Keep an eye on build output and Copilot iteration logs to understand what was modified.
- Warning: Always review and, if necessary, run static analysis or security checks on automatically generated code before merging.
- Warning: Automated dependency changes can affect other parts of your solution—review them carefully.

---

End of manual.