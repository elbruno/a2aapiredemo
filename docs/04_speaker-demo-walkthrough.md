# Speaker Demo Walkthrough — Code Implementation Guide

### Step-by-Step Instructions for Live Coding During the Session
### From `/src-01-start` (Baseline) to Fully Agentic Solution

---

## 📋 Overview

This document provides **detailed, step-by-step instructions** for the speaker to follow during the live demo session. The demo is structured with pre-built components to make the live coding simpler and more reliable.

### Demo Structure

| Step | Folder | What You'll Do |
|------|--------|----------------|
| 0 | `/src-01-start` | Show baseline app (no coding) |
| 1 | `/src-01-start` | Live code: Replace `ComputeDiscountAsync` only (helpers pre-built) |
| 2 | `/src-02-multiagent` | Open and explain pre-built multi-agent workflow |
| 3 | `/src-03-dependency-injection` | Open and explain DI registration |
| 4 | `/src-04-devui` | Open and explain Agent Framework registration + DevUI |

### Prerequisites

Before starting the live demo:

1. ✅ Open `/src-01-start` in your IDE
2. ✅ Verify the baseline app builds and runs: `dotnet run` from `eShopAppHost`
3. ✅ Have Azure OpenAI / Microsoft Foundry connection configured
4. ✅ Have `/src-02-multiagent`, `/src-03-dependency-injection`, and `/src-04-devui` ready to open for quick transitions
5. ✅ Open `/src-05-complete` in a separate window for reference (if needed)

---

## 🎯 Demo 1: Implement the DiscountAgent (5 minutes)

### What's Already Pre-Built

The `/src-01-start` version now includes:
- ✅ System prompt with discount rules
- ✅ `ParseAgentResponse` helper method  
- ✅ `ComputeFallbackDiscount` helper method
- ✅ All required using statements

This allows you to focus on **just the AI integration code** during the live demo.

### File to Edit

```
src-01-start/AgentServices/Discount/DiscountAgentService.cs
```

### Current State (Placeholder)

The file has the placeholder method you'll replace:

```csharp
public Task<DiscountResult> ComputeDiscountAsync(DiscountRequest request)
{
    _logger.LogInformation("TODO: DiscountAgent not implemented...");
    
    // Placeholder: No discount applied
    // DEMO: Replace this with AI-powered discount calculation
    var result = new DiscountResult
    {
        DiscountAmount = 0,
        DiscountReason = "No discount applied - Agent not implemented",
        TotalAfterDiscount = request.Subtotal,
        Success = true
    };
    return Task.FromResult(result);
}
```

### Live Coding: Replace the ComputeDiscountAsync Method

Replace the entire `ComputeDiscountAsync` method with:

```csharp
/// <summary>
/// DEMO: Compute discount using Microsoft Agent Framework's AIAgent.
/// </summary>
public async Task<DiscountResult> ComputeDiscountAsync(DiscountRequest request)
{
    _logger.LogInformation("DEMO: {AgentName} starting - Tier: {Tier}, Subtotal: {Subtotal:C}", 
        AgentName, request.Tier, request.Subtotal);

    // If chat client is not available, use fallback deterministic logic
    if (_chatClient == null || !_settings.AgentsEnabled)
    {
        _logger.LogWarning("DEMO: AI not available, using fallback discount logic");
        return ComputeFallbackDiscount(request);
    }

    try
    {
        // DEMO: Create an AIAgent using Microsoft Agent Framework
        // The agent encapsulates the system prompt (instructions) and agent identity
        var discountAgent = _chatClient.CreateAIAgent(
            instructions: AgentInstructions,
            name: AgentName);

        // DEMO: Build the user message with cart context
        var itemsSummary = string.Join(", ", request.Items.Select(i => $"{i.Name} (${i.Price:F2} x {i.Quantity})"));
        var userMessage = $"""
            Customer membership tier: {request.Tier}
            Cart subtotal: ${request.Subtotal:F2}
            Items: {itemsSummary}
            
            Calculate the discount based on the membership tier rules.
            """;

        _logger.LogInformation("DEMO: Sending request to {AgentName} via Agent Framework", AgentName);
        
        // DEMO: Use the Agent Framework's RunAsync method
        // This is the key change from direct ChatClient calls - the agent handles 
        // conversation context, system prompts, and message formatting internally
        var response = await discountAgent.RunAsync(userMessage);
        var content = response.Text ?? "";

        _logger.LogInformation("DEMO: {AgentName} response: {Response}", AgentName, content);

        // DEMO: Parse the JSON response
        var result = ParseAgentResponse(content, request.Subtotal);
        _logger.LogInformation("DEMO: {AgentName} computed - Amount: {Amount:C}, Reason: {Reason}", 
            AgentName, result.DiscountAmount, result.DiscountReason);

        return result;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "DEMO: {AgentName} error, using fallback", AgentName);
        return ComputeFallbackDiscount(request);
    }
}
```

### Expected Outcome After Demo 1

- The "Apply AI Discount" button in the cart now calculates discounts
- Gold members see 20% discount applied
- Silver members see 10% discount applied
- Normal members see no discount
- The DiscountReason shows the AI-generated explanation

### 💬 Key Messages to Say

> "Notice we're using Microsoft Agent Framework's `CreateAIAgent` and `RunAsync` pattern - this is the modern way to build agentic applications."

> "We didn't write if-else logic. We only described intent — the AIAgent produced the business rule."

> "Notice the fallback logic — if AI is unavailable, the app still works with deterministic rules."

---

## 🎯 Demo 2: Show Multi-Agent Workflow (Pre-Built)

### What to Do

Instead of live coding, **open the `/src-02-multiagent` folder** and walk through the pre-built implementation.

### Files to Show

**StockAgentService** (`src-02-multiagent/AgentServices/Stock/StockAgentService.cs`):
```csharp
// DEMO: Agent instructions for stock agent message generation
private const string AgentInstructions = """
    You are a friendly e-commerce stock checker assistant.
    Given a list of items and their stock status, generate a brief, friendly summary message.
    Be concise and positive. If all items are available, say something like "Great news! All items are in stock and ready to ship."
    If there are issues, mention them briefly.
    Respond with just the summary message, no JSON or formatting.
    """;

// Create agent with Microsoft Agent Framework
var stockAgent = _chatClient.CreateAIAgent(
    instructions: AgentInstructions,
    name: AgentName);

// Run the agent
var response = await stockAgent.RunAsync(userMessage);
var content = response.Text;
```

**AgentCheckoutOrchestrator** (`src-02-multiagent/AgentServices/Checkout/AgentCheckoutOrchestrator.cs`):
```csharp
// DEMO: Execute the multi-agent checkout workflow
public async Task<AgentCheckoutResult> ProcessCheckoutAsync(AgentCheckoutRequest request)
{
    // Step 1 - Stock Agent
    var stockStep = await RunStockAgent(request);
    result.AgentSteps.Add(stockStep);

    // Step 2 - Discount Agent
    var discountStep = await RunDiscountAgent(request, result);
    result.AgentSteps.Add(discountStep);
    
    return result;
}
```

### Expected Outcome After Demo 2

- Agent steps are displayed in the cart summary:
  - ✅ StockAgent: "Great news! All items are in stock..."
  - ✅ DiscountAgent: "Gold member 20% discount applied"
- The discount is calculated and applied to the total
- The multi-agent workflow completes successfully

### 💬 Key Messages to Say

> "Each agent does one job. This architecture is maintainable and easily replaced."

> "The orchestrator coordinates the workflow — Stock first, then Discount."

> "The StockAgent uses AI for user-friendly messages, not for stock validation."

---

## 🎯 Demo 3: DI Registration + Observability (4 minutes)

### What to Do

Open the `/src-03-dependency-injection` folder and show the advanced patterns for production-ready agent applications.

### Key Features to Highlight

**1. Enhanced DI Registration** (`src-03-dependency-injection/AgentServices/AgentServicesExtensions.cs`):

```csharp
// DEMO Step 3: Register agent services with scoped lifetime
// Scoped lifetime ensures:
// - Each HTTP request gets its own agent instance
// - Agent state is isolated between requests
// - Proper disposal of resources
services.AddScoped<IStockAgentService, StockAgentService>();
services.AddScoped<IDiscountAgentService, DiscountAgentService>();
services.AddScoped<IAgentCheckoutOrchestrator, AgentCheckoutOrchestrator>();
```

**2. Enhanced Logging** (`src-03-dependency-injection/Store/Program.cs`):

```csharp
// DEMO Step 3: Add enhanced logging for agent debugging in development
if (builder.Environment.IsDevelopment())
{
    builder.Logging.SetMinimumLevel(LogLevel.Debug);
}

// ...

app.Logger.LogInformation("DEMO Step 3: Agent services registered via DI with scoped lifetime");
```

### Benefits to Demonstrate

- **Proper Lifecycle**: Scoped services ensure request isolation
- **Testability**: Easy to mock agents in unit tests
- **Configuration**: Centralized settings via `AgentSettings`
- **Observability**: OpenTelemetry integration for tracing

### 💬 Key Messages to Say

> "Registering agents in DI gives you proper lifecycle management and testability."

> "Scoped lifetime means each request gets its own agent instances."

> "This pattern is essential for production-grade agent applications."

---

## 🎯 Demo 4: DevUI for Agent Debugging (3 minutes)

### What to Do

Open the `/src-04-devui` folder and show the advanced Agent Framework registration approach with DevUI integration.

### Key Features to Highlight

**1. DevUI Package Reference** (`src-04-devui/Store/Store.csproj`):

```xml
<!-- DEMO Step 4: DevUI package for agent debugging and visualization -->
<PackageReference Include="Microsoft.Agents.AI.DevUI" Version="1.0.0-preview.251125.1" />
```

**2. DevUI Registration** (`src-04-devui/Store/Program.cs`):

```csharp
// DEMO Step 4: Add DevUI services for agent debugging in development
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDevUI();
    builder.Logging.SetMinimumLevel(LogLevel.Debug);
}

// ...

// DEMO Step 4: Map DevUI endpoint for agent debugging in development
if (app.Environment.IsDevelopment())
{
    app.MapDevUI();
    app.Logger.LogInformation("DEMO Step 4: DevUI endpoint mapped at /devui for agent debugging");
}
```

**3. Agent Framework Registration Pattern** (`src-04-devui/AgentServices/AgentServicesExtensions.cs`):

```csharp
// DEMO Step 4: Register agent services with scoped lifetime
// This follows the Agent Framework pattern for agent registration
services.AddScoped<IStockAgentService, StockAgentService>();
services.AddScoped<IDiscountAgentService, DiscountAgentService>();
services.AddScoped<IAgentCheckoutOrchestrator, AgentCheckoutOrchestrator>();
```

### Benefits to Demonstrate

- **Visual Debugging**: DevUI provides a visual interface to inspect agent behavior
- **Message Flows**: See the actual messages exchanged between agents and LLMs
- **Interactive Testing**: Test agent responses directly from the DevUI interface
- **Development Only**: DevUI is only enabled in Development environment for security

### 💬 Key Messages to Say

> "DevUI gives you a visual interface to debug and inspect your agents."

> "Access DevUI at `/devui` when running in Development mode."

> "This pattern follows the official Agent Framework samples."

---

## 🔍 Summary: Folder Structure

| Folder | Contents |
|--------|----------|
| `/src-01-start` | Baseline with pre-built system prompts and helpers for easy live coding |
| `/src-02-multiagent` | Complete StockAgent + AgentCheckoutOrchestrator implementation |
| `/src-03-dependency-injection` | Full solution with DI registration and observability patterns |
| `/src-04-devui` | Agent Framework registration approach with DevUI packages |
| `/src-05-complete` | Reference implementation with all features |

---

## 📊 Complete Code Reference

For the complete implementation of each file, refer to:

- `/src-02-multiagent` for multi-agent workflow
- `/src-03-dependency-injection` for DI + observability patterns
- `/src-04-devui` for Agent Framework approach with DevUI
- `/src-05-complete` for full reference implementation

---

## ⏱️ Timing Guide

| Demo Step | Duration | What You Do |
|-----------|----------|-------------|
| Demo 0: Baseline | 3 min | Show app running (no coding) |
| Demo 1: DiscountAgent | 5 min | Live code ComputeDiscountAsync only |
| Demo 2: Multi-Agent | 4 min | Open `/src-02-multiagent`, walk through code |
| Demo 3: DI + Observability | 4 min | Open `/src-03-dependency-injection`, show patterns |
| Demo 4: DevUI | 3 min | Open `/src-04-devui`, show DevUI debugging |
| **Total Coding Time** | **5 min** | Only Demo 1 requires live coding |

> **Tip**: Practice the demo several times to ensure smooth transitions between folders.

---

## 🚨 Troubleshooting

### "AI not available" message

- Check that the Azure OpenAI connection string is configured
- Verify `AgentSettings.AgentsEnabled` is `true`
- Check the Aspire dashboard for connection errors

### Discount not being applied

- Ensure you selected a customer (Alice for Gold, Bob for Silver)
- Check the browser console for JavaScript errors
- Verify the cart has items before clicking "Apply AI Discount"

### Build errors after changes

- Ensure all `using` statements are added
- Check for missing closing braces `}`
- Verify method signatures match the interfaces

### DevUI not accessible

- Ensure you're running in Development environment
- Check that the DevUI packages are installed in `/src-03-dependency-injection`
- Verify the endpoint is mapped: `app.MapDevUI()`

---

## 🏁 End of Walkthrough

After completing all demos, the audience will have seen:

✅ AI-powered membership discounts (live coded)  
✅ Friendly stock status messages (pre-built)  
✅ Multi-agent checkout workflow (pre-built)  
✅ DI registration patterns (pre-built)  
✅ DevUI for agent debugging (pre-built)  

**Next**: Return to the session to explain optional Azure AI Foundry integration and wrap up with key lessons.
