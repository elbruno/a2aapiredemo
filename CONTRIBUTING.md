# Contributing to Semantic Kernel to Agent Framework Migration Guide

Thank you for your interest in contributing to this project! This guide provides everything you need to know about contributing to our migration materials.

## Table of Contents

- [Code of Conduct](#code-of-conduct)
- [How Can I Contribute?](#how-can-i-contribute)
- [Getting Started](#getting-started)
- [Development Guidelines](#development-guidelines)
- [Pull Request Process](#pull-request-process)
- [Style Guides](#style-guides)
- [Community](#community)

---

## Code of Conduct

This project adheres to a Code of Conduct that all contributors are expected to follow. Please read [CODE_OF_CONDUCT.md](CODE_OF_CONDUCT.md) before contributing.

---

## How Can I Contribute?

### Reporting Issues

- **Bug Reports**: Found a problem in code samples? Submit an issue with details
- **Documentation**: Spotted an error or unclear explanation? Let us know
- **Feature Requests**: Have ideas for new samples or content? We'd love to hear

**Before submitting an issue:**
- Check if it already exists
- Provide detailed reproduction steps
- Include environment details (.NET version, OS, IDE)

### Contributing Code

We welcome contributions of:

1. **New Code Samples** - Additional SK/AF comparison examples
2. **Bug Fixes** - Corrections to existing samples
3. **Documentation** - Improvements to guides and READMEs
4. **Case Studies** - Real-world migration experiences
5. **Testing** - Additional test examples
6. **Benchmarks** - Performance comparison data

### Contributing Documentation

- Fix typos, grammar, or clarity issues
- Add missing explanations
- Improve code comments
- Create diagrams or visuals
- Translate content (future consideration)

---

## Getting Started

### Prerequisites

1. **Install .NET 9 SDK** (9.0.x or later)
   ```bash
   dotnet --version
   ```

2. **Fork and Clone**
   ```bash
   git clone https://github.com/YOUR-USERNAME/a2aapiredemo.git
   cd a2aapiredemo
   ```

3. **Create a Branch**
   ```bash
   git checkout -b feature/your-feature-name
   ```

4. **Configure User Secrets** (if working with code samples)
   ```bash
   dotnet user-secrets init
   dotnet user-secrets set "OpenAI:ApiKey" "test-key"
   ```

---

## Development Guidelines

### C# Code Standards

All C# code must follow these standards:

1. **Target .NET 9** exclusively
   ```xml
   <TargetFramework>net9.0</TargetFramework>
   ```

2. **Use User Secrets** for configuration (NO .env files)
   ```csharp
   // Good
   var apiKey = configuration["OpenAI:ApiKey"];
   
   // Bad - Never use .env files
   DotNetEnv.Env.Load();
   ```

3. **Follow C# Naming Conventions**
   - PascalCase for classes, methods, properties
   - camelCase for local variables, parameters
   - Async methods end with `Async`

4. **Use Async/Await Properly**
   ```csharp
   // Good
   public async Task<string> GetResponseAsync()
   {
       return await agent.RunAsync();
   }
   
   // Bad - Missing await
   public Task<string> GetResponse()
   {
       return agent.RunAsync();
   }
   ```

5. **Enable Nullable Reference Types**
   ```xml
   <Nullable>enable</Nullable>
   ```

6. **Include XML Documentation**
   ```csharp
   /// <summary>
   /// Creates a new AI agent using the specified chat client.
   /// </summary>
   /// <param name="client">The chat client for agent communication.</param>
   /// <returns>A configured AIAgent instance.</returns>
   public AIAgent CreateAgent(IChatClient client)
   ```

### Project Structure

When adding new samples:

```
modules/
  XX-ModuleName/
    ├── README.md              # Module overview
    ├── demo-script.md         # Step-by-step demo
    ├── presentation-notes.md  # Speaker notes
    └── code-samples/
        ├── before-sk/         # Semantic Kernel version
        │   ├── Program.cs
        │   ├── *.csproj
        │   └── README.md      # How to run, key concepts
        └── after-af/          # Agent Framework version
            ├── Program.cs
            ├── *.csproj
            └── README.md
```

### Naming Conventions

- **Semantic Kernel projects**: Suffix with `_SK` (e.g., `ChatBot_SK.csproj`)
- **Agent Framework projects**: Suffix with `_AF` (e.g., `ChatBot_AF.csproj`)
- **Branches**: 
  - Features: `feature/description`
  - Bugs: `bugfix/description`
  - Docs: `docs/description`

---

## Pull Request Process

### Before Submitting

1. **Test Your Changes**
   ```bash
   # Build all affected projects
   dotnet build
   
   # Run tests if applicable
   dotnet test
   ```

2. **Update Documentation**
   - Update relevant README files
   - Add comments explaining migration-specific changes
   - Update CHANGELOG.md if significant

3. **Verify Code Compiles**
   - All projects must compile without errors
   - No warnings (treat warnings as errors)

4. **Follow Style Guides** (see below)

### Submitting a Pull Request

1. **Push Your Changes**
   ```bash
   git add .
   git commit -m "Description of changes"
   git push origin feature/your-feature-name
   ```

2. **Create Pull Request**
   - Use a clear, descriptive title
   - Fill out the PR template completely
   - Reference related issues with `Fixes #123`

3. **PR Description Should Include**
   - What changes were made
   - Why the changes were made
   - How to test the changes
   - Screenshots (for UI changes)
   - Breaking changes (if any)

### Review Process

- Maintainers will review within 3-5 business days
- Address feedback constructively
- Keep discussions focused and respectful
- Once approved, maintainers will merge

---

## Style Guides

### Code Comments

Add inline comments explaining **migration-specific** changes:

```csharp
// SK: Used Kernel-based agent creation
// AF: Now using IChatClient directly without Kernel dependency
var agent = chatClient.CreateAIAgent();
```

### Markdown Style

- Use proper heading hierarchy (don't skip levels)
- Include code fences with language specification
- Add alt text for images
- Link to related content

```markdown
# Main Title (H1)

## Section (H2)

### Subsection (H3)

```csharp
// Code with language specification
var example = "value";
```

![Architecture Diagram](diagram.png "System architecture showing SK and AF comparison")
```

### Commit Messages

Follow conventional commits:

```
type(scope): brief description

Longer explanation if needed

Fixes #123
```

Types:
- `feat`: New feature
- `fix`: Bug fix
- `docs`: Documentation changes
- `refactor`: Code refactoring
- `test`: Adding tests
- `chore`: Maintenance

Examples:
```
feat(module-02): add basic migration code sample
fix(docs): correct API mapping table
docs(readme): improve quick start instructions
```

---

## Testing Guidelines

### Code Sample Testing

All code samples must:

1. **Compile Successfully**
   ```bash
   dotnet build --configuration Release
   ```

2. **Run Without Errors** (with valid config)
   ```bash
   dotnet run
   ```

3. **Produce Expected Output**
   - Document expected outputs in README
   - Include sample output in comments

### Documentation Testing

- All links must be valid
- Code examples must be syntactically correct
- Commands must execute successfully

---

## Documentation Standards

### README Structure for Code Samples

Every code sample must include a README.md with:

```markdown
# [Project Name]

## Overview
Brief description of what this sample demonstrates.

## Prerequisites
- .NET 9 SDK
- OpenAI or Azure OpenAI access

## Configuration

Configure User Secrets:
```bash
dotnet user-secrets set "OpenAI:ApiKey" "your-key"
```

## Running the Sample

```bash
dotnet run
```

## Key Concepts Demonstrated

- Concept 1: Explanation
- Concept 2: Explanation

## Migration Notes (for AF versions)

Describes how this differs from the SK version and why.
```

---

## Community

### Communication Channels

- **Issues**: Bug reports and feature requests
- **Discussions**: Q&A and general discussion
- **Pull Requests**: Code contributions

### Getting Help

- Check [FAQ.md](docs/FAQ.md) first
- Search existing issues
- Ask in GitHub Discussions
- Review [TROUBLESHOOTING.md](docs/TROUBLESHOOTING.md)

### Recognition

Contributors will be:
- Listed in CHANGELOG.md
- Acknowledged in release notes
- Credited in contributed content

---

## Questions?

If you have questions about contributing, please:

1. Check this guide thoroughly
2. Review existing issues and discussions
3. Open a new discussion if needed

Thank you for contributing! 🎉

---

**Note**: This project is maintained by [Bruno Capuano](https://github.com/elbruno) and the community. Response times may vary, but we appreciate your patience and contributions.
