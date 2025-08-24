# BRK-447 Presenter Guide: Real-World AI Development with Visual Studio and GitHub Copilot

<!-- NOTE: Updated per copilot prompt - trimmed prerequisites, moved setup/pre-work to Pre-Session, renamed Modules to Demos, and constrained total session time to 40–45 minutes. -->

This concise presenter guide is optimized for a technical presenter delivering session BRK-447. It focuses on live/demonstration flow, essential prerequisites, and clear pre-session preparation.

---

## Overview

**Session Title**: BRK-447 - Building AI-Powered Applications with Visual Studio and GitHub Copilot  
**Duration**: 40–45 minutes  
**Target Audience**: Developers, DevOps engineers, and technical architects

<!-- Rationale: Reduced duration to 40–45 minutes per prompt requirement. -->

### Session Objectives

By the end of this session, attendees will be able to:

- Use GitHub Copilot effectively inside Visual Studio to accelerate development
- Leverage MCP servers to augment Copilot with external documentation and tools
- Implement AI search functionality and validate it with unit tests
- Use Copilot Agents to automate issue-driven code changes and UI updates

<!-- Rationale: Removed objectives that are pre-session work (Azure setup and running coding agent actions). Those items moved to Pre-Session section below. -->

---

## Pre-Session / Presenter Pre-Setup

These items are required as presenter pre-work and should NOT be listed as session objectives.

- Review the Initial Setup guide and recorded setup video: `docs/brk447-01-Initial%20Setup-gen-01/brk447-01-Initial%20Setup-en-US-02-userguide.md` (the same folder contains the recorded setup video).  <!-- Rationale: Explicit reference required by prompt -->
- Ensure the following minimal prerequisites are installed and available:
  - `.NET SDK` — repository targets `dotnet 9` (see `global.json`)  <!-- Rationale: use repo target -->
  - `Visual Studio 2022` (recommended for full-solution experience)  <!-- Rationale: mention VS2022 if repo references it -->
  - Basic level access to and familiarity with GitHub Copilot (signed-in and extension installed)

<!-- Rationale: Removed all other non-essential prerequisites (Docker, Azure CLI, Git workflow instructions) per prompt. -->

Optional pre-deploys (presenter choice): pre-deploy AI resources or collect connection strings if you plan to demo live services — otherwise use the recorded video segments described in Engagement Strategies.

---

## Demo Flow (rename: Demo = formerly Module)

All demos below reference the userguide files in the `docs/` folder. Links use forward slashes and are wrapped in code spans as requested.

Summary timeline (aim to keep total between 40–45 minutes):

- Introduction & Agenda — 3 min
- Demo 1: Initial Setup — 6 min  (`docs/brk447-01-Initial%20Setup-gen-01/brk447-01-Initial%20Setup-en-US-02-userguide.md`)
- Demo 2: Architecture / Zava Overview — 5 min  (`docs/brk447-02-Zava%20Overview-gen-01/brk447-02-Zava%20Overview-en-US-02-userguide.md`)
- Demo 3: Copilot Features in Visual Studio — 6 min  (`docs/brk447-03-VS2022%20and%20GHCP%20Overview-gen-01/brk447-03-VS2022%20and%20GHCP%20Overview-en-US-02-userguide.md`)
- Demo 4: AI Search & Unit Testing — 6 min  (`docs/brk447-04%20Add%20single%20unit%20Test%20for%20AI%20Search-gen-01/brk447-04%20Add%20single%20unit%20Test%20for%20AI%20Search-en-US-02-userguide.md`)
- Demo 5: MCP Servers — 4 min  (`docs/brk447-05%20add%20mcp%20servers-gen-01/brk447-05%20add%20mcp%20servers-en-US-02-userguide.md`)
- Demo 6: Querying Documentation (MCP) — 4 min  (`docs/brk447-06-query%20mcp%20ms%20learn-gen-01/brk447-06-query%20mcp%20ms%20learn-en-US-02-userguide.md`)
- Demo 7: Issue-Driven Development with Agent — 6 min  (`docs/brk447-07-Implement%20unit%20tests%20using%20GH%20Issue-gen-01/brk447-07-Implement%20unit%20tests%20using%20GH%20Issue-en-US-02-userguide.md`)
- Demo 8: Image-Based UI Updates — 4 min  (`docs/brk447-08-update%20ui%20using%20agent%20based%20on%20images-gen-01/brk447-08-update%20ui%20using%20agent%20based%20on%20images-en-US-02-userguide.md`)
- Q&A & Wrap-up — 3 min

Total: 43 minutes  <!-- Rationale: sum fits 40–45 minute requirement -->

For each demo, use the referenced userguide for talking points and follow a short 1–2 step demo flow.

If a demo is fragile or long-running, follow Engagement Strategies below and play a recorded segment.

### Demo 1 — Initial Setup (6 min)

- Reference: `docs/brk447-01-Initial%20Setup-gen-01/brk447-01-Initial%20Setup-en-US-02-userguide.md`
- Focus: highlight what was completed during pre-session setup and show a short verification step (e.g., open solution, confirm restore/build).

### Demo 2 — Zava Architecture Overview (5 min)

- Reference: `docs/brk447-02-Zava%20Overview-gen-01/brk447-02-Zava%20Overview-en-US-02-userguide.md`
- Focus: high-level architecture and key components (no deep dive).

### Demo 3 — Copilot Features in Visual Studio (6 min)

- Reference: `docs/brk447-03-VS2022%20and%20GHCP%20Overview-gen-01/brk447-03-VS2022%20and%20GHCP%20Overview-en-US-02-userguide.md`
- Focus: Ask vs. Agent modes, quick examples of completion and simple modifications.

### Demo 4 — AI Search & Unit Testing (6 min)

- Reference: `docs/brk447-04%20Add%20single%20unit%20Test%20for%20AI%20Search-gen-01/brk447-04%20Add%20single%20unit%20Test%20for%20AI%20Search-en-US-02-userguide.md`
- Focus: TDD flow with Copilot assisting to scaffold a unit test and validate results.

### Demo 5 — MCP Servers (4 min)

- Reference: `docs/brk447-05%20add%20mcp%20servers-gen-01/brk447-05%20add%20mcp%20servers-en-US-02-userguide.md`
- Focus: brief explanation and one quick verification of a configured MCP tool.

### Demo 6 — Querying Documentation (MCP) (4 min)

- Reference: `docs/brk447-06-query%20mcp%20ms%20learn-gen-01/brk447-06-query%20mcp%20ms%20learn-en-US-02-userguide.md`
- Focus: show how the Agent can consult Microsoft Learn or other docs to suggest code changes.

### Demo 7 — Issue-Driven Development with Agent (6 min)

- Reference: `docs/brk447-07-Implement%20unit%20tests%20using%20GH%20Issue-gen-01/brk447-07-Implement%20unit%20tests%20using%20GH%20Issue-en-US-02-userguide.md`
- Focus: show the issue briefly and the Agent's plan/patch workflow (use a short prerecorded clip if long).

### Demo 8 — Image-Based UI Updates (4 min)

- Reference: `docs/brk447-08-update%20ui%20using%20agent%20based%20on%20images-gen-01/brk447-08-update%20ui%20using%20agent%20based%20on%20images-en-US-02-userguide.md`
- Focus: show before/after images and the Agent's suggested UI changes (use recorded clip if applying changes is time-consuming).

---

## Engagement Strategies

- For demos that are long-running, environment-dependent, or brittle to run live, play pre-recorded video clips with a voice-over narration rather than attempting a live run.  <!-- Rationale: required by prompt to instruct recorded videos for brittle/long demos -->
- Keep live demos short (30–90 seconds) and reserve complex changes for recorded segments.
- Invite audience questions during short pauses between demos and use the final Q&A for deeper discussion.

---

## Presenter Notes and Quick Tips

- Keep the session focused on practical demonstrations and avoid deep infrastructure tasks during the live session.
- If you must show environment setup, show only verification steps during the session and leave full setup to the pre-session guide.
- Use the referenced userguides for copy/paste snippets and backup clips.

---

## Quick Reference Links

### Essential Documentation

- Initial Setup Guide: `docs/01-InitialSetup.md`
- PRD Template: `docs/PRD_Add_Payment_Mock_Server.md`
- Issue Creation Template: `docs/Create_Issue_for_unit_tests.md`

### Demo Userguides

- Demo 1: `docs/brk447-01-Initial%20Setup-gen-01/brk447-01-Initial%20Setup-en-US-02-userguide.md`
- Demo 2: `docs/brk447-02-Zava%20Overview-gen-01/brk447-02-Zava%20Overview-en-US-02-userguide.md`
- Demo 3: `docs/brk447-03-VS2022%20and%20GHCP%20Overview-gen-01/brk447-03-VS2022%20and%20GHCP%20Overview-en-US-02-userguide.md`
- Demo 4: `docs/brk447-04%20Add%20single%20unit%20Test%20for%20AI%20Search-gen-01/brk447-04%20Add%20single%20unit%20Test%20for%20AI%20Search-en-US-02-userguide.md`
- Demo 5: `docs/brk447-05%20add%20mcp%20servers-gen-01/brk447-05%20add%20mcp%20servers-en-US-02-userguide.md`
- Demo 6: `docs/brk447-06-query%20mcp%20ms%20learn-gen-01/brk447-06-query%20mcp%20ms%20learn-en-US-02-userguide.md`
- Demo 7: `docs/brk447-07-Implement%20unit%20tests%20using%20GH%20Issue-gen-01/brk447-07-Implement%20unit%20tests%20using%20GH%20Issue-en-US-02-userguide.md`
- Demo 8: `docs/brk447-08-update%20ui%20using%20agent%20based%20on%20images-gen-01/brk447-08-update%20ui%20using%20agent%20based%20on%20images-en-US-02-userguide.md`

---

<!-- NOTE: Removed sections per prompt: "Backup Plans and Troubleshooting", "Post-Session Resources", and "Success Metrics" were intentionally omitted to keep the guide concise and aligned with the requested content model. -->

*This presenter guide was updated to comply with the generator prompt and focuses on a 40–45 minute demo-oriented delivery; modify timings as needed for the event slot.*
