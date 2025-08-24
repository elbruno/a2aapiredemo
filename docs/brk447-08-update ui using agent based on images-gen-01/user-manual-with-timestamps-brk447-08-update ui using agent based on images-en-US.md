# Video: [brk447-08-update ui using agent based on images.mp4](./REPLACE_WITH_VIDEO_LINK) — 00:05:05

# UI Branding Update Manual — Using Multimodal Copilot/Compiler with Blazor

This manual walks you through the process demonstrated in the video to apply branding and style updates to a Blazor web app using a multimodal agent (GitHub Copilot / Compiler). It covers preparing assets, feeding visual references to the agent, allowing automated edits, verifying changes with hot reload, and iterating with screenshots.

- Total demo duration: 00:05:04.840
- Primary goal: Update website style to SAVA branding using a multimodal agent

---

## Overview

(00:00:00.080 — 00:00:08.760)

This workflow uses a multimodal agent (Copilot / Compiler) that can accept images and prompts, scan the repository, and propose or apply front-end changes automatically. The agent will:

- Read repository assets and front-end files (Blazor Razor components, CSS).
- Apply edits (notably to app.css and components) to match SAVA branding.
- Accept iterative visual feedback (screenshots) to refine colors and contrast.
- Support hot reload for quick verification.

Use this manual to reproduce the same process reliably and safely.

---

## Step-by-step Instructions

Follow these steps in order. Where timestamps are relevant, they are provided for reference to the demo.

### 1. Set goal and enable the multimodal agent (Intro)
- Timestamp reference: 00:00:00.080
- Action:
  1. Open your development workspace and enable the multimodal agent (Copilot / Compiler) feature.
  2. Define the high-level goal for the agent: update the site style to SAVA branding.
- Tip: Be explicit about the target branding (colors, imagery, tone) in your prompt.

![Intro / agent enabled](./path/to/placeholder)  
Caption: Agent enabled and goal set (00:00:00.080)

---

### 2. Locate and copy branding/style assets
- Timestamp reference: 00:00:09.080 — 00:00:35.200
- Action:
  1. Open the repository file list and locate branding assets: reference images, color files, and any existing style file.
  2. Identify a suitable style file to copy (e.g., a branding stylesheet or theme file).
  3. Copy the chosen file into your project folder or workspace.
  4. Return to the Copilot/Compiler input area in your IDE to prepare the asset for upload.
- UI elements to look for: repository file list, branding images, GitHub Copilot input area.
- Tip: Keep filenames clear (e.g., sava-branding.css or sava-style.png) so the agent can reference them.

![Locate branding assets](./path/to/placeholder)  
Caption: Repository assets and branding images located (00:00:09.080)

---

### 3. Attach visual references and prompt the compiler
- Timestamp reference: 00:00:35.200 — 00:01:04.200
- Action:
  1. Open the reference images (two or more images) that exemplify the SAVA look.
  2. Paste/upload those images into the Copilot/Compiler context (image paste/upload area).
  3. In the prompt input, clearly request the UI improvements you want based on those images. Example prompt:
     - "Use these two reference images and the branding file I pasted. Update the site CSS and component styles to match SAVA branding—adjust primary/secondary colors, button styles, headings, and card contrast. Focus on front-end Blazor components and app.css."
  4. Send the prompt.
- UI elements: reference images, Copilot prompt input, image paste/upload area.
- Tip: Include what not to change (e.g., avoid changing core layout or server-side logic).

![Attach reference images](./path/to/placeholder)  
Caption: Two reference images attached to the compiler context (00:00:35.200)

---

### 4. Let Copilot analyze the project and plan edits
- Timestamp reference: 00:01:09.380 — 00:02:09.599
- Action:
  1. Allow Copilot to scan the repository. It will look for front-end files such as:
     - Blazor Razor components (.razor)
     - CSS files (app.css)
     - Component tracer / store files
  2. Review Copilot’s proposed plan (if presented). Typical changes include edits to app.css and updating component styles.
  3. If Copilot attempts to apply changes while the app is running, you may be prompted to stop the running application. Respond to that popup:
     - If asked, choose to stop the running application so file edits can be applied.
  4. Let Copilot apply the edits (it will often modify app.css first).
- UI elements: Blazor front-end files, app.css, popup "stop running application", Copilot activity view.
- Warning: Stopping the app will terminate the current run. Save any unsaved work before confirming.

![Copilot scanning project](./path/to/placeholder)  
Caption: Copilot scans front-end files and starts editing (00:01:09.380)

---

### 5. Restart app and preview applied styles
- Timestamp reference: 00:02:09.720 — 00:03:16.391
- Action:
  1. Restart or run the application in your browser (use your usual run command / browser preview).
  2. Refresh the browser page to load the updated styles.
  3. Inspect the UI for initial results: primary color changes, button styles, and overall contrast.
- UI elements: web browser preview, refresh button.
- Tip: Use a private/incognito window if caching causes the old CSS to persist.
- Warning: Initial automated changes can sometimes be too dark or have poor contrast—do not assume final perfection on first pass.

![Preview updated styles](./path/to/placeholder)  
Caption: Browser preview shows new styling after initial edits (00:02:09.720)

---

### 6. Capture a screenshot of issues and feed back to the agent
- Timestamp reference: 00:02:09.720 — 00:03:16.391
- Action:
  1. If you notice visual issues (e.g., colors too dark, poor contrast), use your OS browser screenshot tool to capture the problematic area.
  2. Paste or upload the screenshot back into the Copilot/Compiler input context.
  3. Add a concise description of the issues and a clear request for adjustments. Example:
     - "Screenshot shows product cards with too-dark backgrounds and low contrast on text. Please lighten card backgrounds, increase text contrast, and adjust button colors to match SAVA branding while keeping accessibility in mind."
  4. Send the prompt and wait for the agent to analyze.
- UI elements: screenshot tool, Copilot input (image + description).
- Tip: Highlight or annotate regions in the screenshot if your tool supports it.
- Warning: Analysis can take a few minutes—be patient.

![Screenshot and feedback](./path/to/placeholder)  
Caption: Screenshot pasted with description asking for further improvements (00:02:40.000)

---

### 7. Use iterative, small edits and Blazor hot reload
- Timestamp reference: 00:03:16.391 — 00:04:33.417
- Action:
  1. When the agent applies updates, prefer incremental changes (smaller steps) so you can quickly verify.
  2. Use Blazor hot reload to see CSS and Razor component edits applied without a full rebuild when possible.
  3. After each agent change, refresh the browser or rely on hot reload to inspect the specific pages affected (e.g., Product pages).
- UI elements: Copilot progress/analysis, Blazor hot reload, Razor component pages (product.razor).
- Tip: If an automated change breaks layout, revert that file and ask Copilot to make a narrower change.

---

### 8. Verify UI changes and test functionality (product cards, cart)
- Timestamp reference: 00:04:33.417 — 00:05:04.920
- Action:
  1. Navigate to the product page (product.razor) and check product cards for correct colors, contrast, and spacing.
  2. Add items to the cart and inspect the cart UI to ensure buttons, totals, and text are readable and styled.
  3. If remaining issues exist, capture screenshots and run another targeted Copilot prompt for refinement.
- UI elements: product page, product cards, cart, browser run button/command.
- Tip: Test on multiple screen sizes and browser zoom levels to validate responsive styles.

![Verify product cards and cart](./path/to/placeholder)  
Caption: Product cards and cart checked after iterative updates (00:04:33.417)

---

## Tips and Warnings (general)

- Tip: Be explicit in prompts—include color hex codes, font names, and elements to change for predictable results.
- Tip: Provide multiple reference images showing the desired look (buttons, cards, navbars).
- Warning: Allowing automated tools to edit while the app is running may require stopping the app; save your work.
- Warning: Automated changes may affect accessibility; always verify contrast ratios and readable font sizes after edits.

---

## Snapshots

Include the images captured at the following key moments to document each stage visually.

- [00:00:00.080] — Agent enabled and goal set (intro)
- [00:00:09.080] — Repository branding assets located
- [00:00:35.200] — Reference images attached to Copilot/Compiler
- [00:01:09.380] — Copilot scanning project / planning edits
- [00:02:09.720] — Browser preview showing initial updated styles
- [00:02:40.000] — Screenshot pasted into Copilot with issue description
- [00:03:16.391] — Copilot analysis / hot reload in progress
- [00:04:33.417] — Final verification: product cards and cart UI

(Note: Replace placeholder images in the manual with the actual screenshots captured at the timestamps above.)