# Video: [brk447-03-VS2022 and GHCP Overview.mkv](./REPLACE_WITH_VIDEO_LINK) — 00:02:43

# GitHub Copilot in Visual Studio — User Manual

This manual guides you through using GitHub Copilot integrated into Visual Studio as demonstrated in the analyzed video. It explains how to enable Copilot in a preview build, use inline suggestions and the Next‑Edit feature, check usage/consumption, switch Copilot working modes (Ask vs Agent), select models (example: GPT‑5), read configured-model responses, and request Copilot to create plans and modify code (C#) and infrastructure (Bicep).

- Total demo duration reference: 00:00:01.800 → 00:02:41.520

---

## Overview (00:00:01.800 — 00:02:41.520)

- GitHub Copilot is integrated into Visual Studio as an extension/pane. The demo uses a preview build — expect changes in future releases. (00:00:01.800 — 00:00:12.960)
- Key capabilities shown:
  - Inline suggestions while typing and a "Next‑Edit" suggestion feature (00:00:12.960 — 00:00:31.880)
  - Monitoring consumption/usage for Pro/Premium requests (00:00:12.960 — 00:00:31.880)
  - Two working modes: Ask (question/response) and Agent (interactive side‑by‑side coding) (00:00:31.880 — 00:00:55.520)
  - Selecting models (example: choosing GPT‑5) and reading which models are configured for chat and embeddings (00:00:55.720 — 00:02:11.680)
  - Requesting plans and automated code/infrastructure edits (C# and Bicep) to upgrade models or switch to local models (00:02:11.680 — 00:02:41.520)

---

## Step‑by‑Step Instructions

Follow the sections below in order to reproduce the actions shown in the video.

---

### 1. Open Visual Studio with GitHub Copilot enabled
Timestamp: 00:00:01.800 — 00:00:12.960

Purpose: Start Visual Studio with the Copilot extension active (preview build used in demo).

Steps:
1. Launch Visual Studio.
2. Verify the GitHub Copilot extension is installed and enabled. Look for a Copilot pane/indicator in the IDE.
3. If you are using a preview build, confirm the preview version indicator is visible — this indicates features may change.

UI elements:
- GitHub Copilot integration pane/extension
- Preview version indicator

Tips:
- If Copilot is not present, install it from Visual Studio Marketplace or the Tools → Extensions menu.
- Sign in with your GitHub account if required.

Warning:
- Preview builds may introduce UI and behavior changes. Do not rely on preview-only behavior for critical workflows.

---

### 2. Use Inline Suggestions and the Next‑Edit feature
Timestamp: 00:00:12.960 — 00:00:31.880

Purpose: Accept inline suggestions while coding and invoke the Next‑Edit suggestion for context-aware, successive changes.

Steps:
1. Open a code file in your project and begin typing where you want assistance.
2. Watch for the inline suggestion widget — Copilot proposes completions as you type.
3. Accept a suggestion using the inline suggestion controls (e.g., on‑screen accept button or your configured keyboard shortcut).
4. To request the next context-aware edit, use the "Next‑Edit" control in the Copilot UI. This prompts Copilot to propose the next logical modification or refinement.

UI elements:
- Inline suggestion widget
- Next‑Edit suggestion control

Tips:
- If you don’t see suggestions, ensure Copilot is enabled for the current file / language.
- Many users map keyboard shortcuts to quickly accept suggestions — check Visual Studio keyboard settings for Copilot bindings.

---

### 3. Check usage / consumption (Pro plan / Premium requests)
Timestamp: 00:00:12.960 — 00:00:31.880

Purpose: Monitor how many premium or Pro plan requests you’ve used (helpful when using paid features).

Steps:
1. Open the Copilot consumption or usage panel inside Visual Studio. This may be accessible from the Copilot pane or a dedicated menu.
2. Review the usage indicator. The demo shows a 28% usage example.

UI elements:
- Consumption / usage panel
- Pro plan / premium request indicator

Tips:
- Track consumption regularly if you are on a metered plan.
- If you approach your quota, switch to fewer premium requests or request additional quota through your subscription.

---

### 4. Understand solution architecture and choose Copilot working mode
Timestamp: 00:00:31.880 — 00:00:55.520

Purpose: Learn the high‑level components of the sample solution and select between Ask and Agent modes depending on workflow.

Steps:
1. Inspect the project structure in Solution Explorer to identify key components: orchestrators, backend, frontend, and models.
2. Open the Copilot interface and choose the desired mode:
   - Ask mode: Use a question/response interface for explanations, plans, and targeted queries.
   - Agent mode: Use the interactive assistant for side‑by‑side coding actions and automated edits.

UI elements:
- Ask mode (question/response interface)
- Agent mode (interactive coding assistant)
- Project files: orchestrators, backend, frontend, models

Tips:
- Use Ask mode for planning, high‑level questions, or to get an explanation of configuration.
- Use Agent mode when you want Copilot to propose or apply code changes directly.

---

### 5. Select a model (example: choosing GPT‑5)
Timestamp: 00:00:55.720 — 00:01:12.440

Purpose: Select or change the OpenAI model used by the Copilot instance for chat/embeddings.

Steps:
1. Open the model selector in the Copilot UI (look for a dropdown or settings area that lists available models).
2. Review the model list (examples: GPT‑5, GPT‑4.1 mini, embeddings models).
3. Select the desired model (e.g., choose GPT‑5) from the dropdown.

UI elements:
- Model selector / dropdown
- Model list: GPT‑5, GPT‑4.1 mini, embeddings models

Tips:
- Different models have different capabilities and costs — confirm suitability before switching.
- Model selection may affect latency, cost, and available features like embeddings or chat.

Warning:
- Changing models can affect behavior across your solution. Test changes in a safe environment before applying them to production workflows.

---

### 6. Read Copilot’s response about configured models and embeddings
Timestamp: 00:01:54.560 — 00:02:11.680

Purpose: Confirm which models are configured in the sample solution (e.g., which model is used for embeddings vs chat).

Steps:
1. In Ask mode, ask Copilot which OpenAI models are configured in the solution (or examine the AppHost configuration file referenced in responses).
2. Read Copilot’s answer in the response pane. The demo response notes AppHost defines specific models, such as:
   - GPT‑4.1 mini for chat
   - A specific embeddings model for vector searches
3. Confirm which components (backend, AppHost) use embeddings or chat features based on the Copilot response.

UI elements:
- Copilot answer/response pane
- AppHost configuration reference shown in the response

Tips:
- Use Copilot to quickly summarize configuration files so you don’t have to read every line manually.
- If configuration details are ambiguous, ask Copilot to show the exact lines or files it used to draw conclusions.

---

### 7. Request a plan to upgrade models and apply code/infrastructure changes
Timestamp: 00:02:11.680 — 00:02:41.520

Purpose: Ask Copilot to create a plan to upgrade configured models (e.g., upgrade to GPT‑5), consider switching to local models, and implement changes across code (C#) and infrastructure (Bicep).

Steps:
1. In Ask or Agent mode, type a request such as:
   - "Create a plan to upgrade the configured models to GPT‑5."
   - "Consider switching to local models and outline required changes."
2. Review Copilot’s proposed plan in the response pane. The plan should include:
   - Code changes needed in the backend (C#) to reference the new model.
   - Infrastructure changes in Bicep or IaC files to support model endpoints, authentication, or compute resources.
3. Ask Copilot to apply the changes:
   - In Agent mode, request Copilot to modify the necessary C# files and Bicep files.
   - Review the suggested edits in the change suggestion pane.
4. Accept and commit the changes if they are correct, run any required build or deployment steps, and test in a non‑production environment.

UI elements:
- Ask interface for planning and action requests
- Change suggestion pane for automated edits
- Code files (C#)
- Bicep files (infrastructure as code)

Tips:
- Request a stepwise plan first, then ask Copilot to implement changes to minimize risk.
- Use version control (git) to review and revert changes if needed.

Warnings:
- Always review and test automated code and infrastructure edits before deploying.
- Infrastructure changes (Bicep) can affect billing and environment state—apply them in a development or staging environment first.
- When switching to local models, consider security, compute requirements, and licensing.

---

## Tips & Best Practices (general)

- Keep Copilot and Visual Studio updated. Preview features may move to stable releases.
- Use source control for all changes Copilot applies so you can review diffs and revert if necessary.
- Monitor consumption frequently if you are on a metered or Pro plan.
- When making model upgrades, confirm compatibility with existing code paths (embedding sizes, API differences, rate limits).
- For production critical infrastructure changes, follow your organization’s change management and testing procedures.

---

Timestamps quick reference:
- Introduction and preview note: 00:00:01.800 — 00:00:12.960
- Inline suggestions and Next‑Edit, usage panel: 00:00:12.960 — 00:00:31.880
- Solution overview and modes: 00:00:31.880 — 00:00:55.520
- Model selection example (GPT‑5): 00:00:55.720 — 00:01:12.440
- Copilot response on configured models: 00:01:54.560 — 00:02:11.680
- Requesting upgrade plan and code/infrastructure edits: 00:02:11.680 — 00:02:41.520

This manual should enable you to follow the same workflow demonstrated in the video: enabling Copilot in Visual Studio, using inline suggestions and Next‑Edit, monitoring usage, choosing models, reviewing configured models, and requesting/implementing upgrades in both code and infrastructure files.