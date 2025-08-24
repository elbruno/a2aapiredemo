# Video: [brk447-04 Add single unit Test for AI Search.mkv](./REPLACE_WITH_VIDEO_LINK) — 00:04:02

# Implementing Unit Tests for AI Search with Copilot — User Manual

This manual guides you through the process demonstrated in the video: using Copilot (agent mode) to implement and run unit tests for AI search endpoints. Follow the steps below to reproduce the workflow, from identifying missing tests to running and verifying the new tests.

---

## Overview

Duration: 00:00:02.400 — 00:03:57.680

This workflow shows how to:
- Identify missing unit tests for AI search endpoints
- Use Copilot in agent mode to auto-generate tests and any required code/dependencies
- Let Copilot build and iterate on failures
- Accept changes and run the resulting tests in the Test Explorer

Key concepts demonstrated:
- Selecting an appropriate Copilot context (AI Search)
- Choosing the model (e.g., GPT-5)
- Triggering a predefined task via hashtag
- Monitoring automated analysis, build, and iteration
- Verifying test results in Test Explorer

---

## Step-by-step Instructions

Follow these steps in order. Timestamps are provided for reference to the corresponding parts of the video.

1. Prepare your workspace (00:00:02.400 — 00:00:12.480)
   - Ensure your codebase is open in your IDE.
   - Commit or create a new branch before making automated changes.  
     Tip: Always keep a backup or branch to easily revert changes made by Copilot.

   ![Snapshot: Ready workspace and task overview][00:00:02.400]

2. Identify missing tests and decide scope (00:00:13.120 — 00:00:34.160)
   - Inspect the codebase or endpoints list to find endpoints lacking unit tests.
   - In this scenario, the AI search endpoint (and possibly the formal search) are missing tests.
   - Decide which components you want Copilot to cover — e.g., AI Search only, or the whole solution.

   ![Snapshot: Code/Endpoints view showing missing tests][00:00:13.120]

3. Switch to Copilot Agent mode and start a new chat (00:00:34.360 — 00:00:47.320)
   - Open Copilot and switch it to the agent/assistant mode.
   - Click "New chat" to create a fresh conversation for the task.
   - Note: You can return to previous chats if you need to revisit earlier context.

   ![Snapshot: Copilot agent mode and new chat button][00:00:34.360]

   Warning: Agent mode may request access to read your solution. Confirm you are comfortable granting this access for the session.

4. Select the AI Search context and choose a model (00:00:47.320 — 00:00:59.800)
   - In the new chat, set the context selector to "AI Search".
   - Choose the model you prefer for generation (the demo uses GPT-5). You can switch models before running the task.

   ![Snapshot: Context selector set to 'AI Search' and model selector showing GPT-5][00:00:47.320]

5. Trigger the task using the hashtag/task selector (00:01:09.560 — 00:01:31.870)
   - Type a hashtag or open the task autocomplete and select "implement unit tests".
   - Confirm the task scope (AI Search) when prompted.
   - Start the task: Copilot will read the codebase, propose or add test files, attempt builds, and iterate.

   ![Snapshot: Hashtag/task selector chosen: 'implement unit tests'][00:01:09.560]

   Tip: Adding a concise task description helps Copilot focus. Example: "#implement unit tests for AI Search endpoints".

6. Let Copilot analyze the solution and generate code (00:01:31.870 — 00:02:56.040)
   - Wait while Copilot scans the repository and generates files (it may create test files and helper code).
   - Monitor the build output/log; Copilot will attempt to build the solution automatically.
   - Be patient — this is an iterative, automated process and can take some time.

   ![Snapshot: Copilot analyzing and generating tests; build logs visible][00:01:31.870]

7. Inspect and resolve build failures (00:02:56.040 — 00:03:24.304)
   - If the build fails, check the build errors/log to identify missing dependencies or compile issues.
   - Copilot can add missing dependencies or change project configuration. Review these modifications carefully.
   - After Copilot adds fixes (e.g., dependency updates), the agent will re-run the build.

   ![Snapshot: Build failure log showing missing dependencies][00:02:56.040]

   Warning: Automatic dependency changes can affect other parts of the solution. Review and test changes before merging into main branches.

8. Confirm build success and review new tests (00:03:24.304)
   - Once the build succeeds, verify that the new unit test files have been created (the demo shows two new tests).
   - Inspect the generated tests to confirm they match expected behaviors for AI search.

   ![Snapshot: Successful build and new unit test files created][00:03:24.304]

9. Accept Copilot's changes and run tests in Test Explorer (00:03:24.304 — 00:03:58.600)
   - Accept or commit the changes Copilot made (use your IDE's Accept Changes or Commit tools).
   - Open Test Explorer in your IDE.
   - Run tests: click "Run All" or run the specific new tests (e.g., "products AI" tests).
   - Verify all tests pass. If failures remain, iterate: fix code or instruct Copilot to refine tests.

   ![Snapshot: Test Explorer showing 'Products AI' tests and Run All button][00:03:24.304]

   Tip: If any tests fail, examine the failing test's output first — it often points to missing mocks, environment setup, or dependency issues.

10. (Optional) Expand scope later
    - If you want broader coverage, repeat the process but set the task scope to "entire solution" to find and implement other missing unit tests.
    - Plan and run this on a safe branch as the number of automated changes may be significant.

---

## Images / Snapshots

These inline snapshots show important UI states referenced in the steps above. Use them as visual checkpoints while following the procedure.

- Snapshot at 00:00:02.400 — Workspace and task overview
  ![Snapshot: Ready workspace and task overview][00:00:02.400]

- Snapshot at 00:00:13.120 — Code/Endpoints view showing missing tests
  ![Snapshot: Code/Endpoints view showing missing tests][00:00:13.120]

- Snapshot at 00:00:34.360 — Copilot agent mode and new chat
  ![Snapshot: Copilot agent mode and new chat button][00:00:34.360]

- Snapshot at 00:00:47.320 — Context selector (AI Search) and model selector (GPT-5)
  ![Snapshot: Context selector set to 'AI Search' and model selector showing GPT-5][00:00:47.320]

- Snapshot at 00:01:09.560 — Hashtag/task selector 'implement unit tests'
  ![Snapshot: Hashtag/task selector chosen: 'implement unit tests'][00:01:09.560]

- Snapshot at 00:01:31.870 — Copilot analyzing and build logs
  ![Snapshot: Copilot analyzing and generating tests; build logs visible][00:01:31.870]

- Snapshot at 00:02:56.040 — Build failure due to missing dependencies
  ![Snapshot: Build failure log showing missing dependencies][00:02:56.040]

- Snapshot at 00:03:24.304 — Successful build and new unit test files
  ![Snapshot: Successful build and new unit test files created][00:03:24.304]

- Snapshot at 00:03:24.304 — Test Explorer showing 'Products AI' tests and Run All button
  ![Snapshot: Test Explorer showing 'Products AI' tests and Run All button][00:03:24.304]

---

## Snapshots

[00:00:02.400]
[00:00:13.120]
[00:00:34.360]
[00:00:47.320]
[00:01:09.560]
[00:01:31.870]
[00:02:56.040]
[00:03:24.304]