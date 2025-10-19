# Semantic Kernel to Microsoft Agent Framework Migration Guide

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4)](https://dotnet.microsoft.com/download/dotnet/9.0)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)
[![C#](https://img.shields.io/badge/Language-C%23-239120)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![GitHub Models](https://img.shields.io/badge/AI-GitHub_Models-181717)](https://github.com/marketplace/models)

> **Comprehensive migration materials for developers transitioning from Semantic Kernel to Microsoft Agent Framework in C# and .NET 9**

This repository provides everything you need to successfully migrate your AI agent applications from Semantic Kernel to the new Microsoft Agent Framework, including:

- 📚 **15 Comprehensive Modules** - Step-by-step learning path
- 💻 **Working Code Samples** - Side-by-side SK and AF examples using GitHub Models
- 🏭 **Real-World Case Studies** - Production migration examples
- 🧪 **Testing Strategies** - Comprehensive test examples
- 🎓 **Hands-On Labs** - Interactive learning exercises
- 📝 **Blog Post Series** - In-depth articles on [elbruno.com](https://www.elbruno.com)
- 🛠️ **Automation Scripts** - PowerShell tools to assist migration

---

## 🎯 Why Migrate to Agent Framework?

Microsoft Agent Framework represents the evolution of AI agent development in .NET, offering:

1. **Simplified API** - Less boilerplate, more intuitive
2. **Unified Interface** - Consistent patterns across AI providers
3. **Modern .NET** - Built on .NET 9 with latest C# features
4. **Future-Proof** - Microsoft's strategic direction for AI agents
5. **GitHub Models Ready** - Easy integration with free AI models

---

## 🚀 Quick Start

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) (9.0.x or later)
- IDE: [Visual Studio 2022](https://visualstudio.microsoft.com/) (17.12+), [VS Code](https://code.visualstudio.com/) with C# Dev Kit, or [JetBrains Rider](https://www.jetbrains.com/rider/)
- [GitHub Personal Access Token](https://github.com/settings/tokens) for GitHub Models access

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/elbruno/a2aapiredemo.git
   cd a2aapiredemo
   ```

2. **Verify .NET 9 SDK**
   ```bash
   dotnet --version
   # Should output 9.0.x
   ```

3. **Configure GitHub Token** 
   
   For any sample project:
   ```bash
   cd src/samples/BasicAgent_AF
   dotnet user-secrets init
   dotnet user-secrets set "GITHUB_TOKEN" "your-github-token"
   ```
   
   Get your token at: https://github.com/settings/tokens

4. **Run a sample**
   ```bash
   dotnet run
   ```

For detailed setup instructions, see [docs/SETUP-GUIDE.md](docs/SETUP-GUIDE.md).

---

## 📚 Learning Path

### Module Overview

| Module | Topic | Duration | Key Concepts |
|--------|-------|----------|--------------|
| [01](modules/01-Introduction/) | Introduction | 10 min | Why migrate, benefits, comparison |
| [02](modules/02-Basic-Migration/) | Basic Migration | 15 min | Hello World, core concepts |
| [03](modules/03-Namespace-And-Packages/) | Packages & Namespaces | 10 min | Package updates, imports |
| [04](modules/04-Agent-Creation/) | Agent Creation | 15 min | Simplified patterns |
| [05](modules/05-Thread-Management/) | Thread Management | 15 min | Conversation lifecycle |
| [06](modules/06-Tool-Registration/) | Tool Registration | 20 min | Plugins → Functions |
| [07](modules/07-Invocation-Patterns/) | Invocation Patterns | 15 min | RunAsync, response handling |
| [08](modules/08-Streaming-Responses/) | Streaming | 15 min | Real-time responses |
| [09](modules/09-Dependency-Injection/) | Dependency Injection | 15 min | ASP.NET Core integration |
| [10](modules/10-Options-Configuration/) | Configuration | 10 min | Settings, GitHub token |
| [11](modules/11-Complete-Example/) | Complete Example | 20 min | Full chatbot application |
| [12](modules/12-Real-World-Migrations/) | Real-World Cases | 25 min | Production migrations |
| [13](modules/13-Testing-Strategies/) | Testing | 20 min | Unit, integration tests |
| [14](modules/14-Production-Deployment/) | Production | 20 min | Deployment best practices |
| [15](modules/15-ASPNetCore-Integration/) | ASP.NET Core | 25 min | Web API, Blazor, SignalR |

### Suggested Learning Paths

**Quick Start (90 minutes)**: Modules 1-4, 6, 9, 11
**Comprehensive Workshop (3 hours)**: All modules in order
**Production Focus (2 hours)**: Modules 1-2, 9-15

---

## 💡 Blog Post Series

Comprehensive articles published on [elbruno.com](https://www.elbruno.com):

1. [**Why Migrate to Agent Framework**](blog-posts/01-why-migrate-to-agent-framework.md) - Benefits, decision factors, and ROI
2. [**Step-by-Step Migration Guide**](blog-posts/02-step-by-step-migration-guide.md) - Complete walkthrough with examples
3. [**Real-World Migration Examples**](blog-posts/03-real-world-migration-examples.md) - Enterprise case studies
4. [**Testing and Best Practices**](blog-posts/04-testing-and-best-practices.md) - Testing strategies and production readiness

---

## 🔬 Code Samples

All code samples follow these principles:

- ✅ **C# Only** - All examples in C#, no Python or JavaScript
- ✅ **.NET 9** - Latest framework features
- ✅ **GitHub Models** - Free AI models for development and testing
- ✅ **User Secrets** - Secure token configuration
- ✅ **Side-by-Side** - SK version (_SK) and AF version (_AF) for comparison
- ✅ **Production-Ready** - Error handling, logging, best practices

### Sample Structure

```
src/
├── samples/              # Standalone runnable samples
│   ├── BasicAgent_SK/    # Semantic Kernel version
│   ├── BasicAgent_AF/    # Agent Framework version
│   └── ...
├── case-studies/         # Real-world migration examples
└── tests/               # Test example projects
```

---

## 🧪 Hands-On Labs

Interactive exercises with starter code and solutions:

- [Lab 01: Basic Migration](labs/lab-01-basic-migration/) - Convert a simple SK agent
- [Lab 02: Tool Migration](labs/lab-02-tool-migration/) - Transform plugins to functions
- [Lab 03: ASP.NET Integration](labs/lab-03-aspnet-integration/) - Web API with agents
- [Lab 04: Testing Strategies](labs/lab-04-testing-strategies/) - Comprehensive testing

---

## 📖 Documentation

### Essential Guides

- [**Setup Guide**](docs/SETUP-GUIDE.md) - Environment setup and prerequisites
- [**IDE Setup Guide**](docs/IDE-SETUP-GUIDE.md) - VS 2022, VS Code, Rider configuration
- [**Presenter Guide**](docs/PRESENTER-GUIDE.md) - For demo hosts and speakers
- [**Quick Reference**](docs/QUICK-REFERENCE.md) - One-page cheat sheet
- [**Migration Checklist**](docs/migration-checklist.md) - Step-by-step migration tasks
- [**API Mapping**](docs/api-mapping.md) - SK to AF API translation
- [**Troubleshooting**](docs/TROUBLESHOOTING.md) - Common issues and solutions
- [**FAQ**](docs/FAQ.md) - Frequently asked questions

### Advanced Topics

- [**Incremental Migration Guide**](docs/incremental-migration-guide.md) - Side-by-side approach
- [**Advanced Scenarios**](docs/advanced-scenarios.md) - Multi-agent, RAG, vector DB
- [**Security Guide**](docs/security-guide.md) - Best practices for production
- [**Common Pitfalls**](docs/common-pitfalls.md) - What to avoid

---

## 🛠️ Automation Scripts

PowerShell scripts to assist with migration:

```bash
# Verify environment setup
./scripts/setup/verify-environment.ps1

# Configure GitHub token for all projects
./scripts/setup/configure-secrets.ps1

# Analyze SK patterns in your code
./scripts/automation/migration-analyzer.ps1 -Path "C:\MyProject"

# Update package references
./scripts/automation/package-updater.ps1

# Validate migration completeness
./scripts/validation/verify-migration.ps1
```

See [scripts/README.md](scripts/README.md) for details.

---

## 🎤 For Presenters and Demo Hosts

This repository includes complete materials for delivering live presentations:

- **Demo Scripts** - Exact commands and expected outputs
- **Timing Guides** - Module duration estimates
- **Presenter Notes** - Talking points and engagement tips
- **Troubleshooting** - Handle issues during live demos
- **Multiple Formats** - 90-min, 3-hour workshop, 4-session series

See [docs/PRESENTER-GUIDE.md](docs/PRESENTER-GUIDE.md) to get started.

---

## 🔄 Migration Decision Tree

Not sure if you should migrate? Use our decision framework:

```
Do you use Semantic Kernel in production?
├─ Yes → Are you on .NET 6/8 and planning to upgrade to .NET 9?
│  ├─ Yes → Migrate now (best time for modernization)
│  └─ No → Evaluate benefits vs. effort (see Module 01)
└─ No → Start fresh with Agent Framework
```

See [docs/migration-decision-tree.md](docs/migration-decision-tree.md) for the complete framework.

---

## 🤝 Contributing

We welcome contributions! Please see [CONTRIBUTING.md](CONTRIBUTING.md) for:

- How to contribute code, docs, or samples
- Coding standards and conventions
- Pull request process
- Issue reporting guidelines

---

## 📞 Support and Community

- **Issues**: [GitHub Issues](https://github.com/elbruno/a2aapiredemo/issues)
- **Discussions**: [GitHub Discussions](https://github.com/elbruno/a2aapiredemo/discussions)
- **Blog**: [elbruno.com](https://www.elbruno.com)
- **Author**: [Bruno Capuano](https://github.com/elbruno)

---

## 📚 External Resources

- [Agent Framework Documentation](https://learn.microsoft.com/en-us/agent-framework/)
- [Migration Guide (Official)](https://learn.microsoft.com/en-us/agent-framework/migration-guide/from-semantic-kernel/)
- [Agent Framework GitHub](https://github.com/microsoft/agent-framework)
- [Semantic Kernel Documentation](https://learn.microsoft.com/en-us/semantic-kernel/)

---

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## 🙏 Acknowledgments

- Microsoft Agent Framework Team
- Semantic Kernel Community
- All contributors and early adopters

---

**Ready to migrate?** Start with [Module 01: Introduction](modules/01-Introduction/) or jump straight to [Module 02: Basic Migration](modules/02-Basic-Migration/) if you're already familiar with Agent Framework concepts.

---

<p align="center">
  <strong>Built with ❤️ by <a href="https://www.elbruno.com">Bruno Capuano</a></strong>
</p>
