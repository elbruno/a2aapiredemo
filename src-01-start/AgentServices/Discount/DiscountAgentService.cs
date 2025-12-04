using System.Text.Json;
using AgentServices.Configuration;
using AgentServices.Models;
using DataEntities;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

namespace AgentServices.Discount;

/// <summary>
/// DEMO: Discount Agent Service
/// Uses Microsoft Agent Framework with Azure OpenAI to compute membership-based discounts.
/// 
/// This is the starting point for the live demo.
/// The agent instructions and helper methods are already defined to make the demo easier to follow.
/// During the demo, you will implement the ComputeDiscountAsync method using Microsoft Agent Framework's AIAgent.
/// See docs/04_speaker-demo-walkthrough.md for step-by-step instructions.
/// </summary>
public class DiscountAgentService : IDiscountAgentService
{
    private readonly IChatClient? _chatClient;
    private readonly ILogger<DiscountAgentService> _logger;
    private readonly AgentSettings _settings;

    // DEMO: Agent name for identification in logs and debugging
    private const string AgentName = "DiscountAgent";

    // DEMO: Agent instructions for the discount agent
    // This defines the discount rules that the AI agent will follow
    // Used with Microsoft Agent Framework's CreateAIAgent method
    private const string AgentInstructions = """
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

    public DiscountAgentService(
        IChatClient? chatClient,
        AgentSettings settings,
        ILogger<DiscountAgentService> logger)
    {
        _chatClient = chatClient;
        _settings = settings;
        _logger = logger;
    }

    /// <summary>
    /// TODO: Implement AI-powered discount calculation during the live demo using Microsoft Agent Framework.
    /// Replace this placeholder method with AIAgent-powered discount logic.
    /// 
    /// Example pattern using Agent Framework:
    /// 1. Create agent: var agent = _chatClient.CreateAIAgent(instructions: AgentInstructions, name: AgentName);
    /// 2. Run agent: var response = await agent.RunAsync(userMessage);
    /// 3. Parse response: var result = ParseAgentResponse(response.Text, subtotal);
    /// 
    /// See docs/04_speaker-demo-walkthrough.md#step-12-replace-the-computediscountasync-method
    /// </summary>
    public Task<DiscountResult> ComputeDiscountAsync(DiscountRequest request)
    {
        _logger.LogInformation("TODO: {AgentName} not implemented...", AgentName);
        
        // Placeholder: No discount applied
        // DEMO: Replace this with Microsoft Agent Framework AIAgent-powered discount calculation
        var result = new DiscountResult
        {
            DiscountAmount = 0,
            DiscountReason = "No discount applied - Agent not implemented",
            TotalAfterDiscount = request.Subtotal,
            Success = true
        };
        return Task.FromResult(result);
    }

    /// <summary>
    /// DEMO: Parse the AI agent's JSON response.
    /// This helper is already implemented to make the demo easier to follow.
    /// </summary>
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
                if (end > start)
                {
                    jsonContent = jsonContent.Substring(start, end - start).Trim();
                }
            }
            else if (jsonContent.Contains("```"))
            {
                var start = jsonContent.IndexOf("```") + 3;
                var end = jsonContent.IndexOf("```", start);
                if (end > start)
                {
                    jsonContent = jsonContent.Substring(start, end - start).Trim();
                }
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
    /// This helper is already implemented to ensure the app works without AI.
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
}
