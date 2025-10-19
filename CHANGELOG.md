# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added
- Initial repository structure and foundation
- Comprehensive README.md with project overview
- LICENSE (MIT)
- CONTRIBUTING.md with contribution guidelines
- CODE_OF_CONDUCT.md for community standards
- Project uses .NET 9 SDK exclusively
- User Secrets configuration approach (no .env files)
- **Lab 02: Tool Migration** - Complete code samples for migrating customer management system with async database operations
  - Starter code (SK) with CustomerPlugin and 5 async CRUD functions
  - Solution code (AF) with functions using closures for database access
  - Comprehensive README with migration guide
- **Lab 03: ASP.NET Core Integration** - Complete web API samples with environment-based configuration
  - Starter code (SK) with Kernel dependency injection
  - Solution code (AF) with IChatClient and ChatClientAgent
  - Both regular and streaming endpoints
  - Health checks and monitoring
- **Lab 04: Testing Strategies** - Complete xUnit test projects
  - Starter code (SK) with ChatCompletionAgent tests
  - Solution code (AF) with ChatClientAgent tests
  - Function unit tests (fast, no AI)
  - Integration tests with GitHub Models (free!)
  - Theory-based tests for multiple scenarios
- Updated all blog posts (01-04) with links to lab code samples
- Enhanced labs/README.md with detailed descriptions of all labs

### In Progress
- Documentation folder structure
- Module 01: Introduction
- Module 02: Basic Migration
- Code samples for SK and AF comparison

## [0.1.0] - 2025-10-19

### Added
- Project initialization
- Migration plan document (plans/SK-to-AgentFx-Migration-Plan.md)
- Repository foundation files
- .gitignore for .NET projects
- global.json pinning .NET 9 SDK

---

## Release Notes

### Version 0.1.0 (October 19, 2025)

This is the initial release of the Semantic Kernel to Microsoft Agent Framework Migration Guide repository.

**Scope**: Repository foundation and planning phase

**What's Included**:
- Project structure and planning documents
- MIT License
- Contribution guidelines and code of conduct
- .NET 9 SDK configuration

**Coming Soon**:
- 15 comprehensive learning modules
- Side-by-side code samples
- Real-world case studies
- Performance benchmarks
- Testing strategies
- Blog post series
- Hands-on labs

**Target Audience**: .NET developers migrating from Semantic Kernel to Agent Framework

**Prerequisites**: .NET 9 SDK, IDE (VS 2022/VS Code/Rider), Azure OpenAI or OpenAI API

---

## Version History

| Version | Date | Description |
|---------|------|-------------|
| 0.1.0 | 2025-10-19 | Initial release with foundation |

---

## Upcoming Milestones

### Phase 1: Foundation (Week 1) ✅
- [x] Repository structure
- [x] Root-level files
- [x] Basic documentation

### Phase 2: Documentation (Weeks 2-3) 🚧
- [ ] Complete docs/ folder
- [ ] Setup guides
- [ ] API mapping reference

### Phase 3: Core Modules (Weeks 2-6) 📋
- [ ] Modules 01-11 (Basic to Advanced)
- [ ] Code samples for each module
- [ ] Demo scripts

### Phase 4: Advanced Content (Weeks 7-9) 📋
- [ ] Modules 12-15 (Real-world, Performance, Testing)
- [ ] Case studies
- [ ] Benchmarks

### Phase 5: Supporting Materials (Weeks 10-12) 📋
- [ ] Hands-on labs
- [ ] PowerShell scripts
- [ ] Blog posts

### Phase 6: Launch (Week 13) 📋
- [ ] Final testing
- [ ] Documentation review
- [ ] Community setup
- [ ] Public release

---

## Notes

- All code samples target .NET 9
- Configuration uses User Secrets (no .env files)
- All examples in C# only
- Side-by-side SK and AF implementations for comparison
- Production-ready code with error handling and logging

---

For detailed information about changes in each version, see the Git commit history.
