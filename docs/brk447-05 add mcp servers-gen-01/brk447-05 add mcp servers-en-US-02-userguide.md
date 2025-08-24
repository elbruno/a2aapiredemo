# Video: [brk447-05 add mcp servers.mkv](./REPLACE_WITH_VIDEO_LINK) — 00:02:42

# Extending GitHub Copilot with MCP Servers — User Manual

This manual shows how to register additional MCP (Managed Connector Provider) servers so GitHub Copilot in Visual Studio can use external sources (for example: GitHub and Microsoft Learn / Microsoft Docs). Follow the steps below to create an MCP configuration file in your solution root, add server entries, authenticate, and verify the new tools appear in the Copilot assistant.

Duration referenced from source video: 00:00:02.760 — 00:02:37.560

---

## Overview
(00:00:02.760 — 00:00:16.080)

- Goal: Give GitHub Copilot more capabilities by registering MCP servers.
- Result: After registering servers and authenticating, Copilot gains additional tools (e.g., GitHub-related tools and documentation lookup via Microsoft Docs).

---

## Step-by-step instructions

Note: These steps assume you are using Visual Studio with GitHub Copilot already installed and running.

### 1. Open your solution in Visual Studio
(00:00:56.528 — 00:01:00 approx.)
1. Launch Visual Studio and open the solution in which you want to register MCP servers.
2. Open Solution Explorer and confirm the solution root is visible.

Tip: Place the MCP configuration in the solution root so it applies to the whole solution.

---

### 2. Create the MCP configuration file in the solution root
(00:00:56.528 — 00:01:25.952)

1. In Solution Explorer, right-click the solution root (or the project root if no solution-level node) and choose Add > New Item (or Add > New File).
2. Name the new file using the pattern: <name>.mcp.json — for example: my-solution.mcp.json
   - The filename can be any name but must end with `.mcp.json` and be located at solution root.
3. Save the file.

Warning: The file must be saved in the solution root. If the file is elsewhere, Copilot may not detect the registered servers.

---

### 3. Add the GitHub MCP server entry
(00:01:25.952 — 00:01:55.680)

1. Open the newly created `<name>.mcp.json` file in the editor.
2. Add a JSON object describing the GitHub server. Use this minimal example as a starting point, replacing placeholders with your settings where necessary:

```json
{
  "servers": [
    {
      "id": "github",
      "displayName": "GitHub",
      "type": "github",
      "url": "https://api.github.com"
    }
  ]
}
```

3. Save the file. Saving registers the server with Visual Studio/Copilot.

What to expect:
- Visual Studio may show the server status as "in cache, not connected" initially.
- An authentication prompt may appear.

Tip: If you have an account already signed into Visual Studio (e.g., GitHub or Microsoft account), Visual Studio will present it in the credential chooser.

---

### 4. Authenticate to GitHub via Visual Studio credentials
(00:01:25.952 — 00:01:55.680)

1. When prompted, choose the account to authenticate with (Visual Studio’s credential chooser).
2. Complete the sign-in flow for GitHub as requested by Visual Studio.
3. After successful authentication, Visual Studio connects to the GitHub server entry.

What to observe:
- The server status should change from cached/not connected to connected.
- Copilot will enumerate GitHub-related tools — the presenter saw +90 tools become available.

Warning: If you deny or cancel authentication, Copilot cannot access GitHub resources and related tools will remain unavailable.

---

### 5. Add Microsoft Learn / Microsoft Docs server entry
(00:01:55.680 — 00:02:10 approx.)

1. Edit the same `<name>.mcp.json` and add a Microsoft Docs / Learn server entry. Example:

```json
{
  "servers": [
    {
      "id": "github",
      "displayName": "GitHub",
      "type": "github",
      "url": "https://api.github.com"
    },
    {
      "id": "microsoft-docs",
      "displayName": "Microsoft Learn / Docs",
      "type": "docs",
      "url": "https://learn.microsoft.com"
    }
  ]
}
```

2. Save the file to register the new server.

Tip: Replace the "type" and "url" values to match any specific MCP server schema required by the server provider. If unsure, consult the provider’s MCP configuration examples.

---

### 6. Verify new tools appear in GitHub Copilot assistant (chat mode)
(00:01:55.680 — 00:02:40.320)

1. Open GitHub Copilot in Assistant / Chat mode (the Copilot pane in Visual Studio).
2. In the Copilot chat window, open the Tools panel (often a sidebar or tab labeled “Tools”).
3. Look for entries such as:
   - GitHub Copilot local (built-in),
   - GitHub (the registered server tools),
   - Microsoft Docs / Microsoft Learn (the registered docs server).

What to expect:
- The Tools pane should now list the newly registered tools.
- You can invoke these tools from Copilot chat to query documentation, search repositories, or use server-provided capabilities.

Tip: If the new tools don't appear immediately, try closing and reopening the Copilot assistant, or wait a moment for Visual Studio to finish the server handshake.

---

## Helpful Tips and Warnings

- Tip: Use meaningful IDs and display names in your `.mcp.json` for easier identification in the Copilot Tools pane.
- Tip: If you manage multiple solutions, place a separate `.mcp.json` in each solution root to have solution-specific servers.
- Warning: Ensure the JSON is valid. A syntax error in `.mcp.json` can prevent the MCP servers from registering.
- Warning: Authentication uses Visual Studio credentials. If Visual Studio is signed into the wrong account, sign out or add the correct account before authenticating.

---

Timestamps reference:
- Introduction & Goal: 00:00:02.760 — 00:00:16.080
- Out-of-the-box MCP Tools Overview: 00:00:16.080 — 00:00:36.640
- Selecting Additional Servers: 00:00:36.680 — 00:00:56.528
- Create mcp.json: 00:00:56.528 — 00:01:25.952
- Add GitHub Server & Authentication: 00:01:25.952 — 00:01:55.680
- Add Microsoft Docs & Verify: 00:01:55.680 — 00:02:40.320

This manual provides the actionable steps demonstrated in the video to extend GitHub Copilot by registering MCP servers and verifying available tools in the Copilot assistant.