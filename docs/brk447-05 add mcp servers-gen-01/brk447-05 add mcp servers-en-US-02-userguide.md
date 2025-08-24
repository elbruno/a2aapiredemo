# Extend GitHub Copilot with MCP Servers — User Manual

This manual shows how to add MCP (Modular Copilot Provider) servers to GitHub Copilot in Visual Studio so Copilot can surface extra tools (for example, GitHub and Microsoft Docs). Follow the steps below to create an MCP configuration file in your solution, add server entries, authenticate, and verify the tools inside Copilot Chat (Assistant mode).

- Video reference duration: 00:00:02.760 – 00:02:40.320

---

## Overview
(00:00:02.760 – 00:00:16.080)

MCP servers extend GitHub Copilot with additional tools and data sources. Out of the box Copilot may include a small set of MCP tools; adding MCP server entries to a configuration file in your solution lets you load more servers (for example, GitHub and Microsoft Docs) and exposes many additional Copilot tools.

Key points:
- MCP servers are defined via a JSON configuration file placed in your solution root.
- After adding a server entry you may need to authenticate; Visual Studio can provide local credentials to do this.
- Once connected, Copilot’s Assistant tools panel will list added tools.

---

## Step-by-step instructions

Follow these steps to add MCP servers and verify them in Copilot Chat.

### 1) Inspect default MCP tools
(00:00:16.080 – 00:00:26.800)

1. Open Visual Studio.
2. Open GitHub Copilot (Assistant mode).
3. Open the Copilot Tools/Plugins panel to view the default out-of-the-box tools (the demo shows two default tools).

Tip: This gives a baseline of what’s available before you add new servers.

---

### 2) Open Visual Studio MCP servers list and plan additions
(00:00:27.400 – 00:00:49.258)

1. In Visual Studio, open the MCP servers list (this is in the Copilot / extension settings area or a dedicated MCP servers view).
2. Review existing servers. Identify which additional servers you want to add (the demo targets:
   - GitHub
   - Microsoft Learn / Microsoft Docs)

Note the desired servers so you can add their JSON configuration entries in the next steps.

---

### 3) Create the MCP configuration file in your solution
(00:00:49.258 – 00:01:22.090)

1. In Solution Explorer, right-click the solution root (top-level node).
2. Choose Add → New Item (or Add New File).
3. Create a new file named using the pattern: <name>.mcp.json
   - Example: my-servers.mcp.json or copilot-servers.mcp.json
4. Save the file in the solution root.

Warning: The file must be valid JSON and located in the solution root so Visual Studio/Copilot can discover it.

---

### 4) Add a GitHub MCP server entry
(00:01:22.090 – 00:01:36.013)

1. Open the newly created <name>.mcp.json in the editor.
2. Paste or type the GitHub server JSON snippet into the file.

Example minimal structure (use this as a template — replace or expand fields with the exact configuration snippet you have from your source):

```json
{
  "servers": [
    {
      "id": "github",
      "label": "GitHub",
      "endpoint": "https://api.github.com",
      "type": "github"
    }
  ]
}
```

3. Save the file.

Tip: If you have an official snippet or configuration from your organization or the video, paste it here instead of the template.

---

### 5) Authenticate the added server and connect
(00:01:36.013 – 00:01:55.680)

1. After adding the GitHub entry, return to the MCP servers list or the Copilot servers view.
2. The newly added GitHub server will likely appear in the server cache as “not connected” or “requires authentication.”
3. Trigger connection/authentication:
   - Click the server entry or a “Connect”/“Authenticate” action shown in Visual Studio.
   - When prompted, choose the local Visual Studio credential (the account you use to sign into Visual Studio) to authenticate to GitHub.
4. Confirm the authentication completes. Once authenticated, GitHub will expose many additional tools (the demo shows +90 tools available).

Tip: Use the Visual Studio account option to avoid entering credentials repeatedly. If your organization uses SSO, follow your normal sign-on flow.

Warning: If authentication fails, double-check that your Visual Studio account has permission to access the GitHub organization or resources being requested.

---

### 6) Add Microsoft Docs (Microsoft Learn) MCP server entry
(00:01:56.000 – 00:02:12.240)

1. Return to your <name>.mcp.json file.
2. Add a Microsoft Docs entry to the "servers" array.

Example minimal entry to append (adjust as needed):

```json
{
  "id": "microsoft-docs",
  "label": "Microsoft Docs",
  "endpoint": "https://learn.microsoft.com",
  "type": "docs"
}
```

3. Save the file.
4. If needed, refresh the MCP servers view in Visual Studio so the new entry is recognized.

Tip: The Microsoft Docs config is typically simple — just ensure it uses the correct endpoint and an appropriate "type" or label.

---

### 7) Verify tools in Copilot Chat (Assistant mode)
(00:02:24.000 – 00:02:40.320)

1. Open GitHub Copilot Chat in Assistant mode in Visual Studio.
2. Open the Tools panel (Tools list / Plugins section).
3. Confirm that:
   - The Microsoft Docs tool is listed.
   - The GitHub Copilot toolset (and newly exposed GitHub tools) are present.

If the tools are shown, the configuration and authentication were successful and Copilot can now use those MCP servers to surface additional assistance.

---

## Tips & Warnings (summary)
- Tip: Name the file clearly (e.g., copilot-servers.mcp.json) and keep it in the solution root so it is discovered automatically.
- Tip: If you have official JSON snippets for GitHub or Microsoft Docs from a trusted source, prefer pasting those exact snippets into the file.
- Warning: Ensure the JSON is valid. A stray comma or missing bracket will prevent Visual Studio/Copilot from parsing the file.
- Warning: Authentication is required for many servers (GitHub especially). Use your Visual Studio authenticated account to connect if available.
- Troubleshooting hint: If tools do not appear after adding entries and authenticating, restart Visual Studio or reload the solution to force a refresh of discovered servers.

---

Timestamps referenced in this manual correspond to the demo sections in the analyzed video (total duration 00:02:40.320). Follow the steps in order to reproduce the result shown in the video: adding GitHub and Microsoft Docs MCP servers to extend GitHub Copilot’s toolset.