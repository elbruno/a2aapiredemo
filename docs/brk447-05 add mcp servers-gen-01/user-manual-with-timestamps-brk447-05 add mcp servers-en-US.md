# Extend GitHub Copilot with MCP Servers — User Manual

This manual shows how to add MCP servers to GitHub Copilot (Assistant mode) inside Visual Studio so Copilot gains additional tools (for example, GitHub and Microsoft Docs). Follow the steps below to create an MCP configuration file, add server entries, authenticate, and verify the new tools in the Copilot UI.

## Overview
[00:00:02.760 - 00:00:16.080]

MCP (Managed Copilot Provider) servers are configuration entries that let GitHub Copilot access external tool sets and content sources. By adding MCP servers to your project (via a <name>.mcp.json file placed in the solution root), Copilot can expose additional tools — in the demonstrated example, GitHub tooling (+90 tools) and Microsoft Docs access — directly in the Copilot Chat (Assistant) Tools panel.

Tip: This manual follows the exact workflow shown in the demonstration video. Timestamps mark where each action appears.

## Step-by-step instructions

### 1. Confirm default MCP tools
[00:00:16.080 - 00:00:26.800]

1. Open Visual Studio and open a solution.
2. Open GitHub Copilot in Assistant mode.
3. Open the Copilot Tools panel and observe the default, out-of-the-box MCP tools.

![Default MCP tools view][00:00:16.080]

Tip: The default tools typically include local Copilot helpers and any .NET-specific assistance shipped with Visual Studio.

---

### 2. Open Visual Studio MCP servers list and plan additions
[00:00:27.400 - 00:00:49.258]

1. In Visual Studio, open the list of MCP servers (look for an MCP/servers view or configuration area in the solution explorer or relevant extension settings).
2. Review the existing servers.
3. Decide which external servers you want to add. In the example, the presenter chooses:
   - GitHub (to surface many GitHub-related tool integrations)
   - Microsoft Learn / Docs (for quick access to documentation)

![MCP servers list planning][00:00:27.400]

Tip: Choose servers that will provide tools you will actually use to avoid cluttering the Tools panel.

---

### 3. Create an MCP configuration file in the solution root
[00:00:49.258 - 00:01:22.090]

1. In Solution Explorer, right-click the solution root (or use the Add New File command).
2. Add a new file and name it using the extension `.mcp.json`. Example: `project.mcp.json` or `<name>.mcp.json`.
3. Save the new file in the solution root.

![Create new .mcp.json file in solution explorer][00:00:49.258]

Tip: Use a meaningful filename such as `workspace.mcp.json` so it’s easy to find later.

Warning: The MCP file must be valid JSON. Invalid JSON will prevent Visual Studio from reading the server entries.

---

### 4. Add GitHub MCP server entry
[00:01:22.090 - 00:01:36.013]

1. Open the newly created `<name>.mcp.json` file in the editor.
2. Paste the GitHub server configuration JSON snippet into the file. (In the video, the presenter copies the GitHub entry directly into this file.)
3. Save the file.

Example placeholder structure (replace with the exact snippet you have or obtain the official configuration):
```json
{
  "servers": [
    {
      "label": "GitHub",
      "id": "github",
      "endpoint": "https://github.com"
      // ...additional configuration fields as provided
    }
  ]
}
```

![Paste GitHub MCP server JSON][00:01:22.090]

Tip: If you do not have the snippet, check your organization's documentation or the extension/provider docs for the exact JSON format.

---

### 5. Authenticate the added server (Visual Studio credentials)
[00:01:36.013 - 00:01:55.680]

1. After saving, open the MCP servers list or the Copilot Tools panel. The new GitHub server will appear in the cache/state as "not connected" or "requires authentication."
2. Trigger the authentication flow (this may happen automatically or via a connect/authenticate action).
3. When prompted, select the local Visual Studio authenticated credential (or another credential you prefer) to authenticate with GitHub.
4. Complete any browser-based OAuth or credential confirmation steps.

Result: Once authenticated, the GitHub MCP server becomes available and exposes many additional tools (the video shows ~+90 GitHub tools).

![Authenticate GitHub MCP server with Visual Studio credentials][00:01:36.013]

Warning: Use only credentials you trust. Authenticating with personal accounts may expose personal permissions to the tools; use organization/service accounts as appropriate.

---

### 6. Add Microsoft Docs MCP server entry
[00:01:56.000 - 00:02:12.240]

1. In the same `<name>.mcp.json` file, add another server entry for Microsoft Docs (labelled “Microsoft Docs” or “docs”) by copying the Docs configuration snippet into the JSON file.
2. Save the file.

Example placeholder structure:
```json
{
  "servers": [
    {
      "label": "GitHub",
      "id": "github",
      "endpoint": "https://github.com"
    },
    {
      "label": "Microsoft Docs",
      "id": "docs",
      "endpoint": "https://learn.microsoft.com"
    }
  ]
}
```

![Add Microsoft Docs entry to .mcp.json][00:01:56.000]

Tip: The Microsoft Docs configuration is usually simple—verify label and endpoint values match the recommended snippet.

---

### 7. Verify the new tools in Copilot Chat (Assistant) UI
[00:02:24.000 - 00:02:40.320]

1. Open GitHub Copilot Chat in Assistant mode.
2. Open the Tools panel.
3. Confirm that the newly added Microsoft Docs tool appears in the list.
4. Confirm GitHub Copilot toolset (the GitHub server tools) appear as well.

![Verify Microsoft Docs and GitHub tools in Copilot Tools panel][00:02:24.000]

Tip: If a server does not appear immediately, try:
- Saving the .mcp.json file again
- Closing and reopening the Copilot Tools panel
- Restarting Visual Studio

Warning: Some tools may require further permissions or separate authentications; check the prompt messages carefully.

---

## Snapshots / Inline Image Placeholders

The manual includes inline snapshot placeholders above. Use these timestamps to capture the corresponding frames from the source video and replace the placeholders with images in your documentation. Each inline placeholder is labeled with a timestamp for easy mapping.

## Snapshots

[00:00:02.760]  
[00:00:16.080]  
[00:00:27.400]  
[00:00:49.258]  
[00:01:22.090]  
[00:01:36.013]  
[00:01:56.000]  
[00:02:24.000]