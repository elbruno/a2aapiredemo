# BRK-447 Technical Presenter Guide: Real-World AI Development with Visual Studio and GitHub Copilot

This comprehensive guide provides step-by-step instructions for technical presenters delivering session BRK-447, which demonstrates a real-world AI development scenario using Visual Studio, GitHub Copilot, and Azure AI services.

---

## Overview

**Session Title**: BRK-447 - Building AI-Powered Applications with Visual Studio and GitHub Copilot  
**Duration**: Approximately 45-60 minutes  
**Target Audience**: Developers, DevOps engineers, and technical architects  
**Prerequisites**: Basic knowledge of .NET, Visual Studio, and Git workflows  

### Session Objectives

By the end of this session, attendees will understand how to:
- Set up Azure AI services and local development environment
- Use GitHub Copilot effectively in Visual Studio for AI application development
- Leverage MCP servers to extend GitHub Copilot capabilities
- Implement AI search functionality with proper testing strategies
- Use GitHub Copilot Agents for automated code generation and UI updates
- Follow best practices for AI-assisted development workflows

---

## Pre-Session Preparation

### 1. Review Initial Setup Requirements

**📋 Action**: Thoroughly review the [Initial Setup documentation](./01-InitialSetup.md) to prepare all required resources.

**Key preparation steps**:
- Ensure all prerequisites are installed (Visual Studio 2022, Docker Desktop, PowerShell, Azure CLI, .NET 9 SDK)
- Verify Azure CLI authentication and tenant access
- Pre-deploy Azure AI Foundry resources if possible
- Test local development environment setup
- Prepare backup connection strings and credentials

**⏱️ Time allocation**: 15-20 minutes before session

### 2. Create GitHub Issue and Assign to Coding Agent

**📋 Action**: Create a demonstration GitHub issue that will be used during the session to showcase GitHub Copilot Agent capabilities.

**Steps**:
1. Create a new issue in your demo repository
2. Use the template from [Create Issue for Unit Tests](./Create_Issue_for_unit_tests.md) as a reference
3. Assign the issue to GitHub Copilot Coding Agent
4. Prepare the issue description with clear, actionable requirements

**Sample Issue Title**: "Implement comprehensive unit tests for AI search functionality"

**⏱️ Time allocation**: 5 minutes during demo

### 3. Execute PRD Integration

**📋 Action**: Ensure the Product Requirements Document (PRD) is available and downloadable for integration into the repository.

**Reference Document**: [PRD: Add Mock Payment Server](./PRD_Add_Payment_Mock_Server.md)

**Preparation steps**:
- Review the PRD thoroughly to understand the payment server requirements
- Ensure the PRD is accessible and can be downloaded during the demo
- Prepare to show how PRDs integrate with GitHub Copilot Agent workflows
- Have the PRD ready for potential live implementation during Q&A

**⏱️ Time allocation**: Reference during demo as needed

---

## Demo Flow and Video Modules

### Module 1: Initial Setup and Environment Configuration
**⏱️ Duration**: 8-10 minutes

**📁 Reference Guide**: [Initial Setup User Manual](./brk447-01-Initial%20Setup-gen-01/brk447-01-Initial%20Setup-en-US-02-userguide.md)

**Key talking points**:
- Azure CLI authentication and tenant selection
- Resource group creation and AI Foundry deployment
- Local script execution for connection strings
- Application configuration and user secrets management
- Azure Portal verification of deployed resources

**Demo flow**:
1. Show Azure CLI login process
2. Execute deployment commands
3. Retrieve and configure connection strings
4. Start local application and verify functionality

---

### Module 2: Zava Application Architecture Overview
**⏱️ Duration**: 5-7 minutes

**📁 Reference Guide**: [Zava Overview User Manual](./brk447-02-Zava%20Overview-gen-01/brk447-02-Zava%20Overview-en-US-02-userguide.md)

**Key talking points**:
- Solution architecture and project structure
- .NET Aspire orchestration benefits
- Service discovery and inter-service communication
- Local development vs. production deployment patterns

**Demo flow**:
1. Open solution in Visual Studio
2. Explain project relationships and dependencies
3. Show Aspire dashboard and service discovery
4. Highlight key architectural decisions

---

### Module 3: GitHub Copilot in Visual Studio - Features and Modes
**⏱️ Duration**: 8-10 minutes

**📁 Reference Guide**: [VS2022 and GitHub Copilot Overview User Manual](./brk447-03-VS2022%20and%20GHCP%20Overview-gen-01/brk447-03-VS2022%20and%20GHCP%20Overview-en-US-02-userguide.md)

**Key talking points**:
- GitHub Copilot integration in Visual Studio
- Understanding consumption and billing models
- Ask mode vs. Agent mode differences
- Model selection and performance considerations

**Demo flow**:
1. Open GitHub Copilot panel in Visual Studio
2. Show consumption meter and plan status
3. Demonstrate Ask mode for code understanding
4. Switch to Agent mode for code modifications
5. Compare different AI models (GPT-4, GPT-5 if available)

---

### Module 4: AI Search Implementation with Testing
**⏱️ Duration**: 8-10 minutes

**📁 Reference Guide**: [Add Single Unit Test for AI Search User Manual](./brk447-04%20Add%20single%20unit%20Test%20for%20AI%20Search-gen-01/brk447-04%20Add%20single%20unit%20Test%20for%20AI%20Search-en-US-02-userguide.md)

**Key talking points**:
- Test-driven development with AI assistance
- GitHub Copilot's understanding of testing patterns
- AI search functionality implementation
- Unit test creation and validation strategies

**Demo flow**:
1. Create new unit test class
2. Use Copilot to generate test scaffolding
3. Implement AI search functionality with Copilot assistance
4. Run tests and validate implementation
5. Show test coverage and code quality improvements

---

### Module 5: Extending Copilot with MCP Servers
**⏱️ Duration**: 6-8 minutes

**📁 Reference Guide**: [Add MCP Servers User Manual](./brk447-05%20add%20mcp%20servers-gen-01/brk447-05%20add%20mcp%20servers-en-US-02-userguide.md)

**Key talking points**:
- MCP (Model Context Protocol) servers overview
- Extending GitHub Copilot capabilities
- GitHub and Microsoft Docs integration
- Configuration and authentication processes

**Demo flow**:
1. Create MCP configuration file
2. Add GitHub MCP server entry
3. Configure Microsoft Docs MCP server
4. Authenticate and verify new tools
5. Show expanded tool capabilities in Copilot UI

---

### Module 6: Querying External Documentation with MCP
**⏱️ Duration**: 6-8 minutes

**📁 Reference Guide**: [Query MCP MS Learn User Manual](./brk447-06-query%20mcp%20ms%20learn-gen-01/brk447-06-query%20mcp%20ms%20learn-en-US-02-userguide.md)

**Key talking points**:
- Leveraging external documentation in development
- Microsoft Learn integration benefits
- EF Core 9 best practices and implementation
- Documentation-driven development approach

**Demo flow**:
1. Enable GitHub Copilot Agent with Microsoft Docs access
2. Query Microsoft Learn for EF Core guidance
3. Implement recommended database initialization patterns
4. Apply agent-suggested code changes
5. Validate against official documentation

---

### Module 7: GitHub Issue-Driven Development with Copilot Agent
**⏱️ Duration**: 8-10 minutes

**📁 Reference Guide**: [Implement Unit Tests Using GitHub Issue User Manual](./brk447-07-Implement%20unit%20tests%20using%20GH%20Issue-gen-01/brk447-07-Implement%20unit%20tests%20using%20GH%20Issue-en-US-02-userguide.md)

**Key talking points**:
- GitHub Issues as development requirements
- Copilot Agent understanding of issue context
- Automated code generation from issue descriptions
- Issue-to-implementation workflow automation

**Demo flow**:
1. Reference the GitHub issue created in preparation
2. Assign issue to GitHub Copilot Coding Agent
3. Show agent analysis and implementation plan
4. Execute agent-generated code changes
5. Validate implementation against issue requirements
6. Update issue status and close with commit references

---

### Module 8: UI Development with Image-Based Agent Guidance
**⏱️ Duration**: 8-10 minutes

**📁 Reference Guide**: [Update UI Using Agent Based on Images User Manual](./brk447-08-update%20ui%20using%20agent%20based%20on%20images-gen-01/brk447-08-update%20ui%20using%20agent%20based%20on%20images-en-US-02-userguide.md)

**Key talking points**:
- Visual design-to-code workflows
- Image analysis capabilities of GitHub Copilot
- UI component generation and styling
- Accessibility and responsive design considerations

**Demo flow**:
1. Provide reference images or mockups to Copilot Agent
2. Request UI updates based on visual specifications
3. Show agent-generated HTML/CSS/Blazor components
4. Apply changes and validate visual output
5. Demonstrate responsive behavior and accessibility features

---

## Session Timing and Flow

### Recommended Timeline

| Module | Duration | Cumulative Time |
|--------|----------|-----------------|
| Introduction & Agenda | 3 min | 3 min |
| Module 1: Initial Setup | 10 min | 13 min |
| Module 2: Architecture Overview | 7 min | 20 min |
| Module 3: Copilot Features | 10 min | 30 min |
| Module 4: AI Search & Testing | 10 min | 40 min |
| Module 5: MCP Servers | 8 min | 48 min |
| Module 6: Documentation Query | 8 min | 56 min |
| Module 7: Issue-Driven Development | 10 min | 66 min |
| Module 8: Image-Based UI Updates | 10 min | 76 min |
| Q&A and Wrap-up | 9 min | 85 min |

**Total Session Duration**: ~75-85 minutes (adjust based on available time slot)

---

## Backup Plans and Troubleshooting

### Technical Backup Scenarios

1. **Azure Connectivity Issues**
   - Have pre-configured connection strings ready
   - Use local SQLite databases as fallback
   - Prepare screenshots of successful deployments

2. **GitHub Copilot Service Issues**
   - Have pre-recorded demos available
   - Prepare code snippets for manual demonstration
   - Use cached responses for critical demonstrations

3. **Visual Studio Integration Problems**
   - Test with VS Code as backup IDE
   - Ensure extension versions are compatible
   - Have offline documentation available

### Timing Adjustments

- **Running Behind**: Skip Module 2 (Architecture Overview) and focus on hands-on demos
- **Running Ahead**: Expand Q&A, show additional MCP integrations, or dive deeper into PRD implementation
- **Technical Issues**: Use pre-recorded segments while troubleshooting live environment

---

## Key Messaging and Takeaways

### Primary Messages

1. **GitHub Copilot transforms development workflows** by providing intelligent, context-aware assistance
2. **MCP servers extend capabilities** beyond basic code completion to include external documentation and tools
3. **Issue-driven development** can be automated and streamlined with AI assistance
4. **Visual design specifications** can be efficiently translated to working code
5. **Testing and documentation** are integral parts of AI-assisted development workflows

### Audience Takeaways

- Practical knowledge of GitHub Copilot features and modes
- Understanding of MCP server configuration and benefits
- Hands-on experience with AI-assisted testing strategies
- Insights into issue-driven development automation
- Awareness of visual design-to-code capabilities

---

## Post-Session Resources

### Follow-Up Materials

1. **Repository Access**: Provide attendees with access to the demo repository
2. **Documentation Links**: Share all user manual links for self-paced learning
3. **Setup Scripts**: Make Azure deployment scripts available for download
4. **Sample Issues**: Provide templates for GitHub issue creation
5. **MCP Configuration**: Share working MCP server configurations

### Additional Learning Paths

- [Microsoft Learn: GitHub Copilot Fundamentals](https://learn.microsoft.com/training/paths/copilot/)
- [Visual Studio Copilot Documentation](https://docs.microsoft.com/visualstudio/ide/visual-studio-github-copilot)
- [Azure AI Services Documentation](https://docs.microsoft.com/azure/cognitive-services/)
- [.NET Aspire Orchestration Guides](https://learn.microsoft.com/dotnet/aspire/)

---

## Presenter Notes and Tips

### Engagement Strategies

- **Interactive Elements**: Ask audience about their current Copilot usage
- **Live Coding**: Encourage questions during implementation segments
- **Real-World Context**: Relate demos to common development scenarios
- **Problem-Solution Framing**: Present each module as solving specific developer challenges

### Common Questions and Responses

**Q: "How accurate is GitHub Copilot for production code?"**
A: Demonstrate testing and validation workflows; emphasize human oversight and code review processes.

**Q: "What about security and intellectual property concerns?"**
A: Discuss enterprise features, data handling policies, and organizational configuration options.

**Q: "How does this integrate with existing development workflows?"**
A: Show git integration, CI/CD compatibility, and team collaboration features.

**Q: "What's the learning curve for adoption?"**
A: Provide realistic timelines and suggest incremental adoption strategies.

### Success Metrics

- **Engagement**: Active participation in Q&A sessions
- **Understanding**: Questions that build on demonstrated concepts
- **Interest**: Requests for additional resources and follow-up sessions
- **Implementation Intent**: Audience expressing plans to try demonstrated workflows

---

## Quick Reference Links

### Essential Documentation
- [Initial Setup Guide](./01-InitialSetup.md)
- [PRD Template](./PRD_Add_Payment_Mock_Server.md)
- [Issue Creation Template](./Create_Issue_for_unit_tests.md)

### Individual Module Guides
- [Module 1: Initial Setup](./brk447-01-Initial%20Setup-gen-01/brk447-01-Initial%20Setup-en-US-02-userguide.md)
- [Module 2: Architecture Overview](./brk447-02-Zava%20Overview-gen-01/brk447-02-Zava%20Overview-en-US-02-userguide.md)
- [Module 3: Copilot Features](./brk447-03-VS2022%20and%20GHCP%20Overview-gen-01/brk447-03-VS2022%20and%20GHCP%20Overview-en-US-02-userguide.md)
- [Module 4: AI Search Testing](./brk447-04%20Add%20single%20unit%20Test%20for%20AI%20Search-gen-01/brk447-04%20Add%20single%20unit%20Test%20for%20AI%20Search-en-US-02-userguide.md)
- [Module 5: MCP Servers](./brk447-05%20add%20mcp%20servers-gen-01/brk447-05%20add%20mcp%20servers-en-US-02-userguide.md)
- [Module 6: Documentation Query](./brk447-06-query%20mcp%20ms%20learn-gen-01/brk447-06-query%20mcp%20ms%20learn-en-US-02-userguide.md)
- [Module 7: Issue-Driven Development](./brk447-07-Implement%20unit%20tests%20using%20GH%20Issue-gen-01/brk447-07-Implement%20unit%20tests%20using%20GH%20Issue-en-US-02-userguide.md)
- [Module 8: Image-Based UI Updates](./brk447-08-update%20ui%20using%20agent%20based%20on%20images-gen-01/brk447-08-update%20ui%20using%20agent%20based%20on%20images-en-US-02-userguide.md)

---

*This guide was generated to support session BRK-447 and provides comprehensive coverage of GitHub Copilot and Visual Studio integration for AI-powered development workflows. For questions or updates to this guide, please refer to the repository documentation or contact the session organizers.*