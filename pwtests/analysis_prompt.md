# Playwright Tests Analysis & User Manual Update Prompt

Purpose

This prompt is designed to be used with an AI assistant (LLM) to:

- Run the Playwright test suite located in `pwtests/`.
- Analyze test failures and underlying code causing them.
- Produce a clear, actionable report and a set of suggested updates to the `pwtests/USER_MANUAL.md`.
- Generate a PR description and CHANGELOG entry for any code or documentation changes.

How to use

1. Run the Playwright tests locally (PowerShell on Windows):

```powershell
cd pwtests
# install dependencies if needed
npm ci
# run the tests and save the output
npx playwright test --reporter=list | Tee-Object -FilePath playwright-test-output.txt
```

2. Provide the AI with the following artifacts (as files or text):

- `playwright-test-output.txt` (test run log)
- failing test file(s) from `pwtests/` (e.g., `scenario1.spec.js`, `scenario2.spec.js`)
- relevant project files referenced in stack traces (paths relative to repo root)
- `pwtests/USER_MANUAL.md` (current manual content)

3. Ask the AI to:

- Summarize the test run: number of tests run, passed, failed, skipped, and timings.
- For each failing test, extract the failure stack trace, error message, and the test code that failed.
- Locate relevant source files and explain likely root causes (e.g., UI changes, selector mismatches, timeouts, flaky network calls, missing test data).
- Propose minimal code or test changes to fix failures or make tests resilient (include code diffs/patches where applicable).
- Propose specific, clear updates to `pwtests/USER_MANUAL.md` to reflect any changes in how tests should be run, prerequisites, or troubleshooting steps.
- Produce a ready-to-use PR description, a small CHANGELOG entry, and a list of affected files.

Prompt template

Below is the template to give directly to the AI. Replace placeholders with the actual content or file attachments.

---

You are an expert test automation engineer with knowledge of Playwright and the repository layout. Your task: analyze the latest Playwright test run and update the user manual.

Inputs (attached as files or inline):

- `playwright-test-output.txt`: <<PASTE CONTENT OR ATTACH FILE>>
- List of failing test files and their contents: <<ATTACH FILES>>
- Any referenced source files from stack traces: <<ATTACH FILES OR PASTE CONTENTS>>
- `pwtests/USER_MANUAL.md`: <<ATTACH FILE OR PASTE CONTENT>>

Please perform the following steps and return results in structured format.

1) Test run summary

- Total tests, passed, failed, skipped
- Duration and any flaky test warnings

2) Failures details (for each failing test)

- Test name and path
- Failing assertion(s) and error messages
- Stack trace excerpt
- The exact lines of test code where failure occurred (with context)

3) Root cause analysis

- For each failure, identify the likely root cause (1-2 sentences)
- List potential code areas to inspect (files and line ranges)
- Suggest whether the fix should be in the test, the application, or test environment/setup

4) Suggested fixes (minimal and safe)

- Provide concrete code snippets or diffs to apply (with file paths and line numbers)
- If the fix is to the user manual, provide exact text to insert or replace in `pwtests/USER_MANUAL.md`

5) Regression and resilience recommendations

- Test retry/backoff strategies
- Better selectors or wait conditions
- Mocks/stubs for flaky network calls or external services

6) PR description & CHANGELOG

- Short PR title
- Detailed PR body explaining why changes were made
- One-sentence CHANGELOG entry
- List of files changed

7) Verification steps

- Exactly how to run the tests after changes, including commands and env variables
- Expected output for a successful run

8) Risk assessment

- Edge cases, possible regressions, assumptions made

Return the output as JSON with fields: `summary`, `failures`, `analysis`, `fixes`, `manual_updates`, `pr_description`, `changelog`, `verification`, `risk`.

---

Failure analysis checklist (quick summary for the AI):

- Check for selector changes due to UI text updates.
- Look for increased test timeouts or missing waits for asynchronous actions.
- Verify test data is present and correctly seeded.
- Inspect network calls / API mocks that may be failing.
- Confirm Playwright version compatibility and node/npm dependency issues.

Example outputs to request from AI

- A minimal code patch to fix a flaky selector
- An updated `pwtests/USER_MANUAL.md` section explaining how to run tests and common troubleshooting
- A PR description ready to paste into GitHub

Suggested PR template snippet

Title: "Fix Playwright scenario tests and update user manual"

Body:

- What: Fixed failing Playwright tests and updated `pwtests/USER_MANUAL.md` with troubleshooting steps.
- Why: Tests were failing due to selector changes and missing waits for network activity.
- How: Adjusted selectors, added explicit waits, and improved test setup documentation.
- Verification: Run `npx playwright test` and confirm all tests pass locally.

Output file location

- Save this prompt in the repo at `pwtests/analysis_prompt.md` so other developers can reuse it.

---

Notes

- Keep the prompt concise, explicit, and friendly. The goal is to allow a developer or CI pipeline to hand this to an LLM and receive a structured analysis and suggested code/docs updates.
