# Video: [brk447-08-update ui using agent based on images.mp4](./REPLACE_WITH_VIDEO_LINK) — 00:05:05

# Using Multimodal Agents (Copilot/Compiler) to Apply Branding and Improve UI — User Manual

This manual shows how to use a multimodal agent (GitHub Copilot / Compiler) to apply branding assets and iteratively improve a Blazor front-end UI, as demonstrated in the video. Follow the steps to copy branding assets, attach visual references, let the agent analyze and edit code (CSS / Razor components), and verify changes with hot reload and browser preview.

Duration referenced in the video: 00:00:00 — 00:05:04

---

## Overview

This workflow uses a multimodal agent to:

- Read your project and locate front-end files (Blazor Razor components, CSS).
- Apply branding assets (images, colors) to update styling (notably app.css).
- Accept screenshots and visual references to iteratively refine UI contrast and layout.
- Use Blazor hot reload and browser refresh to preview changes quickly.

Key concepts:
- Attach reference images and style assets to the agent's context.
- Allow the agent to scan the repo and propose/implement edits.
- Use small, incremental changes and iterate using screenshots and prompts.

Relevant timestamps:
- Intro and goal: 00:00:00–00:00:08  
- Copy style assets: 00:00:09–00:00:35  
- Attach visual references and prompt: 00:00:35–00:01:04  
- Agent analysis and edits (app.css): 00:01:09–00:02:09  
- Preview, screenshot, iterate: 00:02:09–00:03:16  
- Iterative improvement & hot reload: 00:03:16–00:04:33  
- Verify product UI and cart: 00:04:33–00:05:04

---

## Step-by-step instructions

Follow these numbered steps to reproduce the process.

1. Prepare your workspace
   - Open the repository in your IDE that is connected to Copilot/Compiler (or the multimodal agent feature).  
   - Confirm you can run the Blazor application locally (development server) so you can preview changes.  
   - Tip: Work on a development branch and have a backup or version control commit before large automated edits.

2. Identify and copy branding/style assets (00:00:09–00:00:35)
   - In the repository file list, locate branding assets: images, reference images, color information, or an existing style file.  
   - Copy the branding/style file that contains the SAVA color palette or other details into your project folder (for example, into a styles or assets directory).  
   - Paste or attach that file into the Copilot/Compiler input/context area so the agent can access the same files you did.
   - Tip: Keep filenames descriptive (e.g., sava-branding.json, sava-colors.css).

3. Attach visual references and craft the initial prompt (00:00:35–00:01:04)
   - Open and attach reference images (two or more images that define the desired look). Use the agent’s image paste/upload area.  
   - In the Copilot/Compiler prompt input, give a concise instruction: e.g., “Use these SAVA branding images and colors to update the site’s UI—improve header, product cards, and general color contrast.”  
   - Include which files to prioritize (app.css, product.razor, shared components) if possible.
   - Tip: Provide specific goals (e.g., lighter button background, improved text contrast).

4. Allow the agent to analyze the project and plan edits (00:01:09–00:02:09)
   - Let Copilot/Compiler scan the repository. It will identify front-end files such as Razor components, CSS (app.css), and component tracers.  
   - The agent will propose a plan and begin implementing edits (often starting with app.css).  
   - Warning: The agent may need to stop your running application to update files.

5. Respond to the running-application popup and restart app (00:01:09–00:02:09)
   - If a popup appears asking to stop the running application to apply changes, accept it so file edits can be applied.  
   - After edits are applied, restart the development server (or let hot reload reapply changes).  
   - Tip: If hot reload is active, the agent’s changes may appear without a full restart—verify your environment’s behavior.

6. Preview updated styles in the browser and inspect (00:02:09–00:03:16)
   - Refresh the browser (use the preview tab or browser reload) to load updated styles.  
   - Inspect areas affected by the changes: header, product cards, buttons, cart UI.  
   - Note issues (e.g., colors too dark, poor contrast, spacing problems).

7. Capture and submit a screenshot for iterative refinement (00:02:09–00:03:16)
   - Use your OS or browser screenshot tool to capture the problematic view. Include the area that needs improvement.  
   - Paste or upload the screenshot into Copilot/Compiler and add a short description with the issue (e.g., “Buttons appear too dark and low contrast on product cards; please increase contrast and adjust text color.”)  
   - Tip: Crop to the relevant UI area and add a one-line instruction for clarity.

8. Wait for agent analysis and accept incremental changes (00:03:16–00:04:33)
   - The agent will analyze the screenshot and may take time (minutes) to process and apply improved edits.  
   - Prefer smaller incremental edits rather than bulk changes. This improves the chance that hot reload and previews will quickly show useful results.  
   - Tip: If the agent’s changes are not satisfactory, provide targeted feedback and another screenshot.

9. Use Blazor hot reload for quick verification (00:03:16–00:04:33)
   - After each set of edits, rely on Blazor hot reload to see style changes rapidly in running pages (e.g., product.razor).  
   - Refresh specific pages if changes don’t appear immediately.  
   - Warning: Some edits require stopping and restarting the dev server—follow the agent’s prompts.

10. Test interactive UI elements (00:04:33–00:05:04)
    - Navigate to the product page and inspect updated product cards and layout.  
    - Add items to the cart to verify cart UI, button states, and spacing.  
    - Identify any remaining issues (e.g., cart contrast, alignment) and repeat the screenshot → prompt → apply cycle as necessary.

11. Iterate until satisfied
    - Continue the cycle of: observe → capture → prompt the agent with images/instructions → accept edits → preview.  
    - Keep iterations small and focused (contrast, spacing, typography) for faster, reliable results.  
    - Tip: Use precise feedback (hex color suggestions, contrast ratios, font sizes) to guide the agent’s changes.

---

## Useful tips and warnings

- Tips
  - Provide direct, focused prompts with images and specific goals (e.g., “increase button contrast to WCAG AA”).
  - Use small, incremental edits to leverage hot reload and reduce disruptive changes.
  - Attach multiple reference images (colors, spacing, typography) to better convey design intent.
  - Name style files and assets clearly so the agent can find them quickly.

- Warnings
  - Always work in a development environment and a feature branch—automated edits can be broad.
  - Accept the “stop running application” popup only if you are ready to restart or rely on hot reload.
  - Agent analysis may take time; avoid making many simultaneous requests that could conflict.
  - Review all generated code changes before committing to source control.

---

By following this manual you can reproduce the demonstrated workflow: supply branding assets and screenshots to a multimodal agent, let it edit the Blazor front-end (app.css and Razor components), and iterate quickly using hot reload and targeted feedback.