# Environment Setup Guide

This guide walks you through setting up your development environment for working with the Semantic Kernel to Agent Framework migration materials.

## Prerequisites

### Required Software

1. **.NET 9 SDK** (9.0.x or later)
   - Download: [https://dotnet.microsoft.com/download/dotnet/9.0](https://dotnet.microsoft.com/download/dotnet/9.0)
   - Verify installation: `dotnet --version`

2. **IDE** (Choose one)
   - **Visual Studio 2022** (17.12 or later) - Recommended for Windows
   - **Visual Studio Code** with C# Dev Kit - Cross-platform
   - **JetBrains Rider** (2024.x or later) - Cross-platform

3. **Git** (for cloning repositories)
   - Download: [https://git-scm.com/downloads](https://git-scm.com/downloads)

### AI Service Access

You need access to one of the following:

- **Azure OpenAI Service** (recommended for production)
  - Deployment name for chat model (e.g., "gpt-4o")
  - API key and endpoint
- **OpenAI API**
  - API key from [https://platform.openai.com/](https://platform.openai.com/)
  - Model name (e.g., "gpt-4o")

---

## Step 1: Install .NET 9 SDK

### Windows

1. Download the .NET 9 SDK installer from Microsoft
2. Run the installer
3. Verify installation:
   ```powershell
   dotnet --version
   # Should output 9.0.x
   ```

### macOS

```bash
# Using Homebrew
brew install dotnet@9

# Verify
dotnet --version
```

### Linux (Ubuntu/Debian)

```bash
# Add Microsoft package repository
wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb

# Install .NET SDK
sudo apt-get update
sudo apt-get install -y dotnet-sdk-9.0

# Verify
dotnet --version
```

---

## Step 2: Clone the Repository

```bash
# Clone the repository
git clone https://github.com/elbruno/a2aapiredemo.git

# Navigate to the repository
cd a2aapiredemo

# Verify global.json (should pin to .NET 9)
cat global.json
```

Expected output:
```json
{
  "sdk": {
    "version": "9.0.x",
    "rollForward": "latestMinor"
  }
}
```

---

## Step 3: Configure User Secrets

This repository uses .NET User Secrets for secure configuration. **NO .env files are used.**

### Initialize User Secrets (for each project)

Navigate to a project directory and initialize secrets:

```bash
# Example: BasicAgent_AF sample
cd src/samples/BasicAgent_AF
dotnet user-secrets init
```

### Add Your API Keys

#### For OpenAI:

```bash
dotnet user-secrets set "OpenAI:ApiKey" "sk-your-api-key-here"
dotnet user-secrets set "OpenAI:ChatDeploymentName" "gpt-4o"
```

#### For Azure OpenAI:

```bash
dotnet user-secrets set "AzureOpenAI:Endpoint" "https://your-resource.openai.azure.com/"
dotnet user-secrets set "AzureOpenAI:ApiKey" "your-api-key-here"
dotnet user-secrets set "AzureOpenAI:ChatDeploymentName" "gpt-4o"
```

### Verify User Secrets

```bash
# List all secrets for the current project
dotnet user-secrets list
```

### User Secrets Location

User secrets are stored outside your project directory:

- **Windows**: `%APPDATA%\Microsoft\UserSecrets\<user_secrets_id>\secrets.json`
- **macOS/Linux**: `~/.microsoft/usersecrets/<user_secrets_id>/secrets.json`

---

## Step 4: IDE-Specific Setup

### Visual Studio 2022

1. **Install Visual Studio 2022** (17.12 or later)
   - Workload: ".NET Desktop Development" or "ASP.NET and web development"
   
2. **Recommended Extensions**:
   - GitHub Copilot (optional)
   - PowerShell Tools (for scripts)

3. **Managing User Secrets**:
   - Right-click project → "Manage User Secrets"
   - Edit the `secrets.json` file that opens

4. **Run a Sample**:
   - Open the `.csproj` file or solution
   - Press F5 to run

### Visual Studio Code

1. **Install VS Code**: [https://code.visualstudio.com/](https://code.visualstudio.com/)

2. **Install Extensions**:
   ```
   # Required
   - C# Dev Kit (Microsoft)
   
   # Recommended
   - GitHub Copilot
   - REST Client
   - Markdown All in One
   ```

3. **Open Repository**:
   ```bash
   code .
   ```

4. **Managing User Secrets** (via terminal):
   ```bash
   cd src/samples/BasicAgent_AF
   dotnet user-secrets set "OpenAI:ApiKey" "your-key"
   ```

5. **Run a Sample**:
   - Open terminal: `` Ctrl+` ``
   - Navigate to project: `cd src/samples/BasicAgent_AF`
   - Run: `dotnet run`

### JetBrains Rider

1. **Install Rider**: [https://www.jetbrains.com/rider/](https://www.jetbrains.com/rider/)

2. **Recommended Plugins**:
   - GitHub Copilot
   - .NET Core User Secrets

3. **Managing User Secrets**:
   - Right-click project → Tools → .NET User Secrets → Open
   - Edit the JSON file

4. **Run a Sample**:
   - Open project or solution
   - Run/Debug configuration will be auto-created
   - Press Shift+F10 to run

---

## Step 5: Verify Installation

### Test .NET Installation

```bash
# Check .NET SDK version
dotnet --version
# Expected: 9.0.x

# Check installed SDKs
dotnet --list-sdks

# Check installed runtimes
dotnet --list-runtimes
```

### Build a Sample Project

```bash
# Navigate to a sample project
cd src/samples/BasicAgent_AF

# Restore packages
dotnet restore

# Build the project
dotnet build

# Run the project (requires configured secrets)
dotnet run
```

Expected output (if secrets are configured):
```
Starting Agent Framework Basic Sample...
Agent created successfully!
Response: [AI-generated response]
```

---

## Step 6: Running Examples

### Console Applications

```bash
cd src/samples/BasicAgent_AF
dotnet run
```

### Web APIs

```bash
cd modules/15-ASPNetCore-Integration/code-samples/web-api-af
dotnet run
```

Then open browser to: `http://localhost:5000/swagger`

### Running Tests

```bash
cd src/tests/UnitTests
dotnet test
```

---

## Common Setup Issues

### Issue: "SDK version not found"

**Problem**: Global.json specifies .NET 9, but it's not installed

**Solution**:
```bash
# Remove version constraint temporarily
mv global.json global.json.backup

# Or install .NET 9 SDK
```

### Issue: "Missing User Secrets"

**Problem**: Code fails with null configuration values

**Solution**:
```bash
# Initialize and configure secrets
dotnet user-secrets init
dotnet user-secrets set "OpenAI:ApiKey" "your-key"
```

### Issue: "Package restore failed"

**Problem**: NuGet packages can't be downloaded

**Solution**:
```bash
# Clear NuGet cache
dotnet nuget locals all --clear

# Restore packages
dotnet restore
```

### Issue: "Build errors with C# 12 features"

**Problem**: Older .NET SDK installed

**Solution**:
```bash
# Verify SDK version is 9.0+
dotnet --version

# Update to latest .NET 9 SDK
```

---

## Environment Variables (for scripts only)

Some automation scripts may use environment variables. These are **NOT** for application configuration:

### Windows (PowerShell)

```powershell
$env:OPENAI_API_KEY = "your-key"
```

### macOS/Linux (Bash)

```bash
export OPENAI_API_KEY="your-key"
```

**Note**: Applications should ALWAYS use User Secrets, not environment variables.

---

## Next Steps

Now that your environment is set up:

1. **Learn the Basics**: Start with [Module 01: Introduction](../modules/01-Introduction/)
2. **Try a Sample**: Run [BasicAgent_AF](../src/samples/BasicAgent_AF/)
3. **Read the Quick Reference**: Check [QUICK-REFERENCE.md](QUICK-REFERENCE.md)
4. **Begin Migration**: Follow [migration-checklist.md](migration-checklist.md)

---

## Additional Resources

- [IDE Setup Guide](IDE-SETUP-GUIDE.md) - Detailed IDE configuration
- [Troubleshooting](TROUBLESHOOTING.md) - Common issues and solutions
- [FAQ](FAQ.md) - Frequently asked questions

---

## Getting Help

If you encounter issues not covered here:

1. Check [TROUBLESHOOTING.md](TROUBLESHOOTING.md)
2. Search [GitHub Issues](https://github.com/elbruno/a2aapiredemo/issues)
3. Ask in [GitHub Discussions](https://github.com/elbruno/a2aapiredemo/discussions)
4. Review [FAQ.md](FAQ.md)

---

**Environment ready? Let's start migrating!** 🚀
