# Slide Content and Speaker Notes
### .NET Agentic Modernization Demo Session
### Prepared for: Bruno Capuano

---

## Overview

This document contains the high-level content for the presentation slides, including speaker notes for each slide. The presentation accompanies the live demo session documented in [Session Delivery Guide](session-delivery-guide.md) and [Speaker Demo Walkthrough](03_speaker-demo-walkthrough.md).

---

## Slide 1: Title Slide

### Slide Content
**Title**: Modernization Made Simple: Building Agentic .NET Applications

**Subtitle**: Transform your .NET apps with AI agents using .NET Aspire and Azure AI Foundry

**Presenter**: Bruno Capuano

### Speaker Notes
> Welcome everyone! Today we're going to explore a powerful way to modernize .NET applications using AI agents. I'll show you how to take a traditional e-commerce application and transform it into an intelligent, agent-powered solution—all without a complete rewrite.

---

## Slide 2: The Problem

### Slide Content
**Title**: The Challenge of Modern Applications

**Key Points**:
- Hardcoded business rules become maintenance nightmares
- If-else logic chains are rigid and error-prone
- Business requirements change faster than code can adapt
- Customer expectations for personalization are increasing

### Speaker Notes
> Every enterprise has applications with deeply embedded business logic. Think about discount calculations, validation rules, pricing strategies. These often become huge if-else chains that are difficult to maintain and even harder to change. As businesses evolve, these rigid systems struggle to keep up.

---

## Slide 3: What is Agentic Modernization?

### Slide Content
**Title**: Introducing Agentic Modernization

**Definition**: Using AI agents to replace rigid business logic with intelligent, adaptable components

**Key Characteristics**:
- **Intent-driven**: Describe what you want, not how to do it
- **Reasoning**: Agents can make decisions based on context
- **Composable**: Multiple agents can work together
- **Resilient**: Graceful fallback when AI is unavailable

### Speaker Notes
> Agentic modernization means replacing your hardcoded business rules with AI agents that can reason and adapt. Instead of writing "if customer is Gold, apply 20% discount," you tell the agent "apply appropriate membership discounts based on tier." The agent figures out the logic. This is intent-driven programming.

---

## Slide 4: The Architecture

### Slide Content
**Title**: Multi-Agent Architecture

**Diagram Elements**:
- **Orchestrator**: Coordinates the workflow
- **StockAgent**: Validates inventory availability
- **DiscountAgent**: Computes tier-based discounts
- **Azure OpenAI**: Powers the reasoning

**Benefits**:
- Single responsibility for each agent
- Easy to replace or upgrade individual agents
- Observable and debuggable

### Speaker Notes
> Our architecture uses multiple specialized agents coordinated by an orchestrator. Each agent has one job. The StockAgent checks inventory, the DiscountAgent calculates membership discounts. The orchestrator runs them in sequence and combines the results. This separation makes the system maintainable and testable.

---

## Slide 5: The Tech Stack

### Slide Content
**Title**: Technology Stack

**Core Technologies**:
- **.NET Aspire**: Cloud-native orchestration
- **Microsoft Agent Framework**: Building AI agents
- **Azure OpenAI**: GPT models for reasoning
- **Blazor Server**: Interactive web UI
- **Azure AI Foundry**: Production deployment (optional)

### Speaker Notes
> We're using the latest .NET technologies. Aspire handles our distributed application orchestration. The Microsoft Agent Framework provides the patterns for building AI agents. Azure OpenAI powers the reasoning with GPT models. And for production scenarios, Azure AI Foundry offers enterprise-grade agent management.

---

## Slide 6: Demo Introduction

### Slide Content
**Title**: Live Demo: eShop Lite Transformation

**What We'll Do**:
1. Start with a working e-commerce app (baseline)
2. Add AI-powered discount calculations
3. Add intelligent stock validation
4. Create a multi-agent checkout workflow
5. See agent steps visualized in the UI

### Speaker Notes
> Let's see this in action! I have an eShop Lite application that works today—it has products, a cart, and checkout. But the checkout is simple and static. We're going to add AI agents to make it intelligent. Watch how little code we need to add.

---

## Slide 7: Demo Step 1 — Baseline Application

### Slide Content
**Title**: Starting Point: Baseline eShop Lite

**Features**:
- Product catalog with semantic search
- Shopping cart functionality
- Simple checkout flow
- Customer membership tiers (Gold, Silver, Normal)

**What's Missing**:
- No automated discounts
- No intelligent stock messaging
- Static checkout process

### Speaker Notes
> [DEMO] Let me run the baseline application. As you can see, we have a functioning e-commerce store. Customers can browse products, add them to cart, and checkout. But notice—there's no discount being applied even though Alice is a Gold member. The checkout just shows the subtotal. Let's fix that.

---

## Slide 8: Demo Step 2 — The DiscountAgent

### Slide Content
**Title**: Adding the DiscountAgent

**System Prompt Approach**:
```
Rules:
- GOLD → 20% discount
- SILVER → 10% discount
- NORMAL → No discount
```

**Key Pattern**:
- Define intent, not implementation
- AI produces the business rule
- Fallback to deterministic logic when AI unavailable

### Speaker Notes
> [DEMO] Now I'm adding the DiscountAgent. Notice we're not writing if-else logic. We're describing the discount rules in a system prompt. The AI agent will apply these rules. This is the key insight—we describe what we want, not how to compute it. And critically, we have a fallback for when AI is unavailable.

---

## Slide 9: Demo Step 3 — Multi-Agent Workflow

### Slide Content
**Title**: Orchestrating Multiple Agents

**Workflow**:
1. **StockAgent** → Validates availability, generates friendly message
2. **DiscountAgent** → Computes membership discount
3. **Result** → Combined checkout with full transparency

**Agent Steps UI**:
- ✅ StockAgent: "Great news! All items are in stock..."
- ✅ DiscountAgent: "Gold member 20% discount applied"

### Speaker Notes
> [DEMO] Now let's add the orchestrator that coordinates both agents. The StockAgent runs first to check inventory. Then the DiscountAgent calculates the discount. Both agents log their steps so users can see exactly what happened. This transparency builds trust in AI-powered systems.

---

## Slide 10: The Code Difference

### Slide Content
**Title**: Before and After

**Before** (Traditional Logic):
```csharp
if (tier == MembershipTier.Gold)
    discount = subtotal * 0.20m;
else if (tier == MembershipTier.Silver)
    discount = subtotal * 0.10m;
else
    discount = 0m;
```

**After** (Microsoft Agent Framework):
```csharp
// Create AIAgent with instructions (system prompt)
var discountAgent = _chatClient.CreateAIAgent(
    instructions: AgentInstructions,
    name: "DiscountAgent");

// Run the agent with user message
var response = await discountAgent.RunAsync(
    $"Tier: {tier}, Subtotal: {subtotal}");
var content = response.Text;
```

### Speaker Notes
> Here's the fundamental difference. On the left, traditional if-else logic. When business rules change, you modify code. On the right, the Microsoft Agent Framework approach using `CreateAIAgent` and `RunAsync`. When rules change, you update the agent instructions. The AI adapts. Plus, the agent can explain its reasoning—try doing that with if-else chains!

---

## Slide 11: Production Considerations

### Slide Content
**Title**: Taking Agents to Production

**Azure AI Foundry Benefits**:
- Persistent agent definitions
- Workflow management and reuse
- Full audit trails
- Model version management
- Enterprise security and compliance

**Best Practices**:
- Always implement fallback logic
- Log agent decisions for debugging
- Monitor latency and costs
- Test with diverse inputs

### Speaker Notes
> When you're ready for production, Azure AI Foundry provides enterprise-grade capabilities. You get persistent agents, audit trails, version management. But even in development, follow best practices: always have fallback logic when AI is unavailable, log everything, and test extensively.

---

## Slide 12: Key Takeaways

### Slide Content
**Title**: Key Takeaways

1. **Agents replace rigid business rules** with flexible, intent-driven logic
2. **Multi-agent architecture** enables separation of concerns
3. **.NET Aspire** simplifies cloud-native orchestration
4. **Fallback patterns** ensure reliability
5. **Transparency** through agent step logging builds trust

### Speaker Notes
> Let me leave you with these key points. Agents let you describe intent instead of implementation. The multi-agent pattern keeps your system modular. Aspire handles the complexity of distributed systems. Always have fallbacks. And show users what the AI is doing—transparency builds trust.

---

## Slide 13: Resources

### Slide Content
**Title**: Resources & Next Steps

**Demo Repository**:
👉 https://github.com/elbruno/a2aapiredemo

**Documentation**:
- `/docs/session-delivery-guide.md` - Session overview
- `/docs/03_speaker-demo-walkthrough.md` - Step-by-step code guide

**Learn More**:
- .NET Aspire: https://learn.microsoft.com/dotnet/aspire
- Azure AI Foundry: https://azure.microsoft.com/products/ai-foundry
- Microsoft Agent Framework: https://aka.ms/agent-framework

### Speaker Notes
> All the code from today's demo is available on GitHub. The repository includes detailed documentation for running the demo yourself. I encourage you to clone it, experiment, and think about how you could apply these patterns to your own applications.

---

## Slide 14: Q&A / Closing

### Slide Content
**Title**: Questions?

**Contact**:
- GitHub: @elbruno
- Twitter: @elbruno

**Thank You!**

### Speaker Notes
> I'd love to answer any questions. Whether it's about the technical implementation, the agentic patterns, or how to get started with your own modernization journey—ask away! Thank you for your time today.

---

# Appendix: Additional Notes

## Key Messages to Repeat

Throughout the session, reinforce these core messages:

1. **"We didn't write if-else logic. We described intent."**
   - Emphasize the shift from imperative to declarative

2. **"Each agent does one job."**
   - Reinforce the single-responsibility principle

3. **"The fallback ensures reliability."**
   - Production systems need graceful degradation

4. **"Transparency builds trust."**
   - Users can see what the AI is doing

## Demo Flow Quick Reference

| Time | Action | Document Reference |
|------|--------|-------------------|
| 0:00 | Introduction | - |
| 2:00 | Baseline Demo | [Prerequisites](03_speaker-demo-walkthrough.md#prerequisites) |
| 5:00 | Concepts: Agents | - |
| 7:00 | DiscountAgent Demo | [Demo 1](03_speaker-demo-walkthrough.md#-demo-1-implement-the-discountagent-5-minutes) |
| 12:00 | Concepts: Orchestration | - |
| 15:00 | Multi-Agent Demo | [Demo 2](03_speaker-demo-walkthrough.md#-demo-2-implement-the-stockagent-3-minutes), [Demo 3](03_speaker-demo-walkthrough.md#-demo-3-implement-the-agentcheckoutorchestrator-4-minutes) |
| 22:00 | Production Considerations | - |
| 25:00 | Closing & Q&A | - |

## Troubleshooting Quick Tips

If something goes wrong during the demo:

1. **AI not responding**: Switch to demonstrating the fallback logic
2. **Build errors**: Have `/src-04-complete` open as a backup
3. **Slow responses**: Explain that production would have caching/optimization
4. **Connection issues**: Pre-record a backup video of the demo

---

# End of Document
