# Applying Brand Styles to a Blazor App Using GitHub Copilot / Compiler (Multimodal Agent)

Overview
--------
This manual guides you through the workflow shown in the video to update a Blazor application's UI to match a brand (SAVA) using a multimodal agent (GitHub Copilot / Compiler). The process covers preparing style assets and reference images, prompting the compiler with multimodal inputs, letting the agent analyze the codebase, applying CSS edits, restarting the app, and iterating using screenshots to refine the UI.

Key outcomes:
- Use images and style files as references for automated UI improvements
- Let Copilot scan the project and produce a plan
- Apply CSS changes safely and verify them in the browser
- Iterate with screenshots to refine visuals and component styling

Relevant timestamps for reference: see inline timestamps in steps and final Snapshots list.

Step-by-step Instructions
-------------------------

1. Introduction — Understand the goal (00:00:00.080 → 00:00:15.560)
   - Goal: update the Blazor app UI to match the SAVA brand and leverage Copilot's multimodal capabilities.
   - Tip: Have a clear visual target (brand style guide or screenshots) ready before you start.

   ![Intro snapshot — agent + brand intent (00:00:00.080)](./snapshot-00-00-00.080.png)

2. Prepare style assets and reference images (00:00:15.560 → 00:00:58.400)
   - Open your repository file explorer (store view) and locate:
     - Current CSS cascade file(s) (commonly app.css, site.css, or styles/*.css)
     - Component files (.razor) for pages you expect to change (e.g., Product.razor)
     - Brand assets: SAVA style file(s) and reference images (logos, color swatches, card mockups)
   - Copy the SAVA/brand style file and any images to a location where you can paste them into the Copilot/Compiler context.
   - In the Compiler interface, paste the brand style file and upload or paste reference images into the prompt/input area.

   Tips:
   - Use high-resolution images for better analysis.
   - If the repo has many CSS files, note the primary cascade file (app.css) to prioritize.

   ![Preparing assets in repo and Copilot input (00:00:15.560)](./snapshot-00-00-15.560.png)

3. Prompt the Compiler to analyze and improve the UI (initial run) (00:00:58.400 → 00:01:12.706)
   - In the prompt textbox, give a concise instruction such as:
     - "Analyze these reference images and the repository files. Update the app's styles and components so the UI follows the SAVA brand colors, typography, and card styles. Make minimal, safe changes and list the files you'll modify."
   - Send the prompt to start the analysis.
   - Wait while the Compiler begins scanning and planning (this can take some time).

   Tip:
   - Ask for a step-by-step plan first if you prefer manual review before modifications.

   ![Prompting Compiler with images & directions (00:00:58.400)](./snapshot-00-00-58.400.png)

4. Let the Compiler analyze the codebase and prepare a plan (00:01:12.706 → 00:02:09.720)
   - Copilot will scan:
     - The Blazor front end structure
     - CSS cascade files (e.g., app.css)
     - Component .razor files (e.g., Product.razor)
     - The repository "store" / file list
   - Review the plan Copilot provides. Typical plan items:
     - Edit app.css or create a new CSS file for SAVA variables
     - Update component classes to use new styles
     - Apply color and typographic changes to product cards

   Tip:
   - If the plan makes large or risky changes, request the agent to break work into smaller commits.

5. Apply CSS edits and handle the running application (00:02:09.720 → 00:03:03.040)
   - When Copilot applies edits, you will be shown the modified files (e.g., APP.css).
   - If the app is running, a popup will prompt you to stop the running application before changes can be fully applied.
     - Accept stopping the running app to let changes be written and compiled.
   - Restart/run the application after the agent finishes edits.
   - Refresh the browser to view updated styles.

   Warnings:
   - **Save or commit current work** before accepting automated edits to avoid losing changes.
   - **Stopping a running app** will interrupt user sessions. Do this in a dev environment.

   ![Popup asking to stop the app and restart to apply styles (00:02:09.720)](./snapshot-00-02-09.720.png)

6. Evaluate visual results and take a screenshot (00:03:03.040 → 00:03:25.830)
   - Inspect the updated UI in the browser.
   - Look for issues:
     - Incorrect color values or contrast
     - Typography not applied
     - Product cards not matching the reference
   - Take a screenshot of the areas that need fixing. Include the surrounding layout so the agent has context.

   Tip:
   - Prefer full-window screenshots or targeted region captures of components (e.g., product cards) that need improvement.

   ![Inspecting results in the browser and taking a screenshot (00:03:03.040)](./snapshot-00-03-03.040.png)

7. Iterative improvement — resubmit screenshots and request fixes (00:03:25.830 → 00:03:56.709)
   - Paste the screenshot(s) into the Copilot/Compiler prompt area.
   - Provide a concise description of the issues and request specific fixes. Examples:
     - "Contrast is too low on product cards; align colors to SAVA primary/secondary swatches attached."
     - "Make product card backgrounds white with a subtle shadow and round corners to match SAVA mockup."
   - Optionally ask the agent to make smaller incremental changes (easier to review and revert).

   Tip:
   - When results are noisy, ask for one change at a time (e.g., colors first, then spacing, then card layout).

   ![Pasting screenshot & requesting further changes (00:03:25.830)](./snapshot-00-03-25.830.png)

8. Inspect specific components and verify updates (Product.razor example) (00:03:56.709 → 00:04:22.274)
   - Navigate to the component page in your app (Product.razor).
   - Refresh the browser to ensure the component picks up the new CSS.
   - Check the repository for a new or updated CSS snippet that the agent added to improve styling (e.g., additional selectors or a new CSS file).
   - Confirm whether the Product.razor markup requires class updates or if CSS overrides suffice.

   Tip:
   - If a component still looks unchanged, inspect the element with browser dev tools to see which CSS rules are applied and whether selectors are being overridden.

   ![Verifying Product.razor and new CSS edits (00:03:56.709)](./snapshot-00-03-56.709.png)

9. Final run, UI evaluation, and further iterative refinement (00:04:22.274 → 00:05:04.920)
   - Run the application and inspect interactive elements:
     - Product cards: appearance, spacing, shadows, borders
     - Cart interactions: adding a product to cart and UI response
   - Test a sample interaction (add a product to cart) to validate states (hover, active, added).
   - Note remaining issues (e.g., card appearance not matching reference) and submit fresh screenshots + instructions for additional agent iterations.

   Tips:
   - Use the app's normal workflows to catch state-related style issues (disabled, hover, focus).
   - Continue the cycle: prompt → analyze → apply → verify → screenshot until the UI matches the reference.

   ![Running the app, testing add-to-cart, and evaluating final UI (00:04:22.274)](./snapshot-00-04-22.274.png)

Helpful Tips & Best Practices
-----------------------------
- Start with a clear reference: color hex codes, typography specs, and sample mockups improve agent accuracy.
- Ask for a safety-first plan: request the agent to list files to modify before applying edits.
- Keep changes small and commit often to make rollbacks easy.
- Use targeted screenshots with annotations (if supported) to point out exact problems.
- If an automated change breaks something, revert via version control (git) and ask the agent for an incremental fix.
- Use browser dev tools to confirm which CSS selectors need to be changed; include these findings in your next prompt.

Warnings
--------
- Always work in a development environment, not production, when letting agents modify code.
- Stop the running application only when safe to do so — unsaved data and sessions may be interrupted.
- Automated edits may not capture every edge case; manual review is required before release.

Inline Snapshot Placeholders
----------------------------
Place the actual extracted frames at these points in your own documentation or editing tool where the placeholders are referenced above. Filenames or references shown next to each placeholder can be replaced with real images extracted from the video.

## Snapshots
[00:00:00.080]  
[00:00:15.560]  
[00:00:58.400]  
[00:01:12.706]  
[00:02:09.720]  
[00:03:03.040]  
[00:03:25.830]  
[00:03:56.709]  
[00:04:22.274]  
[00:05:04.920]