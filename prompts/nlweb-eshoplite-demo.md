# Agent2Agent (A2A) eShopLite Demo Scenario

## Overview

This demo showcases a practical Agent2Agent (A2A) scenario using the eShopLite sample as a foundation. The goal is to demonstrate how multiple autonomous agents (microservices) can collaborate to fulfill a complex business request, leveraging the A2A SDK for seamless agent-to-agent communication and orchestration.

**References:**

- C# A2A SDK: <https://github.com/a2aproject/a2a-dotnet>
- SDK Overview: <https://devblogs.microsoft.com/foundry/building-ai-agents-a2a-dotnet-sdk/>
- Base scenario: [eShopLite Semantic Search](https://github.com/Azure-Samples/eShopLite/tree/main/scenarios/01-SemanticSearch)

## Objective

Demonstrate a multi-agent orchestration where the Products service coordinates with Inventory, Promotions, and Researcher agents to deliver enriched product search results. This scenario highlights:

- Decoupled agent responsibilities
- Dynamic agent-to-agent communication
- Aggregation of data from multiple sources
- Real-world extensibility of the A2A SDK

## Solution Structure

- **Solution Path:** `src\eShopLite-A2A.slnx`
- **Agents (Web Projects):**
  - **Inventory Agent**
    - *Responsibility:* Provides real-time stock levels for products.
    - *API:* `/api/inventory/check` — Accepts a product ID, returns current stock quantity.
  - **Promotions Agent**
    - *Responsibility:* Supplies active promotions or discounts for products.
    - *API:* `/api/promotions/active` — Accepts a product ID, returns current promotions.
  - **Researcher Agent**
    - *Responsibility:* Delivers product insights, reviews, or ratings.
    - *API:* `/api/researcher/insights` — Accepts a product ID, returns aggregated insights.
- **Products Agent**
  - *Responsibility:* Orchestrates the search process and aggregates responses from all agents.
  - *API:* `A2ASearch` endpoint — Accepts a search query, coordinates with other agents, and returns a comprehensive result.

All agents are added to Aspire orchestration and communicate using the A2A SDK.

## Orchestration Flow

When a user calls the `A2ASearch` endpoint on the Products agent:

1. **Products Agent** receives the search request and identifies relevant products.
2. For each product:
    - Queries **Inventory Agent** for stock levels.
    - If in stock, queries **Promotions Agent** for active promotions.
    - Queries **Researcher Agent** for product insights or reviews.
3. **Products Agent** aggregates all responses and returns a unified result, including product details, stock status, promotions, and insights.

This flow demonstrates dynamic, runtime agent-to-agent orchestration, where each agent is responsible for a specific domain and can be extended or replaced independently.

## Implementation Tasks

- [ ] Add and implement the three new web projects (Inventory, Promotions, Researcher) as described above.
- [ ] Integrate all agents into Aspire orchestration.
- [ ] Implement the `A2ASearch` endpoint in the Products agent to coordinate the multi-agent workflow.
- [ ] Ensure the solution builds and runs successfully end-to-end.
- [ ] Document the architecture, agent responsibilities, orchestration flow, and any implementation details or challenges.

## Expected Outcome

By the end of this demo, users should understand how to:

- Design and implement agent-based architectures using the A2A SDK
- Orchestrate complex workflows across multiple agents
- Extend the scenario with new agents or features with minimal coupling

---
*This scenario provides a foundation for exploring advanced agent-based patterns, such as dynamic agent discovery, failure handling, and distributed decision-making, using the A2A SDK.*
