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
- **Frontend store project:** `src\Store` (this is the main web UI for users to perform product searches)
- **Products main backend API:** `src\Products` (this is the main backend API for product-related operations and agent orchestration)
- All agents must be added to Aspire orchestration and communicate using the A2A SDK. *Aspire is a .NET application composition and orchestration framework for cloud-native apps. See: <https://devblogs.microsoft.com/dotnet/introducing-dotnet-aspire/>*
- The solution must include the Store project as the FrontEnd, which allows users to perform product searches.

#### 3.2 Agents (Web Projects)

- **Inventory Agent**
  - Provides real-time stock levels for products.
  - API: `/api/inventory/check` — Accepts a product ID, returns current stock quantity.
    - Example request: `{ "productId": "string" }`
    - Example response: `{ "productId": "string", "stock": 42 }`
- **Promotions Agent**
  - Supplies active promotions or discounts for products.
  - API: `/api/promotions/active` — Accepts a product ID, returns current promotions.
    - Example request: `{ "productId": "string" }`
    - Example response: `{ "productId": "string", "promotions": [ { "title": "string", "discount": 10 } ] }`
- **Researcher Agent**
  - Delivers product insights, reviews, or ratings.
  - API: `/api/researcher/insights` — Accepts a product ID, returns aggregated insights.
    - Example request: `{ "productId": "string" }`
    - Example response: `{ "productId": "string", "insights": [ { "review": "string", "rating": 4.5 } ] }`
- **Products Agent**
  - Orchestrates the search process and aggregates responses from all agents.
  - API: `A2ASearch` endpoint — Accepts a search query, coordinates with other agents, and returns a comprehensive result.
    - Example request: `{ "query": "string" }`
    - Example response: `{ "products": [ { "productId": "string", "name": "string", "stock": 42, "promotions": [ ... ], "insights": [ ... ] } ] }`
  - Also exposes endpoints for standard and semantic search as in the base scenario.

#### 3.3 Orchestration Flow

- When a user calls the `A2ASearch` endpoint on the Products agent:
  1. Products Agent receives the search request and identifies relevant products.
  2. For each product:
      - Queries Inventory Agent for stock levels.
      - If in stock, queries Promotions Agent for active promotions.
      - Queries Researcher Agent for product insights or reviews.
  3. Products Agent aggregates all responses and returns a unified result, including product details, stock status, promotions, and insights.

#### 3.4 FrontEnd Requirements

- The search page in the Store project (`src\Store`) must include a combo box (dropdown) that allows the user to select the search type: Standard Search, Semantic Search, or A2A Search.
- Based on the selected search type, the frontend must call the appropriate endpoint in the Products API (`src\Products`):
  - Standard Search → standard search endpoint
  - Semantic Search → semantic search endpoint
  - A2A Search → `A2ASearch` endpoint
- The UI must display the enriched results returned by the A2A Search, including product details, stock status, promotions, and insights.
- The UI must be accessible (WCAG 2.1 AA compliant) and responsive for desktop and mobile devices.

---

### 4. Non-Functional Requirements

- The solution must build and run successfully end-to-end.
- Code should be clean, maintainable, and well-documented.
- The architecture and orchestration flow must be documented.
- The user experience in the FrontEnd must be intuitive, with clear feedback for each search type and error handling for failed agent calls.
- All APIs must return consistent, well-structured JSON responses.
- The system must gracefully handle agent failures (e.g., if Promotions agent is unavailable, return results with a warning and partial data).
- The A2A Search endpoint should respond within 2 seconds for typical queries (performance target).
- Unit and integration tests must be provided for each agent and the frontend, in addition to Playwright E2E tests.

---

### 5. Implementation Tasks

- [ ] Add and implement the three new web projects (Inventory, Promotions, Researcher) as described above.
- [ ] Integrate all agents into Aspire orchestration.
- [ ] Implement the `A2ASearch` endpoint in the Products agent to coordinate the multi-agent workflow.
- [ ] Update the FrontEnd project:
  - [ ] Add a combo box to the search page for selecting search type (Standard, Semantic, A2A).
  - [ ] Implement logic to call the correct Products endpoint based on the selected search type.
  - [ ] Display all relevant data in the UI, including enriched results from A2A Search.
- [ ] Ensure the solution builds and runs successfully end-to-end.
- [ ] Document the architecture, agent responsibilities, orchestration flow, and any implementation details or challenges.
- [ ] Use Playwright MCP Tools to automate navigation and testing of the site, and to take screenshots. Navigation must start from the AppHost Aspire project, then launch the store from there.
- [ ] Implement unit and integration tests for all agents and the frontend.

---

### 9. Glossary

- **A2A**: Agent-to-Agent, a pattern and SDK for enabling autonomous agents to communicate and collaborate.
- **Aspire**: .NET application composition and orchestration framework for cloud-native apps.
- **MCP**: Model Context Protocol, used for Playwright MCP Tools in automated testing.

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
