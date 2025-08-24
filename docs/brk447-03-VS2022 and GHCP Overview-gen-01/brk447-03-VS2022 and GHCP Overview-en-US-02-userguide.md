# GitHub Copilot in Visual Studio — User Manual

This manual explains how to use the GitHub Copilot integration inside Visual Studio (preview) as demonstrated in the video. It covers setup, inspecting consumption, working with solution architecture and Copilot modes, selecting OpenAI models (example: GPT-5), and asking Copilot to analyze and upgrade project and infrastructure files.

Total demo duration: 00:02:41.520

---

## Overview
(Reference timestamps shown throughout the manual.)

- GitHub Copilot is available as a Visual Studio preview extension that provides inline suggestions and a “next edit” suggestion while coding. (00:00:01.800 – 00:00:17.700)
- You can view consumption/usage of your Copilot plan (e.g., percent used for premium requests). (00:00:17.700 – 00:00:33.680)
- Typical solution components include orchestrators, backend, AI front-end and model configuration. Copilot supports two modes:
  - Ask mode: Q&A about the system.
  - Agent mode: Interactive, hands-on coding assistance. (00:00:33.680 – 00:01:02.600)
- You can query which OpenAI model is in use, switch models via a selector (example: GPT-5), and view a full list of available models. (00:01:02.600 – 00:01:54.560)
- Copilot can analyze configured models and embeddings, create an upgrade plan (e.g., to GPT-5), consider local models, and apply code and infrastructure edits (C# and Bicep). (00:01:54.560 – 00:02:41.520)

---

## Step-by-step instructions

Note: timestamps in parentheses indicate where the action appears in the video.

### 1. Install and open GitHub Copilot in Visual Studio (Preview)
(00:00:01.800 – 00:00:17.700)

1. Install the GitHub Copilot extension for Visual Studio — use the preview release if you want the features shown in the demo.
2. Open Visual Studio and load your solution/project.
3. Enable the Copilot extension if prompted.
4. Start typing in an editor file and observe:
   - Inline suggestions appear while you type.
   - The “next edit” suggestion UI proposes follow-up edits across the file.

Tips:
- Preview extensions may change frequently; check release notes.
- If inline suggestions do not appear, confirm the extension is enabled and that you are signed into the associated GitHub/organization account.

Warnings:
- Preview features may be unstable — save your work frequently.

### 2. Check consumption and plan status
(00:00:17.700 – 00:00:33.680)

1. Open the Copilot consumption/usage panel from the Copilot UI or Visual Studio extension controls.
2. Inspect the usage meter. Example shown: 28% of the pro plan used for premium requests.
3. Verify available quotas and other resources (e.g., remaining premium request quota).

Tips:
- Keep an eye on “premium request” usage if you rely on higher-tier model access.
- If you approach quota limits, plan for additional capacity or change model usage.

### 3. Inspect solution architecture and choose Copilot mode
(00:00:33.680 – 00:01:02.600)

1. Open Solution Explorer and identify primary components:
   - Orchestrators
   - Backend services
   - AI front-end
   - Model definitions and embeddings
2. Open the Copilot pane and choose the appropriate mode:
   - Ask mode: Use for questions about system behavior, design choices, or quick documentation. Good for read-only Q&A.
   - Agent mode: Use for interactive, guided changes — instruct Copilot to make edits or run a sequence of steps.

When to use which mode:
- Use Ask mode to understand what the system currently does or which model is configured.
- Use Agent mode when you want Copilot to propose or apply changes in code or infrastructure.

Tip:
- If you’re unsure, start in Ask mode to gather context, then switch to Agent mode for changes.

### 4. Query and select a model (example: GPT-5)
(00:01:02.600 – 00:01:54.560)

1. In the Copilot Q&A interface, ask: “Which OpenAI model is this solution using?”
2. Review Copilot’s response about the currently configured model(s).
3. To change the model:
   - Open the model selector/dropdown in the Copilot UI.
   - Browse the list of available models (the selector contains a full set).
   - Select your desired model (example: GPT-5).
4. Confirm that the UI now reflects your selected model.

Tips:
- Model selection affects behavior and cost — ensure compatibility with code and infra changes.
- After switching models, re-run any Copilot analyses to see updated recommendations.

Warnings:
- Model changes can affect query latency, output quality, and billing. Confirm plan compatibility before switching to premium models.

### 5. Ask Copilot to analyze configured models and create an upgrade plan
(00:01:54.560 – 00:02:41.520)

1. Ask Copilot to analyze project model configuration and embeddings. Example prompts:
   - “Analyze which models and embeddings are configured in this solution.”
   - “List where models are defined and which services use them.”
2. Request an upgrade plan. Example prompts:
   - “Create a plan to upgrade this solution to use GPT-5.”
   - “Consider switching to local models — compare the implications and steps.”
3. Review Copilot’s proposed plan and recommendations in the response pane.
   - The plan may include changes to application code and infrastructure templates.
4. If you want Copilot to apply edits:
   - Use the Copilot edit/perform-changes action (Agent mode).
   - Allow Copilot to modify relevant files (examples: C# code files and Bicep infrastructure-as-code templates).
5. Review the proposed edits:
   - Inspect diffs in the IDE.
   - Approve or reject individual changes as needed.
6. After accepting edits:
   - Run build and test suites.
   - Validate any infrastructure changes in a staging environment before production deployment.

Tips:
- Have source control and a working CI pipeline in place to review and test changes.
- For infra changes (Bicep), run a dry-run or plan operation to validate resource changes before applying.

Warnings:
- Always review code and infra changes suggested by AI before committing.
- Back up critical files or create a feature branch before applying automated changes.

---

## UI elements referenced
- GitHub Copilot extension (Visual Studio preview)
- Inline suggestion UI
- Next edit suggestion
- Consumption/usage meter and pro plan indicator
- Solution Explorer (orchestrators, backend, front end, models)
- Copilot Ask mode and Agent mode controls
- Model selector/dropdown (e.g., GPT-5)
- Copilot response/edit pane
- Project files: C# source files
- Infrastructure-as-code files: Bicep templates

---

## Helpful tips and best practices
- Use Ask mode to gather context before making edits with Agent mode.
- Monitor the consumption meter to avoid unexpected billing from premium model usage.
- Always create a branch and run automated tests before merging Copilot-applied edits.
- For infra changes, perform staged rollouts and validate with dry-runs (ARM/Bicep plan) first.
- Keep the Copilot extension up to date (preview channel if you need preview features).

---

Timestamps quick reference
- Introduction to Copilot in Visual Studio — 00:00:01.800 to 00:00:17.700
- Consumption / Plan status — 00:00:17.700 to 00:00:33.680
- Solution architecture and Copilot modes — 00:00:33.680 to 00:01:02.600
- Model selection and querying Copilot — 00:01:02.600 to 00:01:54.560
- Copilot analysis and upgrade actions (C# and Bicep changes) — 00:01:54.560 to 00:02:41.520

---

If you follow these steps you should be able to reproduce the behaviors shown in the demo: viewing inline suggestions, checking plan usage, querying and switching models (e.g., to GPT-5), and letting Copilot analyze and propose or apply changes to code and infrastructure files.