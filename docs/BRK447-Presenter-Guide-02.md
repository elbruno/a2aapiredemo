# BRK-447 Presenter Guide: Real-World AI Development with Visual Studio and GitHub Copilot

This concise presenter guide is optimized for a technical presenter delivering session BRK-447. It focuses on live/demonstration flow, essential prerequisites, and clear pre-session preparation.

---

## Overview

**Session Title**: BRK-447 - Real-World AI Development with Visual Studio and GitHub Copilot  
**Duration**: 40–45 minutes  
**Target Audience**: Developers, DevOps engineers, and technical architects

### Session Objectives

By the end of this session, attendees will be able to:

- Use GitHub Copilot effectively inside Visual Studio to accelerate development
- Leverage MCP servers to augment Copilot with external documentation and tools
- Use Copilot Agents to automate issue-driven code changes and UI updates

---

## Pre-Session / Presenter Pre-Setup

These items are required as presenter pre-work and should NOT be listed as session objectives.

**Essential Pre-Setup Requirements:**
- Review the Initial Setup guide and recorded setup video: `docs/brk447-01-Initial%20Setup-gen-01/brk447-01-Initial%20Setup-en-US-02-userguide.md` (include the recorded setup video in the same folder as pre-setup material)
- Set up Azure AI services and local development environment 
- Run coding agent actions before the event

**Prerequisites for Attendees and Presenter:**
- `.NET SDK` (repository targets .NET 9)
- `Visual Studio 2022`
- `Basic level access to and familiarity with GitHub Copilot` (sign-in + extension installed)

Optional pre-deploys (presenter choice): pre-deploy AI resources or collect connection strings if you plan to demo live services — otherwise use the recorded video segments described in Engagement Strategies.

---

## Demo Flow

All demos below reference the userguide files in the `docs/` folder. Links use forward slashes and are wrapped in code spans as requested.

Summary timeline (aim to keep total between 40–45 minutes):

| Demo | Estimated Time | Description |
|---|---:|---|
| Introduction & Agenda | 3 min | Brief session welcome and agenda. |
| Demo 1: Initial Setup | 6 min | Deploy models and configure local app |
| Demo 2: Architecture / Zava Overview | 5 min | High-level architecture and key components |
| Demo 3: Copilot Features in Visual Studio | 6 min | Ask vs Agent modes, completions, and quick edits |
| Demo 4: AI Search & Unit Testing | 6 min | TDD flow with Copilot scaffolding unit tests |
| Demo 5: MCP Servers | 4 min | MCP server role and verification |
| Demo 6: Querying Documentation (MCP) | 4 min | Agent consulting Microsoft Learn for code suggestions |
| Demo 7: Issue-Driven Development with Agent | 6 min | Issue->Agent plan->patch workflow |
| Demo 8: Image-Based UI Updates | 4 min | Before/after images and Agent UI suggestions |
| Q&A & Wrap-up | 2–3 min | Session wrap-up and questions |

**Total: 40–44 minutes** (adjust slightly as needed to remain within 40–45 minutes)

### Demo 1 — Initial Setup (6 min)

- Reference: `docs/brk447-01-Initial%20Setup-gen-01/brk447-01-Initial%20Setup-en-US-02-userguide.md`
- Focus: Deploy the models, configure local app, and verify deployments

### Demo 2 — Architecture / Zava Overview (5 min)

- Reference: `docs/brk447-02-Zava%20Overview-gen-01/brk447-02-Zava%20Overview-en-US-02-userguide.md`
- Focus: High-level architecture and key components — keep to the big picture

### Demo 3 — Copilot Features in Visual Studio (6 min)

- Reference: `docs/brk447-03-VS2022%20and%20GHCP%20Overview-gen-01/brk447-03-VS2022%20and%20GHCP%20Overview-en-US-02-userguide.md`
- Focus: Ask vs. Agent modes, quick examples of completion and simple modifications

### Demo 4 — AI Search & Unit Testing (6 min)

- Reference: `docs/brk447-04%20Add%20single%20unit%20Test%20for%20AI%20Search-gen-01/brk447-04%20Add%20single%20unit%20Test%20for%20AI%20Search-en-US-02-userguide.md`
- Focus: TDD flow with Copilot assisting to scaffold a unit test and validate results

### Demo 5 — MCP Servers (4 min)

- Reference: `docs/brk447-05%20add%20mcp%20servers-gen-01/brk447-05%20add%20mcp%20servers-en-US-02-userguide.md`
- Focus: Brief explanation and one quick verification of a configured MCP tool

### Demo 6 — Querying Documentation (MCP) (4 min)

- Reference: `docs/brk447-06-query%20mcp%20ms%20learn-gen-01/brk447-06-query%20mcp%20ms%20learn-en-US-02-userguide.md`
- Focus: Show how the Agent can consult Microsoft Learn or other docs to suggest code changes

### Demo 7 — Issue-Driven Development with Agent (6 min)

- Reference: `docs/brk447-07-Implement%20unit%20tests%20using%20GH%20Issue-gen-01/brk447-07-Implement%20unit%20tests%20using%20GH%20Issue-en-US-02-userguide.md`
- Focus: Show the issue briefly and the Agent's plan/patch workflow (use a short prerecorded clip if long)

### Demo 8 — Image-Based UI Updates (4 min)

- Reference: `docs/brk447-08-update%20ui%20using%20agent%20based%20on%20images-gen-01/brk447-08-update%20ui%20using%20agent%20based%20on%20images-en-US-02-userguide.md`
- Focus: Show before/after images and the Agent's suggested UI changes (use recorded clip if applying changes is time-consuming)

---

## Engagement Strategies

- **For demos that are long-running or brittle to run live, the presenter must use recorded videos with voice-over rather than attempting a live run.** Long demos must be demonstrated using pre-recorded video and a voice-over narration.
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
- PRD Template: `docs/04-PRD_Add_Payment_Mock_Server.md`
- Issue Creation Template: `docs/02-Create_Issue_for_unit_tests.md`