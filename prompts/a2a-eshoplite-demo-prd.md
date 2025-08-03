# Product Requirements Document (PRD)

## Project: Agent2Agent (A2A) eShopLite Demo

### 1. Overview

This project demonstrates a practical Agent2Agent (A2A) scenario using the eShopLite sample as a foundation. The goal is to show how multiple autonomous agents (microservices) can collaborate to fulfill a complex business request, leveraging the A2A SDK for seamless agent-to-agent communication and orchestration.

**References:**

- C# A2A SDK: <https://github.com/a2aproject/a2a-dotnet>
- SDK Overview: <https://devblogs.microsoft.com/foundry/building-ai-agents-a2a-dotnet-sdk/>
- Base scenario: <https://github.com/Azure-Samples/eShopLite/tree/main/scenarios/01-SemanticSearch>

---

### 2. Goals & Objectives

- Demonstrate multi-agent orchestration for product search enrichment.
- Showcase decoupled agent responsibilities and dynamic agent-to-agent communication.
- Aggregate data from multiple sources (inventory, promotions, research).
- Provide a foundation for extensible, agent-based architectures using the A2A SDK.

---

### 3. Functional Requirements

#### 3.1 Solution Structure

- Solution Path: `src\eShopLite-A2A.slnx`
- All agents must be added to Aspire orchestration and communicate using the A2A SDK.

#### 3.2 Agents (Web Projects)

- **Inventory Agent**
  - Provides real-time stock levels for products.
  - API: `/api/inventory/check` — Accepts a product ID, returns current stock quantity.
- **Promotions Agent**
  - Supplies active promotions or discounts for products.
  - API: `/api/promotions/active` — Accepts a product ID, returns current promotions.
- **Researcher Agent**
  - Delivers product insights, reviews, or ratings.
  - API: `/api/researcher/insights` — Accepts a product ID, returns aggregated insights.
- **Products Agent**
  - Orchestrates the search process and aggregates responses from all agents.
  - API: `A2ASearch` endpoint — Accepts a search query, coordinates with other agents, and returns a comprehensive result.

#### 3.3 Orchestration Flow

- When a user calls the `A2ASearch` endpoint on the Products agent:
  1. Products Agent receives the search request and identifies relevant products.
  2. For each product:
      - Queries Inventory Agent for stock levels.
      - If in stock, queries Promotions Agent for active promotions.
      - Queries Researcher Agent for product insights or reviews.
  3. Products Agent aggregates all responses and returns a unified result, including product details, stock status, promotions, and insights.

---

### 4. Non-Functional Requirements

- The solution must build and run successfully end-to-end.
- Code should be clean, maintainable, and well-documented.
- The architecture and orchestration flow must be documented.

---

### 5. Implementation Tasks

- [ ] Add and implement the three new web projects (Inventory, Promotions, Researcher) as described above.
- [ ] Integrate all agents into Aspire orchestration.
- [ ] Implement the `A2ASearch` endpoint in the Products agent to coordinate the multi-agent workflow.
- [ ] Ensure the solution builds and runs successfully end-to-end.
- [ ] Document the architecture, agent responsibilities, orchestration flow, and any implementation details or challenges.

---

### 6. Acceptance Criteria

- All agents are implemented and integrated as described.
- The `A2ASearch` endpoint returns enriched product search results by orchestrating all agents.
- The solution builds and runs without errors.
- Documentation is complete and clear.

---

### 7. Out of Scope

- Advanced agent patterns (e.g., dynamic agent discovery, failure handling) are not required for this demo but may be referenced for future work.

---

### 8. Expected Outcome

By the end of this demo, users should understand how to:

- Design and implement agent-based architectures using the A2A SDK.
- Orchestrate complex workflows across multiple agents.
- Extend the scenario with new agents or features with minimal coupling.

---
