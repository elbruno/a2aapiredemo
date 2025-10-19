# Hands-On Labs

Practical, hands-on exercises to reinforce your learning of Agent Framework migration.

## Lab Overview

Each lab includes:
- **Learning objectives**
- **Starter code** with intentional gaps
- **Step-by-step instructions**
- **Solution code** for reference
- **Verification steps**

## Available Labs

### [Lab 01: Basic Migration](./lab-01-basic-migration/)
**Duration**: 30 minutes  
**Level**: Beginner

Migrate a simple Semantic Kernel chatbot to Agent Framework with GitHub Models.

**What you'll learn**:
- Update NuGet packages
- Convert plugin to functions
- Use GitHub Models
- Test your migration

---

### [Lab 02: Tool Migration](./lab-02-tool-migration/)
**Duration**: 45 minutes  
**Level**: Intermediate

Migrate a multi-tool agent with database operations.

**What you'll learn**:
- Convert complex plugins
- Handle async functions
- Integrate with external services
- Test function calling

---

### [Lab 03: ASP.NET Integration](./lab-03-aspnet-integration/)
**Duration**: 60 minutes  
**Level**: Intermediate

Build a web API with Agent Framework and GitHub Models.

**What you'll learn**:
- Set up dependency injection
- Create REST API endpoints
- Implement streaming responses
- Add health checks and logging

---

### [Lab 04: Testing Strategies](./lab-04-testing-strategies/)
**Duration**: 45 minutes  
**Level**: Advanced

Comprehensive testing with unit tests, integration tests, and mocks.

**What you'll learn**:
- Write unit tests with mocks
- Integration tests with GitHub Models
- Test function independently
- Set up CI/CD testing

---

## Prerequisites

- .NET 9 SDK installed
- GitHub account with token
- Basic C# knowledge
- Text editor or IDE (VS Code, Visual Studio, Rider)

## Getting Started

1. **Clone the repository**
   ```bash
   git clone https://github.com/elbruno/a2aapiredemo.git
   cd a2aapiredemo/labs
   ```

2. **Set up GitHub token**
   ```bash
   dotnet user-secrets init
   dotnet user-secrets set "GITHUB_TOKEN" "your-token-here"
   ```

3. **Choose a lab** and follow the instructions in its README

## Lab Structure

Each lab follows this structure:

```
lab-XX-name/
├── README.md           # Lab instructions
├── starter/            # Starting point with gaps
│   ├── Program.cs
│   ├── *.csproj
│   └── appsettings.json
└── solution/           # Complete solution
    ├── Program.cs
    ├── *.csproj
    └── appsettings.json
```

## Tips for Success

1. **Read instructions carefully** - Each step builds on the previous
2. **Don't peek at solutions** - Try to solve it yourself first
3. **Test frequently** - Build and run after each major step
4. **Use GitHub Models** - Free testing removes barriers
5. **Ask for help** - Use GitHub Discussions if stuck

## Learning Path

Recommended order:
1. **Lab 01** - Start here for basic migration
2. **Lab 02** - Advanced tool patterns
3. **Lab 03** - Web API integration
4. **Lab 04** - Testing and quality

## Additional Resources

- [Module 02: Basic Migration](../modules/02-Basic-Migration/)
- [Quick Reference](../docs/QUICK-REFERENCE.md)
- [FAQ](../docs/FAQ.md)
- [Blog Posts](../blog-posts/)

---

**Ready to get hands-on?** Start with [Lab 01: Basic Migration](./lab-01-basic-migration/)! 🚀
