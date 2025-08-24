# User Manual — Updating a Blazor App UI with GitHub Copilot (Compiler) and Multimodal Inputs

Duration: 00:05:04.920

## Overview
(00:00:00 — 00:05:04)

This manual describes a repeatable workflow for using an agent (GitHub Copilot / Compiler) together with multimodal inputs (style assets and screenshots) to apply a brand style to a Blazor web application. The workflow covers preparing assets, prompting the compiler, letting it analyze the codebase and plan changes, applying CSS edits, handling a running app, inspecting results in the browser, taking screenshots, and iterating until the UI matches the target style.

Main goals:
- Apply SAVA (brand) styles to a Blazor front-end.
- Use images and screenshots as inputs to guide automatic style changes.
- Iterate: analyze → apply edits → review → refine.

---

## Step-by-step Instructions

Follow these steps in order. Timestamps correspond to the demonstrated actions in the source content.

### 1. Introduction — Agents & Multimodal Models (00:00:00 — 00:00:15)
- Understand the approach: the agent can process both code and images to make style/UI improvements.
- Decide the target branding (e.g., SAVA) and gather any reference images or style files you plan to apply.

Tip: Have a clear visual reference image (color palette, typography, example screens) to guide the agent.

---

### 2. Prepare Style Assets and Reference Images (00:00:15 — 00:00:58)
- Open your project repository and locate the current style files and assets:
  - Common files: app.css (or APP.css), site-wide cascade files, component .razor files.
  - Asset locations: repository file explorer (store), images folder, branding folder.
- Copy the SAVA/brand style file(s) and any relevant images (logo, color palette, screenshots).
- Paste these files and images into the GitHub Copilot / Compiler input context so the agent has direct access to them.

How to present assets to the agent:
- Ensure filenames are descriptive (e.g., sava-brand.css, sava-colors.png).
- Attach images as files or paste references into the Compiler prompt area.

Warning: Do not paste sensitive credentials or private secrets into the compiler context.

---

### 3. Prompt the Compiler to Improve the UI — Initial Run (00:00:58 — 00:01:12)
- In the Compiler prompt textbox, request an audit and UI improvement using the provided images and style file.
- Example prompt (use as a template):

```
Please analyze the repository and the attached SAVA style assets. Update the Blazor front-end styles to match the SAVA brand (colors, contrast, and card styles). Focus on app.css and component styles. Report a step-by-step plan before applying changes.
```

- Send the prompt and wait for the Compiler to begin analysis. The process may take some time.

Tip: Ask the agent to produce a brief plan before making edits so you can confirm intent.

---

### 4. Let the Compiler Analyze the Codebase & Plan Changes (00:01:12 — 00:02:09)
- Allow Copilot to scan CSS cascade files, component (.razor) files, and the file list/store view.
- Review the plan it generates — expected files to edit (eg. app.css, Product.razor adjustments), and the change steps.

What the agent commonly inspects:
- Global CSS cascade (app.css)
- Component-level styles and markup (.razor files)
- Visual references from provided images

Tip: If the plan includes risky or large changes, ask the agent to break them into smaller, incremental edits.

---

### 5. Apply CSS Edits and Handle the Running Application (00:02:09 — 00:03:03)
- Accept the changes applied by the Compiler to app.css or other CSS files.
- If the application is running, the IDE or Compiler will likely prompt you to stop the running app before writing files. Choose to stop the app so changes can be saved.
  - Popup action: "Stop running application?" — accept to proceed.
- Restart / run the application to apply the new styles.
- Refresh the browser to load the updated CSS.

Warning: Stopping the running application will interrupt active sessions. Save any work before confirming the stop.

Tip: If possible, keep a version-controlled commit or branch before automated edits so you can revert quickly.

---

### 6. Evaluate Visual Results and Take a Screenshot (00:03:03 — 00:03:25)
- Inspect the updated UI in the browser. Check:
  - Global color changes and contrast
  - Component/card styles
  - Typography and spacing
- If something looks off (colors, contrast, card appearance), take a screenshot of the problematic area.

How to capture useful screenshots:
- Focus on the affected component(s) (product cards, headers, nav).
- Include enough context (surrounding UI) so the agent can interpret layout and color relationships.

Tip: Keep a naming convention for screenshots (e.g., product-card-issue-01.png) and include short notes describing the issue.

---

### 7. Iterative Improvement — Submit Screenshot and Request Fixes (00:03:25 — 00:03:56)
- Paste or attach the screenshot and a concise description of issues back into the Compiler prompt area.
- Ask the agent to improve the UI based on the screenshot and earlier style references. Example prompt:

```
Attached: product-card-issue-01.png. The card contrast and spacing still look off compared to SAVA references. Please adjust the card background, border, padding, and contrast to match SAVA style. Make small, incremental changes and list the edits you will make.
```

- Optionally request that the agent break the work into smaller steps to reduce risk and allow quicker validation.

Tip: Use multiple screenshots if multiple areas need fixes. Smaller change sets make verification faster.

---

### 8. Inspect Specific Components (Product Page) and Verify Updates (00:03:56 — 00:04:22)
- Navigate to the specific component/page (e.g., Product.razor) in the app and review whether the compiler applied targeted changes.
- Refresh the browser to ensure the latest CSS is loaded.
- Confirm whether new or updated CSS rules appear (e.g., a new CSS class or revised app.css entries).

What to check:
- Did the product page reflect the new styles?
- Are new CSS files or rules created and applied?

Tip: If component-level styles aren't updating, verify component scoping (component CSS vs global CSS) and ensure the agent edited the correct file scope.

---

### 9. Final Run, UI Evaluation, and Iterative Refinement (00:04:22 — 00:05:04)
- Run the application again and inspect the product cards, header, and other UI areas.
- Test interactions: add sample products to the cart and verify UI behavior (buttons, confirmation modals, cart counts).
- Identify remaining issues (e.g., card appearance, missized elements), capture screenshots, and repeat the submit-fix cycle with Copilot.
- Request additional changes based on screenshots and the SAVA style references until you reach the desired look.

Checklist during final reviews:
- Contrast and accessibility (text legibility)
- Button and control styling and spacing
- Card layout, shadows, borders, padding
- Interactive states (hover, active)

Tip: Test on multiple browser widths (responsive layouts) to catch layout regressions.

---

## Tips and Warnings

- Tip: Ask the Compiler for a short plan before it applies changes — this improves predictability.
- Tip: Request incremental changes rather than a single large edit when you are uncertain.
- Tip: Keep copies or a branch/commit point before letting the agent perform edits so you can revert easily.
- Warning: When the application is running, automated edits may require stopping it; expect brief downtime.
- Warning: Do not include secrets or private keys in prompts or attached files.

---

This manual provides the practical steps demonstrated in the video to apply brand styling to a Blazor app using an agent that accepts multimodal inputs. Follow the iterative workflow — prepare assets, prompt, review plan, accept edits, verify in the browser, capture screenshots, and repeat — until the UI matches your brand references.