# Module 03: Namespace and Package Updates

**Duration**: 10 minutes  
**Level**: Beginner  
**Prerequisites**: Understanding of NuGet package management

---

## Learning Objectives

By the end of this module, you will:

- Understand which packages to remove and add
- Know how to update namespace imports  
- Recognize the new package structure for Agent Framework
- Successfully update a project's dependencies

---

## Package Migration Overview

The migration from Semantic Kernel to Agent Framework requires updating NuGet packages and namespace imports. This is typically the first step in any migration.

---

## Package Changes

### Remove Semantic Kernel Packages

```xml
<!-- Remove these from your .csproj -->
<PackageReference Include="Microsoft.SemanticKernel" Version="1.61.0" />
<PackageReference Include="Microsoft.SemanticKernel.Agents.Core" Version="1.61.0-alpha" />
```

### Add Agent Framework Packages

```xml
<!-- Add these to your .csproj -->
<PackageReference Include="Microsoft.Extensions.AI" Version="9.10.0" />
<PackageReference Include="Microsoft.Extensions.AI.OpenAI" Version="9.10.0-preview.1.25513.3" />
<PackageReference Include="Microsoft.Agents.AI" Version="1.0.0-preview.251001.1" />
```

---

## Namespace Changes

### Semantic Kernel → Agent Framework

| Semantic Kernel | Agent Framework |
|-----------------|-----------------|
| `Microsoft.SemanticKernel` | `Microsoft.Extensions.AI` |
| `Microsoft.SemanticKernel.Agents` | `Microsoft.Agents.AI` |
| `Microsoft.SemanticKernel.ChatCompletion` | `Microsoft.Extensions.AI` |

**Before:**
```csharp
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
```

**After:**
```csharp
using Microsoft.Extensions.AI;
using Microsoft.Agents.AI;
using OpenAI.Chat;
```

---

## Step-by-Step Migration

### Step 1: Update .csproj

```bash
# Remove old packages
dotnet remove package Microsoft.SemanticKernel

# Add new packages
dotnet add package Microsoft.Extensions.AI --version 9.10.0
dotnet add package Microsoft.Extensions.AI.OpenAI --prerelease
dotnet add package Microsoft.Agents.AI --prerelease
```

### Step 2: Update Using Statements

Replace all SK namespaces with AF equivalents in your .cs files.

### Step 3: Restore and Verify

```bash
dotnet restore
dotnet build
```

---

## Complete Example .csproj

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net9.0</TargetFramework>
    <Nullable>enable</Nullable>
    <UserSecretsId>your-id</UserSecretsId>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.Extensions.AI" Version="9.10.0" />
    <PackageReference Include="Microsoft.Extensions.AI.OpenAI" Version="9.10.0-preview.1.25513.3" />
    <PackageReference Include="Microsoft.Agents.AI" Version="1.0.0-preview.251001.1" />
    <PackageReference Include="Microsoft.Extensions.Configuration" Version="9.0.0" />
    <PackageReference Include="Microsoft.Extensions.Configuration.UserSecrets" Version="9.0.0" />
  </ItemGroup>
</Project>
```

---

## Next Steps

Continue to [Module 04: Agent Creation](../04-Agent-Creation/)! 🔧
