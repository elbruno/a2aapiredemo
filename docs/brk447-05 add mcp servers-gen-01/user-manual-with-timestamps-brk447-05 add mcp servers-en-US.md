# Video: [brk447-05 add mcp servers.mkv](./REPLACE_WITH_VIDEO_LINK) — 00:02:42

# Extend GitHub Copilot with MCP Servers — User Manual

This manual shows how to extend GitHub Copilot by registering additional MCP (Managed Connector Provider) servers in your Visual Studio solution. Follow the steps to add servers such as GitHub and Microsoft Docs (Microsoft Learn), authenticate, and verify the new tools in the Copilot assistant chat.

- Total video reference duration: 00:02:37.560
- Relevant UI: Visual Studio, GitHub Copilot (assistant/chat), Solution Explorer, Tools pane

---

## Overview

(00:00:02.760 — 00:00:16.080)

This guide demonstrates how to give GitHub Copilot more capabilities by registering MCP servers. Built-in MCP tools are available out of the box, and you can add additional servers (for example, GitHub and Microsoft Docs) by creating a configuration file named `<name>.mcp.json` in the solution root. After registering a server, Visual Studio will use local credentials to authenticate and then expose many Copilot-integrated tools.

Snapshot: ![Introduction: Goal - extend Copilot with MCP servers](./%5B00%3A00%3A02.760%5D)

---

## Step-by-step instructions

Follow these steps in order. Each step includes a short explanation, the actions to perform, and a snapshot reference.

### 1. Review out-of-the-box MCP tools
(00:00:16.080 — 00:00:36.640)

What you’ll see:
- GitHub Copilot local (built-in)
- .NET aggregate assistance (built-in)
- MCP servers list in Visual Studio

Why: Understand what is already available before adding servers.

Actions:
1. Open Visual Studio.
2. Open the MCP or Copilot tools/configuration view to see the built-in tools.

Tip: This helps you confirm baseline functionality before adding new connectors.

Snapshot: ![Out-of-the-box MCP tools list](./%5B00%3A00%3A16.080%5D)

---

### 2. Decide which MCP servers to add
(00:00:36.680 — 00:00:56.528)

Common choices:
- GitHub (for repositories, issues, PRs, code search)
- Microsoft Docs / Microsoft Learn (for quick documentation access)

Actions:
1. Identify desired servers to add (e.g., GitHub, Microsoft Docs).
2. Consult the server configuration examples in documentation if you need custom parameters.

Snapshot: ![Selecting additional servers (GitHub, Microsoft Docs)](./%5B00%3A00%3A36.680%5D)

Tip: Start with GitHub and Microsoft Docs to quickly expand Copilot's context and toolset.

---

### 3. Create the mcp.json file in the solution root
(00:00:56.528 — 00:01:25.952)

What to do:
1. Open your solution in Visual Studio.
2. In Solution Explorer, right-click the solution root and choose Add → New Item (or Add → File).
3. Name the file using the pattern `<name>.mcp.json`. Example: `myconnectors.mcp.json`.
   - Ensure the file is placed at the solution root (not nested in a project folder).

Why: Visual Studio scans `.mcp.json` files in the solution root to register MCP servers.

Snapshot: ![Solution Explorer: Add new file named <name>.mcp.json](./%5B00%3A00%3A56.528%5D)

Warning: The file must be at the solution root. If placed in a subfolder, Visual Studio may not detect it.

---

### 4. Add the GitHub server configuration and authenticate
(00:01:25.952 — 00:01:55.680)

What to do:
1. Open `<name>.mcp.json` you created.
2. Paste or write the GitHub server configuration JSON block provided in your documentation or examples.
   - Example (template):
     {
       "servers": [
         {
           "id": "github",
           "type": "github",
           "displayName": "GitHub",
           "endpoint": "https://api.github.com"
         }
       ]
     }
3. Save the file.

What happens next:
- Visual Studio will cache the server entry and show its status (e.g., "in cache, not connected").
- When a connection or operation requires authentication, Visual Studio will prompt you to authenticate.
- Use the Visual Studio local credential chooser to authenticate to GitHub (this uses your current Visual Studio account or token).

After authentication:
- Numerous GitHub-related Copilot tools become available (the video reported seeing +90 tools).

Snapshot: ![mcp.json with GitHub config and authentication prompt](./%5B00%3A01%3A25.952%5D)

Tip: If you have multiple accounts, confirm the credential chooser uses the account that has the necessary repository access.

Warning: If authentication fails, the server will remain cached but not connected. Resolve credential issues before proceeding.

---

### 5. Add the Microsoft Docs / Microsoft Learn configuration
(00:01:55.680 — 00:02:40.320)

What to do:
1. In the same `<name>.mcp.json`, add a server block for Microsoft Docs / Microsoft Learn.
   - Example (template):
     {
       "servers": [
         {
           "id": "microsoft-docs",
           "type": "docs",
           "displayName": "Microsoft Docs",
           "endpoint": "https://learn.microsoft.com"
         }
       ]
     }
2. Save the file.

Verify in Copilot:
1. Open GitHub Copilot in assistant (chat) mode.
2. Open the Tools pane in the Copilot chat UI.
3. Confirm both the new Microsoft Docs tool and the GitHub Copilot (or GitHub server tools) appear in the Tools list.

Snapshot: ![Copilot assistant Tools pane showing Microsoft Docs and GitHub tools](./%5B00%3A01%3A55.680%5D)

Tip: Opening Copilot assistant chat and viewing the Tools pane is the quickest way to confirm server registration and tool availability.

---

## Inline Snapshots / Images

Below are inline snapshot placeholders corresponding to key steps and UI states. Replace each placeholder with the extracted frame from the video when embedding images into documentation.

- Snapshot (00:00:02.760): ![Introduction: Goal - extend Copilot with MCP servers](./%5B00%3A00%3A02.760%5D)
- Snapshot (00:00:16.080): ![Out-of-the-box MCP tools list](./%5B00%3A00%3A16.080%5D)
- Snapshot (00:00:36.680): ![Selecting additional servers (GitHub, Microsoft Docs)](./%5B00%3A00%3A36.680%5D)
- Snapshot (00:00:56.528): ![Solution Explorer: Add new file named <name>.mcp.json](./%5B00%3A00%3A56.528%5D)
- Snapshot (00:01:25.952): ![mcp.json with GitHub config and authentication prompt](./%5B00%3A01%3A25.952%5D)
- Snapshot (00:01:55.680): ![Copilot assistant Tools pane showing Microsoft Docs and GitHub tools](./%5B00%3A01%3A55.680%5D)
- Snapshot (00:02:40.320): ![Final verification of tools after adding Microsoft Docs](./%5B00%3A02%3A40.320%5D)

---

## Tips & Warnings (quick reference)

- Tip: Place the `.mcp.json` file at the solution root — Visual Studio expects it there.
- Tip: Use the credential chooser in Visual Studio to authenticate; it will pick up your local Visual Studio account or token.
- Warning: If the server shows as "in cache, not connected," authentication is still required before tools appear.
- Warning: Ensure you use correct endpoints and server types in your MCP JSON blocks to avoid registration errors.

---

## Snapshots

[00:00:02.760]
[00:00:16.080]
[00:00:36.680]
[00:00:56.528]
[00:01:25.952]
[00:01:55.680]
[00:02:40.320]