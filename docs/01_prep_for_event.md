Here’s the **mega-prompt** you can drop into **GitHub Copilot Coding Agents** for the `https://github.com/elbruno/a2aapiredemo` repo.

For reference, the Microsoft Agent Framework docs recommend using the following NuGet packages for Azure OpenAI + Foundry + workflows:

* `Azure.AI.OpenAI`, `Azure.Identity`, `Microsoft.Agents.AI.OpenAI` ([Microsoft Learn][1])
* `Azure.AI.Agents.Persistent`, `Microsoft.Agents.AI.AzureAI`, `Microsoft.Agents.AI.Workflows` ([Microsoft Learn][2])

---

````md
# GitHub Copilot Coding Agent Prompt: Prepare eShop Lite (a2aapiredemo) for 30-Min Agentic Demo

## Overview

Transform the existing **eShop Lite** .NET Aspire solution in this repository into a **demo-ready, agentic e-commerce app**.

The result must support a **30-minute conference session** with:
- A baseline “modern .NET + AI search” story.
- Two progressive upgrades:
  1. A **single Discount Agent** that applies membership-based discounts at checkout.
  2. A **multi-agent checkout workflow** (Stock + Discount + Cart summary), locally and optionally wired to **Azure AI Foundry / Microsoft Agent Framework**.

The priority is **demo flow + clarity**, not maximum abstraction.

---

## Context (Repository & Existing Features)

You are working in the repo: `https://github.com/elbruno/a2aapiredemo`

From the README and structure:

- Solution: `src-start/eShopLite-Aspire-Modernization.slnx` (or `src-complete/`)
- Key projects:
  - `src-start/eShopAppHost`        → .NET Aspire host
  - `src-start/eShopServiceDefaults`→ shared service configuration
  - `src-start/Products`            → ASP.NET Core API with AI search
  - `src-start/Store`               → Blazor Server front-end
  - `src-start/CartEntities`        → cart/order models
  - `src-start/DataEntities`        → product & customer models
  - `src-start/SearchEntities`      → AI search DTOs
  - `src-start/VectorEntities`      → embeddings models
- Features already in place:
  - Blazor Store with product browsing, cart, checkout.
  - AI-powered semantic search using Azure OpenAI / Foundry and embeddings.
  - SQL Server persistence with EF Core.
  - Customer profiles with membership tiers (Gold/Silver/Regular) mentioned in README.

Do **not** break:
- Existing Aspire orchestration.
- Existing AI semantic search endpoints.
- Existing tests, if possible.

---

## High-Level Goals

1. **Demo-ready flow:**
   - Baseline: eShop Lite with AI search (already working).
   - Step 1 Demo: Add a **Discount Agent** that applies membership-based discounts during checkout.
   - Step 2 Demo: Add a **multi-agent workflow**:
     - StockAgent → validates availability.
     - DiscountAgent → applies tier-based discount.
     - CartAgent → computes final totals and an explanation.

2. **Agent Framework integration:**
   - Use **Microsoft Agent Framework** for the DiscountAgent / workflow logic.
   - Use **Azure OpenAI / Azure AI Foundry** as the underlying model.
   - Keep it **configurable**:
     - Must run **locally** with the least amount of configuration (re-use existing AI connection settings where possible).
     - Optionally enable **Foundry persistent agents / workflows** via environment variables.

3. **User-visible UI for the demo:**
   - Simple way to **select current membership tier** (Guest / Silver / Gold) in the Store UI.
   - Cart/Checkout page must display:
     - Subtotal.
     - Discount amount.
     - Discount reason (short explanation).
     - Final total.
     - Agent steps log (Stock check, Discount applied, etc.).

4. **Observability for live demo:**
   - Log agent steps clearly (for the console and Aspire dashboard).
   - Make it easy, in code, to show “this is where the agents run” during the talk (comments / regions).

5. **Documentation:**
   - Update `README.md` with a short “Agentic Demo” section:
     - What the agents do.
     - How to configure and run the demo.
     - Optional Foundry configuration.

---

## Constraints & Style

- Keep everything compatible with **.NET 9** and existing project style.
- Minimize breaking changes.
- Prefer **clear, demo-friendly code**, even if slightly more verbose.
- Use descriptive comments like `// DEMO: Discount Agent integration` around critical demo points.
- Do **not** introduce unnecessary new external dependencies beyond:
  - `Azure.AI.OpenAI`
  - `Azure.Identity`
  - `Microsoft.Agents.AI.OpenAI`
  - `Azure.AI.Agents.Persistent`
  - `Microsoft.Agents.AI.AzureAI`
  - `Microsoft.Agents.AI.Workflows`
  - plus OpenTelemetry packages if needed.

If some of these packages are already referenced, reuse them rather than duplicating.

---

## Implementation Tasks (Step-by-Step)

### 1. Analyze Current Cart, Customer, and Store

1. Inspect:
   - `src-start/CartEntities` (cart & order models).
   - `src-start/DataEntities` (customer & membership tier definitions).
   - `src-start/Store`:
     - Components under `Components/Pages`, `Components/Cart`.
     - Services under `Services`.

2. Identify:
   - Where the **current cart** is stored/manipulated (service + Razor components).
   - Where **checkout** is performed (e.g., service method or API call).
   - Where, if anywhere, **membership tier** is currently used.

3. **Do not** remove existing functionality.
   - If there is already a cart implementation, we extend it.
   - If membership tiers already exist, we reuse the existing enum/model instead of creating a new one.

---

### 2. Ensure Membership Tier Support for the Demo

Goal: Have a **simple demo-only “current customer context”** with membership tiers accessible to the Store UI and to agents.

1. If not already present, create or finalize:
   - A `MembershipTier` enum in `DataEntities` or a suitable shared project with values:
     - `Regular` (or `Standard`)
     - `Silver`
     - `Gold`

2. In `src-start/Store/Services`, create a `CurrentCustomerContext` service (or extend an existing Customer service) that:
   - Exposes a `MembershipTier CurrentTier` property.
   - Allows setting the tier at runtime (e.g., `SetTier(MembershipTier tier)`).
   - Optionally models a simple “CurrentCustomerId” if needed, but **no full auth** is required for the demo.

3. Register this service as a **Scoped** service in the Store startup (Program/DI):
   - Ensure it’s available to Razor components.

4. In the Store UI:
   - Add a visible UI element (e.g., a component in the page layout or cart page) that:
     - Lets the user pick tier: `Guest/Regular`, `Silver`, `Gold`.
     - Shows a label, e.g.: `Current membership tier: Gold`.
   - This selector should update `CurrentCustomerContext.CurrentTier`.

5. Make sure cart & checkout logic can access `CurrentCustomerContext.CurrentTier`.

---

### 3. Extend Cart Models for Discounts

1. In the cart entity models (`src-start/CartEntities` or equivalent):
   - Add fields for:
     - `decimal Subtotal`
     - `decimal DiscountAmount`
     - `string? DiscountReason`
     - `decimal TotalAfterDiscount`
   - If the existing model already exposes a `Total` or `GrandTotal`, rework it so that:
     - `Subtotal` = sum of price * quantity before discounts.
     - `TotalAfterDiscount` = `Subtotal - DiscountAmount`.
     - Keep backwards compatibility where possible by mapping `Total` → `TotalAfterDiscount` or similar.

2. Update any cart calculation service in `Store` to:
   - Compute `Subtotal` deterministically.
   - Allow external logic (our upcoming agent orchestrator) to set `DiscountAmount` and `DiscountReason`.

3. Update cart/checkout Razor components to **display**:
   - Subtotal.
   - DiscountAmount (line item, only if > 0).
   - DiscountReason (short text, e.g., “Gold member 20% discount applied”).
   - Final total.

   Use visually clear labels suitable for a live demo.

---

### 4. Create an Agents Library for Discount & Workflow Logic

Create a new project under `src-start`:

- Name suggestion: `src-start/AgentServices` (a C# class library).

1. Add this project to the solution and reference it from:
   - `Store`
   - Optionally `Products` (if you need DB access for stock checking; see notes below).

2. In `AgentServices`, add NuGet references (via project file):
   - `Azure.AI.OpenAI` (preview or latest stable that works with Agent Framework).
   - `Azure.Identity`
   - `Microsoft.Agents.AI.OpenAI`
   - `Azure.AI.Agents.Persistent`
   - `Microsoft.Agents.AI.AzureAI`
   - `Microsoft.Agents.AI.Workflows`

3. Introduce configuration options:
   - A class like `AgentSettings` (e.g., in `AgentServices/Configuration/AgentSettings.cs`) with:
     - `bool UseFoundryAgents` (default: `false` for local simple mode).
     - Azure OpenAI / Foundry parameters, such as:
       - `string? AzureOpenAIEndpoint`
       - `string? AzureOpenAIModel`
       - `string? FoundryProjectEndpoint`
       - `string? FoundryModelId`
   - Bind this from configuration (appsettings / environment variables) using existing patterns in the repo.
   - Reuse existing connection string/secret (e.g., `ConnectionStrings:microsoftfoundry`) if this repo already uses one for AI search; avoid duplicating secrets.

---

### 5. Implement a Local DiscountAgent (Simple Mode)

In `AgentServices`, create a class to encapsulate the discount logic:

- Example: `DiscountAgentService` in `AgentServices/Discount/DiscountAgentService.cs`.

Responsibilities:

1. Accept a request model, e.g. `DiscountRequest`:
   - `MembershipTier Tier`
   - `IReadOnlyList<CartItem>` (simple DTO with product name, quantity, unit price, etc.)
   - `decimal Subtotal`

2. Produce a response model, e.g. `DiscountResult`:
   - `decimal DiscountAmount`
   - `string DiscountReason`
   - `decimal TotalAfterDiscount`

3. Use **Microsoft Agent Framework** with **Azure.AI.OpenAI** (local/simple mode) to implement the discount logic:
   - Create an `OpenAIClient` / ChatClient using configuration from `AgentSettings`.
   - Create an **AI Agent** using `Microsoft.Agents.AI.OpenAI` that:
     - Has a clear system prompt, e.g.:

       > "You are an e-commerce pricing assistant.  
       > Rules:  
       > - If the customer is GOLD, apply 20% discount to the subtotal.  
       > - If SILVER, apply 10% discount to the subtotal.  
       > - For REGULAR or GUEST, no discount.  
       > - Never apply negative discounts.  
       > Respond strictly with JSON: { \"discountAmount\": <number>, \"reason\": \"string\" }."

   - Send a message with:
     - Tier name.
     - Subtotal.
     - A short list of cart items (names and prices) for context.

4. Parse the JSON response into `DiscountResult`:
   - Validate numbers and clamp nonsensical values (e.g., discount cannot be > Subtotal).

5. Add defensive behavior so that, if the AI call fails or configuration is missing:
   - DiscountAmount = 0.
   - Reason = "No discount applied (agent unavailable)".

Add clear comments starting with `// DEMO: Discount Agent` around this code.

---

### 6. Implement a StockAgent and a Simple Orchestrator (Local Mode)

Create a simple multi-step orchestration for checkout.

1. Implement a `StockAgentService` (could live in `AgentServices/Stock/StockAgentService.cs`):
   - Responsibility: check stock levels and produce a human-friendly summary.
   - **Note:** For simplicity & reliability, this agent can be mostly deterministic:
     - Allow it to query real product stock data by:
       - Either:
         - Injecting a product repository that internally uses `Products` DB models.
         - Or using a dedicated API call to the `Products` service.
     - Use the LLM **only** to produce a user-friendly explanation string:
       - E.g., "All items are available" or "Item X reduced to 1 due to limited stock."

   - `StockCheckResult` should include:
     - `bool HasStockIssues`
     - Optionally, adjusted quantities or list of items with problems.
     - `string SummaryMessage` (LLM-generated).

2. Implement an `AgentCheckoutOrchestrator` (e.g. `AgentServices/Checkout/AgentCheckoutOrchestrator.cs`):
   - Input:
     - Cart + Subtotal.
     - Membership tier.
   - Steps:
     1. Call `StockAgentService` to validate stock.
     2. Call `DiscountAgentService` to compute discount.
     3. Return a combined `AgentCheckoutResult` containing:
        - Updated `Subtotal`, `DiscountAmount`, `TotalAfterDiscount`.
        - A list of "agent steps" with:
          - `Name` (e.g., "StockAgent", "DiscountAgent").
          - `Status` (Success/Warning/Error).
          - `Message` (short explanation).

3. Ensure this orchestrator works completely in **local mode** without requiring Foundry, as long as Azure OpenAI / Foundry connection string is set for the model used.

---

### 7. Add Optional Azure AI Foundry Workflow Integration (Advanced Mode)

To support an advanced story in the talk, add **optional** integration with Azure AI Foundry agent workflows using:

- `Azure.AI.Agents.Persistent`
- `Microsoft.Agents.AI.AzureAI`
- `Microsoft.Agents.AI.Workflows`

Implementation guidelines:

1. Create a `FoundryWorkflowOrchestrator` that:
   - Is enabled only if `AgentSettings.UseFoundryAgents == true`.
   - Uses `PersistentAgentsClient` + `Microsoft.Agents.AI.Workflows` APIs to:
     - Create or reuse a simple workflow where:
       - A "Stock" agent runs first.
       - A "Discount" agent runs second.
       - Optionally, a "CartSummary" agent runs to generate a final explanation.

2. The **public API** of `FoundryWorkflowOrchestrator` must be the same as `AgentCheckoutOrchestrator`, but the internal implementation uses Foundry workflows instead of purely local logic.

3. The main checkout pipeline should:
   - If `UseFoundryAgents` is true and configuration is valid:
     - Call the Foundry workflow version.
   - Else:
     - Use the local `AgentCheckoutOrchestrator`.

4. Add comments explaining that this path is **optional** and ideal for showing the “cloud-scale agent” story live if Foundry is configured.

---

### 8. Wire Agents into the Store Checkout Flow

In the `Store` project:

1. Locate the **checkout** flow:
   - Where the user confirms the cart.
   - Where totals are computed.

2. Replace the direct calculation of final totals with calls to the orchestrator:
   - Resolve `AgentCheckoutOrchestrator` (or a façade that chooses between local / Foundry) via DI.
   - Pass:
     - Current cart items.
     - Subtotal.
     - `CurrentCustomerContext.CurrentTier`.

3. Update the cart/checkout Razor page(s) to show:
   - `Subtotal`
   - `DiscountAmount`
   - `DiscountReason`
   - `TotalAfterDiscount`
   - A visual section for **Agent steps**, e.g.:

     ```
     Agent Steps:
     - StockAgent: OK — All items in stock
     - DiscountAgent: Applied 20% Gold discount
     ```

4. Ensure the page still works if the agent system is unavailable:
   - Fall back to original totals.
   - Display a user-friendly message such as:
     - "Checkout running in standard mode (no intelligent discounts)."

5. Add prominent comments in the checkout handler and in the Razor page:
   - Example: `@* DEMO: Agentic checkout pipeline starts here *@`.

---

### 9. Observability & Logging for the Demo

1. Wherever agents and orchestrators run:
   - Log key events through the existing logging infrastructure:
     - Start and end of each agent call.
     - Important intermediate messages (e.g., tier detected, discount applied, stock issues).
   - Structure logs so they’re easy to filter in the Aspire dashboard.

2. If needed, add minimal OpenTelemetry support:
   - Optionally add `OpenTelemetry` + `OpenTelemetry.Exporter.Console` and integrate traces/spans around the agent calls.
   - Make this consistent with any existing Aspire telemetry configuration.

3. Ensure that:
   - It is easy to **show** in a live demo where the agents run by tailing the logs.

---

### 10. Update README.md for the Agentic Demo

At the bottom of `README.md`, add a new section:

- Suggested title: `## 🧠 Agentic Checkout Demo (Microsoft Agent Framework)`

Content should include:

1. A short explanation:
   - Agents used:
     - StockAgent.
     - DiscountAgent.
     - Optional Foundry workflow.
   - What they do in the demo.

2. A quick start for the demo:
   - Assume app already runs as per existing README.
   - Additional steps:
     - Configure Azure OpenAI / Foundry environment for agent calls.
     - Environment variables and/or user secrets to set:
       - `AZURE_OPENAI_ENDPOINT`
       - `AZURE_OPENAI_MODEL` or reuse existing model config.
       - Or Foundry-specific:
         - `AZURE_FOUNDRY_PROJECT_ENDPOINT`
         - `AZURE_FOUNDRY_PROJECT_MODEL_ID`
       - `USE_FOUNDRY_AGENTS=true` (if used as a flag).

   - Demo flow:
     - Start Aspire (`eShopAppHost`).
     - Open Store front-end.
     - Select `Gold` tier from the new UI selector.
     - Add 2–3 products to the cart.
     - Go to checkout and show:
       - Agent steps.
       - Discount applied.
       - Differences for `Regular` vs `Silver` vs `Gold`.

3. Mention that:
   - If agent configuration is missing, checkout still works but without AI-based discounts, and the UI will reflect that.

---

## Acceptance Criteria

The repo is considered **ready for the 30-minute demo** when:

1. The solution builds and runs with `dotnet run` from `src-start/eShopAppHost` (or `src-complete/eShopAppHost`) without errors.
2. From the Store UI, the presenter can:
   - Select membership tier (Regular/Silver/Gold).
   - Add items to cart.
   - Proceed to checkout and see:
     - Subtotal, DiscountAmount, DiscountReason, Final total.
     - Agent steps summary.
3. Discount logic is clearly driven by an Agent Framework integration, with code that can be shown live.
4. There is a clear path (flag + config) to switch from local agent mode to Foundry workflow mode.
5. README contains a concise “Agentic Checkout Demo” section with setup + demo steps.
6. Existing AI search features remain functional and unchanged in behavior.

Please implement all the steps above, updating code, configuration, and README as needed, while keeping the repository clean, compiling, and ready for a conference demo.
````

[1]: https://learn.microsoft.com/en-us/agent-framework/tutorials/quick-start?utm_source=chatgpt.com "Microsoft Agent Framework Quick-Start Guide"
[2]: https://learn.microsoft.com/en-us/agent-framework/tutorials/workflows/agents-in-workflows?utm_source=chatgpt.com "Agents in Workflows"
