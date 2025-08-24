# User Manual: Using Copilot Agent Mode to Implement Missing Unit Tests

This manual explains how to use Copilot in agent mode to automatically identify and implement missing unit tests (example: AI Search and "formal" search endpoints), accept generated changes, and run the new tests in your IDE’s Test Explorer.

---

## Overview
(00:00:02 – 00:00:41)

This workflow demonstrates how to:
- Use Copilot (agent mode) to analyze a codebase and generate missing unit tests for AI-related endpoints.
- Configure Copilot’s context and model for focused results (AI Search context, GPT‑5 model).
- Trigger a quick-action request to implement tests.
- Allow Copilot to read, generate, build, and iterate on the solution.
- Review and accept generated changes, then run and verify new tests in the Test Explorer.

Estimated time for automated analysis + build: about 2 minutes.

---

## Step-by-step Instructions

Follow these steps to reproduce the demonstrated process.

### 1. Open your solution and Copilot
- Ensure your project/solution is open in the IDE where Copilot is available.
- Confirm Copilot is visible (usually a sidebar or panel labeled "Copilot").

Timestamp: 00:00:02 – 00:00:41

### 2. Switch Copilot to Agent Mode and create a chat
1. Switch Copilot into **Agent Mode** (look for an agent toggle or mode selector).  
   Timestamp: 00:00:41 – 00:01:15
2. Click **New Chat** to create a fresh session.
3. In the chat, select the appropriate context — for this workflow choose **AI Search** (or a context that maps to the endpoint you want tested).
4. Open the model dropdown and choose **GPT‑5** (or the desired model). You can change models later if needed.

UI elements: Agent mode toggle, New Chat button, context selection, model dropdown.

Tip: Selecting the specific context helps Copilot focus on relevant files and endpoints.

### 3. Trigger the task using a hashtag quick action
1. In the new chat input, type the hashtag symbol (`#`) and begin typing a task name such as "implement unit tests".  
2. From the quick-action list that appears, select the specific quick action for the target area (for example, "Implement unit tests for AI search").  
   Timestamp: 00:01:09 – 00:01:21

Tip: The hashtag quick action is a shortcut to pre-defined tasks — explore other quick actions for additional automation.

### 4. Let Copilot analyze, generate code, and iterate (Read → Build → Iterate)
1. After triggering the quick action, allow Copilot to read/index the entire solution. This is an automated background process.  
2. Copilot will:
   - Identify missing tests for the selected endpoints (AI search, formal search).
   - Generate new test files and modify project files as needed.
   - Attempt to build the solution to validate the generated code.
3. If the build fails due to missing dependencies, Copilot may add or update dependencies and re-run the build automatically.

Estimated duration: ≈ 2 minutes.  
Timestamp: 00:01:21 – 00:02:56

Warning: Automated dependency changes can affect your project. Review modifications before accepting them.

### 5. Review generated changes and accept them
1. When Copilot finishes, review the generated unit tests and any other code changes in the diff or file-change view.
2. Use the **Accept Changes** (or equivalent) control to apply Copilot’s updates to your codebase.  
   Timestamp: 00:02:56 – 00:03:58

Tip: Inspect test logic, mocked inputs, and assertions to ensure they match expected behavior.

### 6. Run tests in Test Explorer and verify results
1. Open the **Test Explorer** in your IDE.
2. Locate the newly added tests (example names: "Products AI test" or similar AI test entries).
3. Run tests:
   - Click **Run All Tests** or run only the new tests you want to validate.
4. Watch the build output / test run logs for success/failure details.
5. Confirm that the new unit tests pass.

UI elements: Test Explorer, Run All Tests button, build/output logs.  
Timestamp: 00:02:56 – 00:03:58

Tip: If a test fails, open the test code and related endpoint code to debug. Copilot can help iterate on failing tests if you re-run the quick action with additional instructions.

---

## Tips and Warnings

- Tip: Use targeted contexts (e.g., "AI Search") to speed up analysis and get more relevant tests generated.
- Tip: Allow Copilot to complete its build+iterate cycle — it may take ~2 minutes depending on solution size.
- Warning: Always review generated code and dependency changes before accepting them. Automated generation can introduce logic or dependency changes that need human validation.
- Warning: Unit tests generated automatically might require adjustments for edge cases, environment setup (mocks, secrets), or test data.

---

Timestamps reference:
- Introduction & Goal: 00:00:02 – 00:00:41  
- Agent Mode, Chat & Model Selection: 00:00:41 – 00:01:15  
- Hashtag Quick Action: 00:01:09 – 00:01:21  
- Copilot Analysis & Iteration: 00:01:21 – 00:02:56  
- Accept Changes & Run Tests: 00:02:56 – 00:03:58  
- Follow-up suggestion: 00:03:58 – 00:04:00

This manual should allow you to reproduce the video workflow to generate and run unit tests using Copilot agent mode.