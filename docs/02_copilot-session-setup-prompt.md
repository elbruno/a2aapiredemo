# GitHub Copilot Coding Agent Prompt  
### Prepare the Repository for a Two-Stage Conference Demo (Start vs Complete)

## 🎯 GOAL
Restructure this repository into **two parallel source folders** for a 30-minute agentic modernization live session:

### 1. `/src-01-start`
A **minimal**, fully building, fully running version of the app **without any agents implemented yet**, containing:
- eShop Lite baseline features  
- Vector search  
- Working checkout (no discounts, no stock agents)  
- Membership tier selector (non-functional)  
- All agent logic removed and replaced with **clear TODO instructions**

### 2. `/src-05-complete`
A **fully implemented agentic solution** (the current codebase), including:
- DiscountAgent  
- StockAgent  
- CartAgent  
- Multi-agent orchestrator  
- UI updates (discounts, agent steps)  
- Optional Azure AI Foundry workflow support  

Both folders must build cleanly using Aspire and run without manual fixes.

(… content shortened for brevity in this preview …)
