# Lab 01 Solution (Agent Framework)

The solution for Lab 01 shows the migrated Weather Bot using Microsoft Agent Framework and the `Microsoft.Extensions.AI` abstractions.

## Prerequisites

- .NET 9 SDK
- One of the following:
  - GitHub personal access token (for GitHub Models - free!)
  - OpenAI API key
  - Azure AI Foundry endpoint and credentials

## Configuration

### 1. Initialize User Secrets

```bash
cd labs/lab-01-basic-migration/solution/after-af
dotnet user-secrets init
```

### 2. Configure Your AI Provider

Choose one of the following options:

#### Option A: GitHub Models (Free - Default)

```bash
dotnet user-secrets set "AI:Provider" "GitHubModels"
dotnet user-secrets set "GITHUB_TOKEN" "ghp_your_token"
```

#### Option B: OpenAI

```bash
dotnet user-secrets set "AI:Provider" "OpenAI"
dotnet user-secrets set "OpenAI:ApiKey" "your-openai-api-key"
dotnet user-secrets set "OpenAI:ChatDeploymentName" "gpt-4o-mini"
```

#### Option C: Azure AI Foundry (Managed Identity)

```bash
dotnet user-secrets set "AI:Provider" "AzureAIFoundry"
dotnet user-secrets set "AzureAIFoundry:Endpoint" "https://your-endpoint.inference.ai.azure.com"
dotnet user-secrets set "OpenAI:ChatDeploymentName" "gpt-4o-mini"
```

#### Option D: Azure AI Foundry (API Key)

```bash
dotnet user-secrets set "AI:Provider" "AzureAIFoundry"
dotnet user-secrets set "AzureAIFoundry:Endpoint" "https://your-endpoint.inference.ai.azure.com"
dotnet user-secrets set "AzureAIFoundry:ApiKey" "your-api-key"
dotnet user-secrets set "OpenAI:ChatDeploymentName" "gpt-4o-mini"
```

## Run the sample

```powershell
 dotnet restore
 dotnet run
```

## Highlights

- `ChatClient` + `ChatClientAgent`
- Direct function registration in `Tools`
- Simple `RunAsync` invocation pattern
- GitHub Models endpoint configuration
- Error handling around agent execution
