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
2. **Introduce a simple agent**  
   - Apply user-tier based discounts.
3. **Move to multi-agent workflows**  
   - Stock validation → discount → cart summary.
4. **Show how the same workflow can run in the cloud**  
   - Azure AI Foundry + Microsoft Agent Framework.
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
| Demo 1: Baseline eShop Lite | 3 min | Show app working today: browse, search, cart, checkout | [Prerequisites](03_speaker-demo-walkthrough.md#prerequisites) |
| Concepts: What are agents? | 2 min | Agent reasoning, tools, workflows, and why they simplify business logic | - |
| Demo 2: Add DiscountAgent | 5 min | First live-coded feature: tier-based discounts | [Demo 1: Implement the DiscountAgent](03_speaker-demo-walkthrough.md#-demo-1-implement-the-discountagent-5-minutes) |
| Concepts: Multi-agent orchestration | 3 min | Stock agent, discount agent, cart agent | - |
| Demo 3: Multi-agent checkout workflow | 4 min | Show agent steps, final totals, collaboration | [Demo 2: Implement the StockAgent](03_speaker-demo-walkthrough.md#-demo-2-implement-the-stockagent-3-minutes) and [Demo 3: Implement the AgentCheckoutOrchestrator](03_speaker-demo-walkthrough.md#-demo-3-implement-the-agentcheckoutorchestrator-4-minutes) |
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

📖 **Code Implementation Guide**: See [Prerequisites](03_speaker-demo-walkthrough.md#prerequisites) for setup instructions.

### Key Messages
- "This is where most .NET apps are today."
- "It works, but logic is rigid."
- "Let's modernize without rewriting everything."

---

## 🤖 Step 1 — Add DiscountAgent (Live)

You or Copilot will add:
- Discount agent service  
- Logic to apply Gold (20%) or Silver (10%) discount  
- Update checkout UI with:
  - DiscountAmount  
  - DiscountReason  

📖 **Code Implementation Guide**: Follow the step-by-step instructions in [Demo 1: Implement the DiscountAgent](03_speaker-demo-walkthrough.md#-demo-1-implement-the-discountagent-5-minutes).

**Steps to follow during demo**:
1. [Add the System Prompt](03_speaker-demo-walkthrough.md#step-11-add-the-system-prompt)
2. [Replace the ComputeDiscountAsync Method](03_speaker-demo-walkthrough.md#step-12-replace-the-computediscountasync-method)
3. [Add Helper Methods](03_speaker-demo-walkthrough.md#step-13-add-helper-methods)
4. [Add Required Using Statements](03_speaker-demo-walkthrough.md#step-14-add-required-using-statements)

### Key Messages
- "We didn't write if-else logic."
- "We only described intent — the agent produced the business rule."

---

## 🧩 Step 2 — Add StockAgent + Multi-Agent Workflow

Implement:
- StockAgent for availability checks  
- AgentCheckoutOrchestrator that runs:
  - StockAgent  
  - DiscountAgent  
  - Summary agent logic  

Update UI to display:
- Agent steps log  
- Validation messages  
- Calculated totals  

📖 **Code Implementation Guide**: 
- Follow [Demo 2: Implement the StockAgent](03_speaker-demo-walkthrough.md#-demo-2-implement-the-stockagent-3-minutes) for the StockAgent implementation.
- Follow [Demo 3: Implement the AgentCheckoutOrchestrator](03_speaker-demo-walkthrough.md#-demo-3-implement-the-agentcheckoutorchestrator-4-minutes) for the orchestrator implementation.

**Steps to follow during demo**:

**StockAgent**:
1. [Add the System Prompt](03_speaker-demo-walkthrough.md#step-21-add-the-system-prompt)
2. [Replace the CheckStockAsync Method](03_speaker-demo-walkthrough.md#step-22-replace-the-checkstockasync-method)
3. [Add the GenerateSummaryMessage Method](03_speaker-demo-walkthrough.md#step-23-add-the-generatesummarymessage-method)

**AgentCheckoutOrchestrator**:
1. [Replace the ProcessCheckoutAsync Method](03_speaker-demo-walkthrough.md#step-31-replace-the-processcheckoutasync-method)
2. [Add the RunStockAgent Method](03_speaker-demo-walkthrough.md#step-32-add-the-runstockagent-method)
3. [Add the RunDiscountAgent Method](03_speaker-demo-walkthrough.md#step-33-add-the-rundiscountagent-method)

### Key Messages
- "Each agent does one job."
- "This architecture is maintainable and easily replaced."

---

## ☁ Step 3 — Connect to Azure AI Foundry (Optional)

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

📖 **Additional Resources**: See [Complete Code Reference](03_speaker-demo-walkthrough.md#-complete-code-reference) for the full implementation details in `/src-complete`.

---

# 🧭 7. Live Demo Tips
- Keep logs visible  
- Switch tiers during demo to show changes  
- Use small, clear sample products  
- Reinforce incremental modernization  
- Keep Foundry section optional  

📖 **Troubleshooting**: If you encounter issues during the demo, refer to the [Troubleshooting](03_speaker-demo-walkthrough.md#-troubleshooting) section.

---

# 🎤 8. Closing Script

> _"This session showed how any .NET app — even a simple store — can evolve using agents.  
> You don't need a big rewrite, just smart components that reason and collaborate.  
> .NET Aspire, Azure AI Foundry, and Microsoft Agent Framework give developers a clear path to intelligent modernization."_

---

# 🏁 End of Document
