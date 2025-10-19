# GitHub Copilot Instructions for Semantic Kernel to Agent Framework Migration

## Repository Purpose

This repository contains comprehensive materials for migrating from Semantic Kernel to Microsoft Agent Framework in C#. It includes demos, code samples, blog posts, and presenter materials.

## Code Generation Guidelines

### When Creating Semantic Kernel Examples (before-sk/)

- Use `Microsoft.SemanticKernel` namespace
- Use `Kernel` for agent management
- Use `[KernelFunction]` attributes for tools
- Use `ChatCompletionAgent` for agents
- Use `InvokeAsync` and `InvokeStreamingAsync` methods
- Target .NET 9

### When Creating Agent Framework Examples (after-af/)

- Use `Microsoft.Agents.AI` and `Microsoft.Extensions.AI` namespaces
- Use `IChatClient` for agent management
- Use direct function registration (no attributes)
- Use `AIAgent` for agents
- Use `RunAsync` and `RunStreamingAsync` methods
- Target .NET 9
- Leverage extension methods like `CreateAIAgent()`

## Naming Conventions

- Semantic Kernel projects: Suffix with `_SK` (e.g., `BasicAgent_SK.csproj`)
- Agent Framework projects: Suffix with `_AF` (e.g., `BasicAgent_AF.csproj`)
- Keep identical functionality between SK and AF versions for comparison

## Configuration Standards

**CRITICAL**: All projects must use .NET User Secrets for configuration. NO .env files allowed.

```csharp
// Good - Using IConfiguration with User Secrets
var apiKey = configuration["OpenAI:ApiKey"];

// Bad - Never use .env files or environment variables directly
// DotNetEnv.Env.Load();
```

### User Secrets Setup

```bash
dotnet user-secrets init
dotnet user-secrets set "OpenAI:ApiKey" "your-key-here"
dotnet user-secrets set "OpenAI:ChatDeploymentName" "gpt-4o"
```

### appsettings.json Structure

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  },
  "OpenAI": {
    "ChatDeploymentName": "gpt-4o"
  }
}
```

## Documentation Standards

- All code samples must include README.md with:
  - Purpose and overview
  - Prerequisites
  - User Secrets configuration steps
  - How to run
  - Key concepts demonstrated
  - Migration notes (for AF versions)
- Use markdown for all documentation
- Include code comments explaining migration-specific changes

## Testing Requirements

- All code samples must compile without errors
- Include unit tests where appropriate
- Ensure SK and AF versions produce equivalent outputs
- Document any behavioral differences

## Blog Post Guidelines

- Target audience: .NET developers familiar with Semantic Kernel
- Include working code examples
- Link to corresponding GitHub code samples
- Use consistent formatting and style
- Include performance comparisons where relevant
- Add SEO optimization (keywords, meta descriptions, Open Graph tags)

## Presenter Materials

- Demo scripts should include exact commands
- Include expected outputs
- Provide troubleshooting tips
- Note timing for each section
- Include engagement prompts for live audiences

## Common Patterns to Follow

### Agent Creation Pattern

**Semantic Kernel:**
```csharp
var kernel = Kernel.CreateBuilder()
    .AddOpenAIChatCompletion("gpt-4", apiKey)
    .Build();
var agent = new ChatCompletionAgent { Kernel = kernel };
```

**Agent Framework:**
```csharp
var client = new OpenAIChatClient("gpt-4", apiKey);
var agent = client.CreateAIAgent();
```

### Tool Registration Pattern

**Semantic Kernel:**
```csharp
public class WeatherPlugin
{
    [KernelFunction]
    public string GetWeather(string location) => $"Weather in {location}";
}
kernel.Plugins.Add(KernelPluginFactory.CreateFromType<WeatherPlugin>());
```

**Agent Framework:**
```csharp
string GetWeather(string location) => $"Weather in {location}";
var agent = client.CreateAIAgent(tools: GetWeather);
```

## Repository-Specific Rules

1. Always create side-by-side SK and AF examples
2. Maintain consistency in functionality across versions
3. Document all migration steps clearly
4. Include performance metrics where applicable
5. Follow .NET 9 best practices
6. Use async/await properly
7. Implement proper error handling
8. Include XML documentation comments
9. All configuration via User Secrets and IConfiguration
10. No .env files or direct environment variable access

## File Organization

- `modules/` - Contains 15 teaching modules
- `src/` - Runnable code samples and projects
- `docs/` - Supporting documentation
- `labs/` - Hands-on exercises
- `scripts/` - Automation and setup scripts
- `blog-posts/` - Blog post content for www.elbruno.com
- `plans/` - Planning and architectural documents

## When Adding New Content

- Update relevant README files
- Add entry to CHANGELOG.md
- Ensure consistency with existing patterns
- Test all code samples
- Update navigation/links if needed
- Follow contribution guidelines in CONTRIBUTING.md

## Project Structure Template

```
modules/XX-ModuleName/
├── README.md
├── demo-script.md
├── presentation-notes.md
└── code-samples/
    ├── before-sk/
    │   ├── Program.cs
    │   ├── *.csproj
    │   ├── appsettings.json
    │   └── README.md
    └── after-af/
        ├── Program.cs
        ├── *.csproj
        ├── appsettings.json
        └── README.md
```

## Target Framework

All projects must target .NET 9:

```xml
<PropertyGroup>
  <TargetFramework>net9.0</TargetFramework>
  <Nullable>enable</Nullable>
</PropertyGroup>
```

## Package Versions

### Semantic Kernel (Latest Stable)
```xml
<PackageReference Include="Microsoft.SemanticKernel" Version="1.61.0" />
<PackageReference Include="Microsoft.SemanticKernel.Agents.Core" Version="1.61.0-alpha" />
```

### Agent Framework (Latest)
```xml
<PackageReference Include="Microsoft.Agents.AI" Version="1.0.0" />
<PackageReference Include="Microsoft.Extensions.AI" Version="9.0.0" />
<PackageReference Include="Microsoft.Extensions.AI.OpenAI" Version="9.0.0" />
```

## Quality Standards

- All code must compile without errors or warnings
- Follow C# naming conventions
- Use proper async/await patterns
- Include error handling
- Add XML documentation for public APIs
- Write clear, self-documenting code
- Add inline comments for migration-specific changes
