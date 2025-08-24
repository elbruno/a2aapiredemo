# Copilot prompt: Generate/Update BRK-447 Presenter Guide

Generate or update the Markdown presenter guide for session BRK-447. The target file is `docs/BRK447-Presenter-Guide.md` — if it exists, update it; otherwise create it. Use the repository documentation (the `docs/` folder and the `brk447-**` reference guides) as the authoritative context.

Key requirements (apply all in the generated presenter guide):

- Session duration: set the overall session time to **40–45 minutes**.

- Prerequisites: reduce to only the essential items required for attendees and the presenter during the session. Remove any mention of Git workflows. The presenter guide should list only:
  - `.NET SDK` (use the repository's target version, e.g. `dotnet 9` if present)
  - `Visual Studio` (mention Visual Studio 2022 if the repo references it)
  - `Basic level access to and familiarity with GitHub Copilot` (sign-in + extension installed)

- Session objectives: remove items that are actually part of presenter pre-work. Specifically, do NOT include these as session objectives:
  - "Set up Azure AI services and local development environment"
  - "Run coding agent actions before the event"

  Instead, place those items in a clear **Pre-Session / Presenter Pre-Setup** section and reference the initial setup guide and video. Add an explicit pre-setup bullet that references the Initial Setup guide and video folder:
  - Reference: `docs/brk447-01-Initial%20Setup-gen-01/brk447-01-Initial%20Setup-en-US-02-userguide.md` (include link and mention the recorded setup video in the same folder as pre-setup material).

- Demo flow language and durations:
  - Replace the term "Module" with the term "Demo" throughout the generated guide.
  - The presenter guide should show per-demo durations that together fit the overall 40–45 minute session. Use the existing reference guides (the `brk447-**` userguide files) as the source of truth for demo content and relative weight, but scale durations so the total is 40–45 minutes.
  - Suggested allocation (example to use or adapt in the generated guide so it fits 40–45 min):
    - Introduction & Agenda — 3 min
    - Demo 1: Initial Setup — 6 min (reference `docs/brk447-01-Initial%20Setup-gen-01/brk447-01-Initial%20Setup-en-US-02-userguide.md`)
    - Demo 2: Architecture / Zava Overview — 5 min (reference `docs/brk447-02-Zava%20Overview-gen-01/brk447-02-Zava%20Overview-en-US-02-userguide.md`)
    - Demo 3: Copilot Features in Visual Studio — 6 min (reference `docs/brk447-03-VS2022%20and%20GHCP%20Overview-gen-01/brk447-03-VS2022%20and%20GHCP%20Overview-en-US-02-userguide.md`)
    - Demo 4: AI Search & Unit Testing — 6 min (reference `docs/brk447-04%20Add%20single%20unit%20Test%20for%20AI%20Search-gen-01/brk447-04%20Add%20single%20unit%20Test%20for%20AI%20Search-en-US-02-userguide.md`)
    - Demo 5: MCP Servers — 4 min (reference `docs/brk447-05%20add%20mcp%20servers-gen-01/brk447-05%20add%20mcp%20servers-en-US-02-userguide.md`)
    - Demo 6: Querying Documentation (MCP) — 4 min (reference `docs/brk447-06-query%20mcp%20ms%20learn-gen-01/brk447-06-query%20mcp%20ms%20learn-en-US-02-userguide.md`)
    - Demo 7: Issue-Driven Development with Agent — 6 min (reference `docs/brk447-07-Implement%20unit%20tests%20using%20GH%20Issue-gen-01/brk447-07-Implement%20unit%20tests%20using%20GH%20Issue-en-US-02-userguide.md`)
    - Demo 8: Image-Based UI Updates — 4 min (reference `docs/brk447-08-update%20ui%20using%20agent%20based%20on%20images-gen-01/brk447-08-update%20ui%20using%20agent%20based%20on%20images-en-US-02-userguide.md`)
    - Q&A & Wrap-up — 2–3 min
  - Total: 40–44 minutes (adjust slightly as needed to remain within 40–45 minutes)

- Sections to remove from the generated presenter guide: do not include the following sections in the final guide (remove them entirely):
  - `## Backup Plans and Troubleshooting`
  - `## Post-Session Resources`
  - `### Success Metrics`

- Engagement Strategies: for demos that are long-running or brittle to run live, the presenter guide must instruct the presenter to use recorded videos with voice-over rather than attempting a live run. Add a clear note in `### Engagement Strategies` that long demos must be demonstrated using pre-recorded video and a voice-over narration.

- Links and formatting: use forward slashes in links and wrap file paths in inline code spans (for example: `docs/brk447-01-Initial%20Setup-gen-01/...`). Ensure the generated Markdown uses a top-level heading as the first line and avoids hard tabs.

Deliverable expectations for this prompt:

- Only update `docs/BRK447-Presenter-Guide.md` when applying the generated content — this prompt must produce that file. (Note: the current request asked to update `docs/99-copilot_prompt_genuserguide_brk447.md` with these changes; this file should instruct the generator to update the presenter guide accordingly.)
- The generated presenter guide should be concise, practical for a technical presenter, and must reflect the changes above.

End of prompt.
