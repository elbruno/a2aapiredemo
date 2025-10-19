# Lab 01 Solution (Agent Framework)

The solution for Lab 01 shows the migrated Weather Bot using Microsoft Agent Framework and the `Microsoft.Extensions.AI` abstractions.

## Prerequisites

- .NET 9 SDK
- GitHub Models token stored via User Secrets

```powershell
cd labs/lab-01-basic-migration/solution/after-af
 dotnet user-secrets init
 dotnet user-secrets set "GITHUB_TOKEN" "ghp_your_token"
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
