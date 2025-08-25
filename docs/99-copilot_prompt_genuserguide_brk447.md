# Copilot prompt: Generate/Update BRK-447 Presenter Guide

Generate or update the Markdown presenter guide for session BRK-447. The target file is `docs/BRK447-Presenter-Guide.md` — if it exists, update it; otherwise create it.
Use the repository documentation (the `docs/` folder and the `brk447-**` reference guides, each one in a different folder) as the authoritative context.

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
  - Always use the termo "Demo" to refer to each one of the folders throughout the generated guide.
  - The presenter guide should not show per-demo durations.
  - the Demo should be presented in a table that shows:
    - the demo name. this should also link to the subfolder minimal document of the demo.
    - the demo description.

- Sections to not generate or remove from the generated presenter guide: do not include the following sections in the final guide (remove them entirely):
  - `## Backup Plans and Troubleshooting`
  - `## Post-Session Resources`
  - `### Success Metrics`

- Engagement Strategies: for demos that are long-running or brittle to run live, the presenter guide must instruct the presenter to use recorded videos with voice-over rather than attempting a live run. Add a clear note in `### Engagement Strategies` that long demos must be demonstrated using pre-recorded video and a voice-over narration.

Deliverable expectations for this prompt:

- Only update `docs/BRK447-Presenter-Guide.md` when applying the generated content — this prompt must produce that file. (Note: the current request asked to update `docs/99-copilot_prompt_genuserguide_brk447.md` with these changes; this file should instruct the generator to update the presenter guide accordingly.)
- The generated presenter guide should be concise, practical for a technical presenter, and must reflect the changes above.

End of prompt.
