# Speaker Demo Walkthrough — Code Implementation Guide

### Step-by-Step Instructions for Live Coding During the Session
### From `/src-start` (Baseline) to `/src-complete` (Agentic Solution)

---

## 📋 Overview

This document provides **detailed, step-by-step instructions** for the speaker to follow during the live demo session. It covers the exact code changes needed to transform the baseline eShop Lite application (`/src-start`) into the fully agentic solution (`/src-complete`).

### What You'll Implement

| Step | Component | What You'll Add |
|------|-----------|-----------------|
| 1 | DiscountAgentService | AI-powered membership discount calculation |
| 2 | StockAgentService | AI-generated friendly stock status messages |
| 3 | AgentCheckoutOrchestrator | Multi-agent checkout workflow |

### Prerequisites

Before starting the live demo:

1. ✅ Open `/src-start` in your IDE
2. ✅ Verify the baseline app builds and runs: `dotnet run` from `eShopAppHost`
3. ✅ Have Azure OpenAI / Microsoft Foundry connection configured
4. ✅ Open `/src-complete` in a separate window for reference (if needed)

---

## 🎯 Demo 1: Implement the DiscountAgent (5 minutes)

### File to Edit

```
src-start/AgentServices/Discount/DiscountAgentService.cs
```

### Current State (TODO Stub)

The file currently contains a placeholder that returns no discount:

```csharp
public Task<DiscountResult> ComputeDiscountAsync(DiscountRequest request)
{
    _logger.LogInformation("TODO: DiscountAgent not implemented...");
    
    // Placeholder: No discount applied
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

### Step 1.1: Add the System Prompt

Add the following constant at the top of the class (after the field declarations):

```csharp
// DEMO: System prompt for the discount agent
private const string SystemPrompt = """
    You are an e-commerce pricing assistant.
    
    Rules:
    - If the customer is GOLD, apply 20% discount to the subtotal.
    - If SILVER, apply 10% discount to the subtotal.
    - For NORMAL (Regular) or any other tier, no discount.
    - Never apply negative discounts.
    - Calculate the discount amount based on the subtotal provided.
    
    Respond strictly with JSON in this format:
    { "discountAmount": <number>, "reason": "<string>" }
    
    Example for GOLD with $100 subtotal:
    { "discountAmount": 20.00, "reason": "Gold member 20% discount applied" }
    
    Example for NORMAL with $100 subtotal:
    { "discountAmount": 0, "reason": "No discount - Standard membership" }
    """;
```

### Step 1.2: Replace the ComputeDiscountAsync Method

Replace the entire `ComputeDiscountAsync` method with:

```csharp
/// <summary>
/// DEMO: Compute discount using the AI agent.
/// </summary>
public async Task<DiscountResult> ComputeDiscountAsync(DiscountRequest request)
{
    _logger.LogInformation("DEMO: DiscountAgent starting - Tier: {Tier}, Subtotal: {Subtotal:C}", 
        request.Tier, request.Subtotal);

    // If chat client is not available, use fallback deterministic logic
    if (_chatClient == null || !_settings.AgentsEnabled)
    {
        _logger.LogWarning("DEMO: AI not available, using fallback discount logic");
        return ComputeFallbackDiscount(request);
    }

    try
    {
        // DEMO: Build the user message with cart context
        var itemsSummary = string.Join(", ", request.Items.Select(i => $"{i.Name} (${i.Price:F2} x {i.Quantity})"));
        var userMessage = $"""
            Customer membership tier: {request.Tier}
            Cart subtotal: ${request.Subtotal:F2}
            Items: {itemsSummary}
            
            Calculate the discount based on the membership tier rules.
            """;

        // DEMO: Call the AI agent
        var messages = new List<ChatMessage>
        {
            new(ChatRole.System, SystemPrompt),
            new(ChatRole.User, userMessage)
        };

        _logger.LogInformation("DEMO: Sending request to DiscountAgent AI");
        var response = await _chatClient.GetResponseAsync(messages);
        var content = response.Text ?? "";

        _logger.LogInformation("DEMO: DiscountAgent AI response: {Response}", content);

        // DEMO: Parse the JSON response
        var result = ParseAgentResponse(content, request.Subtotal);
        _logger.LogInformation("DEMO: DiscountAgent computed - Amount: {Amount:C}, Reason: {Reason}", 
            result.DiscountAmount, result.DiscountReason);

        return result;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "DEMO: DiscountAgent error, using fallback");
        return ComputeFallbackDiscount(request);
    }
}
```

### Step 1.3: Add Helper Methods

Add these helper methods at the end of the class:

```csharp
private DiscountResult ParseAgentResponse(string content, decimal subtotal)
{
    try
    {
        // Extract JSON from response (handle markdown code blocks if present)
        var jsonContent = content.Trim();
        if (jsonContent.Contains("```json"))
        {
            var start = jsonContent.IndexOf("```json") + 7;
            var end = jsonContent.IndexOf("```", start);
            jsonContent = jsonContent.Substring(start, end - start).Trim();
        }
        else if (jsonContent.Contains("```"))
        {
            var start = jsonContent.IndexOf("```") + 3;
            var end = jsonContent.IndexOf("```", start);
            jsonContent = jsonContent.Substring(start, end - start).Trim();
        }

        using var doc = JsonDocument.Parse(jsonContent);
        var root = doc.RootElement;

        var discountAmount = root.GetProperty("discountAmount").GetDecimal();
        var reason = root.GetProperty("reason").GetString() ?? "Discount applied";

        // Validate and clamp discount
        if (discountAmount < 0) discountAmount = 0;
        if (discountAmount > subtotal) discountAmount = subtotal;

        return new DiscountResult
        {
            DiscountAmount = discountAmount,
            DiscountReason = reason,
            TotalAfterDiscount = subtotal - discountAmount,
            Success = true
        };
    }
    catch (Exception ex)
    {
        _logger.LogWarning(ex, "DEMO: Failed to parse agent response, extracting manually");
        
        // Simple fallback parsing
        return new DiscountResult
        {
            DiscountAmount = 0,
            DiscountReason = "Could not parse agent response",
            TotalAfterDiscount = subtotal,
            Success = false
        };
    }
}

/// <summary>
/// DEMO: Deterministic fallback when AI is unavailable.
/// </summary>
private DiscountResult ComputeFallbackDiscount(DiscountRequest request)
{
    var (percentage, reason) = request.Tier switch
    {
        MembershipTier.Gold => (0.20m, "Gold member 20% discount applied"),
        MembershipTier.Silver => (0.10m, "Silver member 10% discount applied"),
        _ => (0m, "No discount - Standard membership")
    };

    var discountAmount = request.Subtotal * percentage;

    return new DiscountResult
    {
        DiscountAmount = discountAmount,
        DiscountReason = reason,
        TotalAfterDiscount = request.Subtotal - discountAmount,
        Success = true
    };
}
```

### Step 1.4: Add Required Using Statements

Ensure these using statements are at the top of the file:

```csharp
using System.Text.Json;
using AgentServices.Configuration;
using AgentServices.Models;
using DataEntities;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
```

### Expected Outcome After Demo 1

- The "Apply AI Discount" button in the cart now calculates discounts
- Gold members see 20% discount applied
- Silver members see 10% discount applied
- Normal members see no discount
- The DiscountReason shows the AI-generated explanation

### 💬 Key Messages to Say

> "We didn't write if-else logic. We only described intent — the agent produced the business rule."

> "Notice the fallback logic — if AI is unavailable, the app still works with deterministic rules."

---

## 🎯 Demo 2: Implement the StockAgent (3 minutes)

### File to Edit

```
src-start/AgentServices/Stock/StockAgentService.cs
```

### Current State (TODO Stub)

The file returns a static message without AI:

```csharp
public Task<StockCheckResult> CheckStockAsync(StockCheckRequest request)
{
    _logger.LogInformation("TODO: StockAgent not implemented...");
    
    var result = new StockCheckResult
    {
        HasStockIssues = false,
        Issues = new List<StockIssue>(),
        SummaryMessage = "Stock check completed - Agent not implemented",
        Success = true
    };
    return Task.FromResult(result);
}
```

### Step 2.1: Add the System Prompt

Add this constant at the top of the class:

```csharp
// DEMO: System prompt for stock agent message generation
private const string SystemPrompt = """
    You are a friendly e-commerce stock checker assistant.
    Given a list of items and their stock status, generate a brief, friendly summary message.
    Be concise and positive. If all items are available, say something like "Great news! All items are in stock and ready to ship."
    If there are issues, mention them briefly.
    Respond with just the summary message, no JSON or formatting.
    """;
```

### Step 2.2: Replace the CheckStockAsync Method

Replace the entire `CheckStockAsync` method with:

```csharp
/// <summary>
/// DEMO: Check stock availability for cart items.
/// For demo purposes, all items are considered in stock.
/// </summary>
public async Task<StockCheckResult> CheckStockAsync(StockCheckRequest request)
{
    _logger.LogInformation("DEMO: StockAgent starting - Checking {ItemCount} items", request.Items.Count);

    // DEMO: For demo purposes, all items are in stock
    // In a real scenario, this would query the Products API or database
    var result = new StockCheckResult
    {
        HasStockIssues = false,
        Issues = new List<StockIssue>(),
        Success = true
    };

    // Generate a friendly summary message
    result.SummaryMessage = await GenerateSummaryMessage(request, result);
    
    _logger.LogInformation("DEMO: StockAgent completed - HasIssues: {HasIssues}, Message: {Message}", 
        result.HasStockIssues, result.SummaryMessage);

    return result;
}
```

### Step 2.3: Add the GenerateSummaryMessage Method

Add this helper method:

```csharp
private async Task<string> GenerateSummaryMessage(StockCheckRequest request, StockCheckResult result)
{
    // If AI is not available, use fallback message
    if (_chatClient == null || !_settings.AgentsEnabled)
    {
        _logger.LogDebug("DEMO: AI not available, using fallback stock message");
        return result.HasStockIssues 
            ? "Some items have limited availability. Please review your cart."
            : "All items are in stock and ready to ship!";
    }

    try
    {
        var itemsList = string.Join("\n", request.Items.Select(i => $"- {i.Name} (Qty: {i.Quantity}): In Stock"));
        var userMessage = $"""
            Items to check:
            {itemsList}
            
            All items are available. Generate a brief, friendly confirmation message.
            """;

        var messages = new List<ChatMessage>
        {
            new(ChatRole.System, SystemPrompt),
            new(ChatRole.User, userMessage)
        };

        _logger.LogDebug("DEMO: Sending request to StockAgent AI for summary");
        var response = await _chatClient.GetResponseAsync(messages);
        var content = response.Text?.Trim() ?? "";

        if (!string.IsNullOrEmpty(content))
        {
            return content;
        }
    }
    catch (Exception ex)
    {
        _logger.LogWarning(ex, "DEMO: StockAgent AI error, using fallback message");
    }

    // Fallback
    return "All items are in stock and ready to ship!";
}
```

### Expected Outcome After Demo 2

- The StockAgent generates friendly, AI-powered confirmation messages
- Messages are personalized based on the cart contents
- Fallback to static message if AI is unavailable

### 💬 Key Messages to Say

> "The StockAgent uses AI for user-friendly messages, not for stock validation."

> "This pattern — deterministic logic + AI explanation — is great for production reliability."

---

## 🎯 Demo 3: Implement the AgentCheckoutOrchestrator (4 minutes)

### File to Edit

```
src-start/AgentServices/Checkout/AgentCheckoutOrchestrator.cs
```

### Current State (TODO Stub)

The orchestrator returns a basic result without running any agents:

```csharp
public Task<AgentCheckoutResult> ProcessCheckoutAsync(AgentCheckoutRequest request)
{
    _logger.LogInformation("TODO: Agent checkout orchestrator not implemented");
    
    var result = new AgentCheckoutResult
    {
        Subtotal = request.Cart.Subtotal,
        DiscountAmount = 0,
        DiscountReason = "Agent checkout not implemented",
        TotalAfterDiscount = request.Cart.Subtotal,
        AgentSteps = new List<AgentStep> { ... },
        Success = true
    };
    return Task.FromResult(result);
}
```

### Step 3.1: Replace the ProcessCheckoutAsync Method

Replace the entire `ProcessCheckoutAsync` method with:

```csharp
/// <summary>
/// DEMO: Execute the multi-agent checkout workflow.
/// </summary>
public async Task<AgentCheckoutResult> ProcessCheckoutAsync(AgentCheckoutRequest request)
{
    _logger.LogInformation("DEMO: Starting agentic checkout pipeline");
    _logger.LogInformation("DEMO: Membership tier: {Tier}, Cart subtotal: {Subtotal:C}", 
        request.MembershipTier, request.Cart.Subtotal);

    var result = new AgentCheckoutResult
    {
        Subtotal = request.Cart.Subtotal,
        AgentSteps = new List<AgentStep>()
    };

    try
    {
        // DEMO: Step 1 - Stock Agent
        _logger.LogInformation("DEMO: Step 1 - Running StockAgent");
        var stockStep = await RunStockAgent(request);
        result.AgentSteps.Add(stockStep);

        if (stockStep.Status == "Error")
        {
            result.Success = false;
            result.ErrorMessage = stockStep.Message;
            return result;
        }

        // DEMO: Step 2 - Discount Agent
        _logger.LogInformation("DEMO: Step 2 - Running DiscountAgent");
        var discountStep = await RunDiscountAgent(request, result);
        result.AgentSteps.Add(discountStep);

        // Finalize result
        result.Success = true;
        _logger.LogInformation("DEMO: Agentic checkout completed successfully");
        _logger.LogInformation("DEMO: Final - Subtotal: {Subtotal:C}, Discount: {Discount:C}, Total: {Total:C}",
            result.Subtotal, result.DiscountAmount, result.TotalAfterDiscount);

        return result;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "DEMO: Agentic checkout pipeline failed");
        
        result.AgentSteps.Add(new AgentStep
        {
            Name = "Orchestrator",
            Status = "Error",
            Message = $"Checkout failed: {ex.Message}",
            Timestamp = DateTime.UtcNow
        });

        // Fallback to no discount
        result.DiscountAmount = 0;
        result.DiscountReason = "Checkout running in standard mode (agent unavailable)";
        result.TotalAfterDiscount = result.Subtotal;
        result.Success = false;
        result.ErrorMessage = ex.Message;

        return result;
    }
}
```

### Step 3.2: Add the RunStockAgent Method

Add this helper method:

```csharp
private async Task<AgentStep> RunStockAgent(AgentCheckoutRequest request)
{
    var step = new AgentStep
    {
        Name = "StockAgent",
        Timestamp = DateTime.UtcNow
    };

    try
    {
        var stockRequest = new StockCheckRequest
        {
            Items = request.Cart.Items
        };

        var stockResult = await _stockAgent.CheckStockAsync(stockRequest);

        step.Status = stockResult.HasStockIssues ? "Warning" : "Success";
        step.Message = stockResult.SummaryMessage;

        _logger.LogInformation("DEMO: StockAgent completed - Status: {Status}", step.Status);
    }
    catch (Exception ex)
    {
        step.Status = "Error";
        step.Message = $"Stock check failed: {ex.Message}";
        _logger.LogError(ex, "DEMO: StockAgent failed");
    }

    return step;
}
```

### Step 3.3: Add the RunDiscountAgent Method

Add this helper method:

```csharp
private async Task<AgentStep> RunDiscountAgent(AgentCheckoutRequest request, AgentCheckoutResult result)
{
    var step = new AgentStep
    {
        Name = "DiscountAgent",
        Timestamp = DateTime.UtcNow
    };

    try
    {
        var discountRequest = new DiscountRequest
        {
            Tier = request.MembershipTier,
            Items = request.Cart.Items,
            Subtotal = result.Subtotal
        };

        var discountResult = await _discountAgent.ComputeDiscountAsync(discountRequest);

        result.DiscountAmount = discountResult.DiscountAmount;
        result.DiscountReason = discountResult.DiscountReason;
        result.TotalAfterDiscount = discountResult.TotalAfterDiscount;

        step.Status = discountResult.Success ? "Success" : "Warning";
        step.Message = discountResult.DiscountReason;

        _logger.LogInformation("DEMO: DiscountAgent completed - Discount: {Discount:C}", 
            discountResult.DiscountAmount);
    }
    catch (Exception ex)
    {
        step.Status = "Error";
        step.Message = $"Discount calculation failed: {ex.Message}";
        
        // Fallback
        result.DiscountAmount = 0;
        result.DiscountReason = "No discount applied (agent unavailable)";
        result.TotalAfterDiscount = result.Subtotal;

        _logger.LogError(ex, "DEMO: DiscountAgent failed");
    }

    return step;
}
```

### Expected Outcome After Demo 3

- Clicking "Apply AI Discount" runs the full agent pipeline
- Agent steps are displayed in the cart summary:
  - ✅ StockAgent: "Great news! All items are in stock..."
  - ✅ DiscountAgent: "Gold member 20% discount applied"
- The discount is calculated and applied to the total

### 💬 Key Messages to Say

> "Each agent does one job. This architecture is maintainable and easily replaced."

> "The orchestrator coordinates the workflow — Stock first, then Discount."

---

## 🔍 Summary: Files Changed

| File | Changes Made |
|------|--------------|
| `AgentServices/Discount/DiscountAgentService.cs` | Added AI-powered discount computation with system prompt |
| `AgentServices/Stock/StockAgentService.cs` | Added AI-powered friendly message generation |
| `AgentServices/Checkout/AgentCheckoutOrchestrator.cs` | Implemented multi-agent checkout workflow |

---

## 📊 Complete Code Reference

For the complete implementation of each file, refer to the `/src-complete` folder:

- `src-complete/AgentServices/Discount/DiscountAgentService.cs`
- `src-complete/AgentServices/Stock/StockAgentService.cs`
- `src-complete/AgentServices/Checkout/AgentCheckoutOrchestrator.cs`

---

## ⏱️ Timing Guide

| Demo Step | Duration | Cumulative |
|-----------|----------|------------|
| Demo 1: DiscountAgent | 5 min | 5 min |
| Demo 2: StockAgent | 3 min | 8 min |
| Demo 3: Orchestrator | 4 min | 12 min |
| **Total Coding Time** | **~12 min** | |

> **Tip**: Practice the demo several times to ensure smooth transitions between steps.

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

---

## 🏁 End of Walkthrough

After completing all three demos, the application should match the `/src-complete` implementation with:

✅ AI-powered membership discounts  
✅ Friendly stock status messages  
✅ Multi-agent checkout workflow  
✅ Agent steps visible in the UI  

**Next**: Return to the session to explain optional Azure AI Foundry integration and wrap up with key lessons.
