# Lab 01 Starter (Semantic Kernel)

This console application represents the **before** state for Lab 01. It uses Semantic Kernel with a `ChatCompletionAgent` and a plugin class that exposes weather-related functions.

## Prerequisites

- .NET 9 SDK
- GitHub Models token stored with User Secrets

```powershell
cd labs/lab-01-basic-migration/starter/before-sk
 dotnet user-secrets init
 dotnet user-secrets set "GITHUB_TOKEN" "ghp_your_token"
```

## Run the sample

```powershell
 dotnet restore
 dotnet run
```

## Highlights

- Semantic Kernel `Kernel` orchestrator
- `[KernelFunction]` based plugin class
- `ChatCompletionAgent` with `InvokeAsync`
- GitHub Models endpoint configuration via `appsettings.json`
