# Modernization Made Simple — Full Session Context & Delivery Guide
### For Bruno Capuano — .NET Agentic Modernization Demo Session  
### Event: .NET Day on Agentic Modernization  
### Duration: 25–30 minutes  
### Version: Speaker Ready

---

# 🎯 1. Session Goal

The purpose of this session is to demonstrate **how a traditional .NET application can be progressively modernized** by introducing **AI agents** using:

- **.NET Aspire**
- **Microsoft Agent Framework**
- **Azure OpenAI / Azure AI Foundry**
- **Multi-agent workflows**

All of this is shown using a working sample app (eShop Lite), which evolves from a simple Aspire app into an **agent‑powered, intelligent e-commerce experience**.

The audience must leave understanding:

✔ What "agentic modernization" means  
✔ How agents can replace rigid business rules  
✔ How to apply these techniques to existing .NET applications  
✔ How Aspire helps orchestrate an AI‑enabled architecture  
✔ How to integrate with Azure AI Foundry for production‑grade agent deployments  

---

# 🧱 2. Application Used for the Live Demo

The application for the demo is the repo:

👉 **https://github.com/elbruno/a2aapiredemo**

### Folder Structure

| Folder | Purpose |
|--------|---------|
| `/src-start` | Baseline app with system prompt and helpers pre-defined (for Step 0 and Step 1) |
| `/src-step2` | Complete implementation of StockAgent + Multi-Agent Workflow |
| `/src-step3` | Full solution with DI registration and DevUI debugging support |
| `/src-complete` | Reference implementation with all features |
| `/src-slides` | Python script for generating PowerPoint presentation |

### Baseline Features (Before Modernization)
- Blazor Server Store front-end  
- Products API  
- SQL database with EF Core  
- Vector search using Azure OpenAI embeddings  
- Cart + checkout flow (functional, but static)  
- .NET Aspire orchestrating the system  

### Modernized Features (After Demonstration)
- **Agent-based discount calculation** (DiscountAgent)  
- **Agent-based stock validation** (StockAgent)  
- **Agent steps log visualized in UI**  
- **Multi-agent checkout orchestrator**  
- **DI-registered agents with DevUI debugging**
- **Optional cloud‑based workflow using Azure AI Foundry**

---

# 🧩 3. Session Pitch (The Story You'll Tell)

This is the narrative presented to the audience.

---

## 🌟 Pitch Summary

> _"Today we're taking a simple .NET Aspire app and evolving it into an intelligent, agent‑powered solution. Instead of hardcoded pricing rules, rigid validations, and if-else forests, we'll use agents that can reason, validate, apply business logic, and collaborate through a workflow."_

The transformation is incremental:

1. **Start with a normal, modern Aspire app**
   - Great architecture, no AI agents yet.
2. **Introduce a simple agent (Live Coding)**  
   - Apply user-tier based discounts.
3. **Move to multi-agent workflows (Pre-built)**  
   - Stock validation → discount → cart summary.
4. **Add DevUI for debugging (Pre-built)**
   - Register agents for DI and use DevUI to debug.
5. **Wrap up with benefits**
   - Flexibility  
   - Lower maintenance  
   - Greater personalization  
   - Faster modernization  

---

# ⏱️ 4. Demo Timeline (25–30 minutes)

| Segment | Duration | Description | Demo Code Reference |
|--------|----------|-------------|---------------------|
| Intro: Modernization landscape | 2 min | Why apps need intelligent behavior; what agentic modernization solves | - |
| Demo 0: Baseline eShop Lite | 3 min | Show app working today: browse, search, cart, checkout | `/src-start` |
| Concepts: What are agents? | 2 min | Agent reasoning, tools, workflows, and why they simplify business logic | - |
| Demo 1: Add DiscountAgent (Live) | 5 min | Live-code the ComputeDiscountAsync method | `/src-start` |
| Concepts: Multi-agent orchestration | 2 min | Stock agent, discount agent, orchestrator | - |
| Demo 2: Multi-agent workflow | 4 min | Open `/src-step2`, show pre-built implementation | `/src-step2` |
| Demo 3: DI + DevUI | 4 min | Open `/src-step3`, show DI registration and DevUI debugging | `/src-step3` |
| Cloud: Foundry + Agent Framework | 2–3 min | Explain optional production architecture | - |
| Closing: Key lessons + call to action | 2 min | Summarize benefits + encourage adoption | - |

Total: **25–30 minutes**

---

# 🧠 5. Detailed Demo Flow

## 🚀 Step 0 — Show the app in baseline mode
- Open `/src-start` version  
- Run Aspire  
- Navigate Store front-end  
- Perform semantic search  
- Add product to cart  
- Checkout → simple subtotal only  

📖 **Code Implementation Guide**: See [Prerequisites](04_speaker-demo-walkthrough.md#prerequisites) for setup instructions.

### Key Messages
- "This is where most .NET apps are today."
- "It works, but logic is rigid."
- "Let's modernize without rewriting everything."

---

## 🤖 Step 1 — Add DiscountAgent (Live Coding)

The system prompt and helper methods are **already defined** in `/src-start` to make the demo easier to follow.

During the live demo, you only need to implement:
- The `ComputeDiscountAsync` method body

📖 **Code Implementation Guide**: Follow the step-by-step instructions in [Demo 1: Implement the DiscountAgent](04_speaker-demo-walkthrough.md#-demo-1-implement-the-discountagent-5-minutes).

**What's Pre-Built in `/src-start`**:
- ✅ System prompt with discount rules (Gold: 20%, Silver: 10%, Normal: 0%)
- ✅ `ParseAgentResponse` helper method
- ✅ `ComputeFallbackDiscount` helper method
- ✅ All required using statements

**Live Code During Demo**:
1. Replace the placeholder `ComputeDiscountAsync` method with AI-powered logic
2. Build and run to show the discount being applied

### Key Messages
- "We didn't write if-else logic."
- "We only described intent — the agent produced the business rule."
- "The system prompt and helpers were already defined to keep the demo focused."

---

## 🧩 Step 2 — Show Multi-Agent Workflow (Pre-Built)

Instead of live coding Step 2, **open the `/src-step2` folder** which has the complete implementation.

Walk through the code and explain:
- **StockAgentService**: AI-generated friendly stock status messages
- **AgentCheckoutOrchestrator**: Coordinates StockAgent → DiscountAgent workflow
- **Agent steps log**: Shows each agent's contribution in the UI

📖 **Code Reference**: 
- `src-step2/AgentServices/Stock/StockAgentService.cs`
- `src-step2/AgentServices/Checkout/AgentCheckoutOrchestrator.cs`

### Key Messages
- "Each agent does one job."
- "This architecture is maintainable and easily replaced."
- "The orchestrator coordinates the workflow — Stock first, then Discount."

---

## 🔧 Step 3 — DI Registration + Observability (Pre-Built)

Open the `/src-step3` folder to show advanced agent patterns:

### Features in Step 3:

1. **Proper DI Registration**
   - Agents registered with scoped lifetime
   - Centralized configuration via `AgentSettings`
   - Easy unit testing through interface injection

2. **Enhanced Observability**
   - Debug-level logging for agent operations
   - OpenTelemetry integration for tracing
   - Detailed request/response logging

📖 **Code Reference**: 
- `src-step3/Store/Program.cs` - Enhanced logging setup
- `src-step3/AgentServices/AgentServicesExtensions.cs` - DI patterns

### Key Messages
- "Registering agents in DI gives you proper lifecycle management."
- "Scoped lifetime means each request gets isolated agent instances."
- "This is essential for production-grade agent applications."

### DI Benefits to Highlight:
- **Testability**: Easy to mock agents in unit tests
- **Lifecycle**: Proper disposal of resources
- **Configuration**: Centralized settings management
- **Observability**: Built-in OpenTelemetry support

### Note on DevUI
For more advanced debugging, the Microsoft Agent Framework provides DevUI, which offers:
- Visual interface for agent debugging
- Inspect agent reasoning and message flows
- Test agent responses interactively

DevUI requires additional packages (`Microsoft.Agents.AI.DevUI`) and setup. See the [Agent Framework documentation](https://github.com/microsoft/agent-framework) for details.

---

## ☁ Step 4 — Connect to Azure AI Foundry (Optional)

Explain the cloud-deployed version:
- Persistent agents  
- Workflow reuse  
- Auditing + observability  
- Model version management  

---

# 📚 6. Required Knowledge for the Presenter
- .NET Aspire fundamentals  
- Microsoft Agent Framework basics  
- Azure OpenAI embeddings  
- Azure AI Foundry  
- Blazor + API interactions  
- Understanding of agentic concepts: tools, reasoning, workflows  

📖 **Additional Resources**: See [Complete Code Reference](04_speaker-demo-walkthrough.md#-complete-code-reference) for the full implementation details in `/src-complete`.

---

# 🧭 7. Live Demo Tips
- Keep logs visible  
- Switch tiers during demo to show changes  
- Use small, clear sample products  
- Reinforce incremental modernization  
- Have `/src-step2` and `/src-step3` ready to open for quick transitions
- Keep Foundry section optional  

📖 **Troubleshooting**: If you encounter issues during the demo, refer to the [Troubleshooting](04_speaker-demo-walkthrough.md#-troubleshooting) section.

---

# 🎤 8. Closing Script

> _"This session showed how any .NET app — even a simple store — can evolve using agents.  
> You don't need a big rewrite, just smart components that reason and collaborate.  
> .NET Aspire, Azure AI Foundry, and Microsoft Agent Framework give developers a clear path to intelligent modernization."_

---

# 🏁 End of Document
