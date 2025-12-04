using System.Text.Json;
using AgentServices.Configuration;
using AgentServices.Models;
using DataEntities;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

namespace AgentServices.Discount;

/// <summary>
/// DEMO: Discount Agent Service
/// Uses Microsoft Agent Framework with Azure OpenAI to compute membership-based discounts.
/// 
/// TODO: This is the starting point for the live demo.
/// During the demo, you will implement the AI-powered discount calculation.
/// See docs/04_speaker-demo-walkthrough.md for step-by-step instructions.
/// </summary>
public class DiscountAgentService : IDiscountAgentService
{
    private readonly IChatClient? _chatClient;
    private readonly ILogger<DiscountAgentService> _logger;
    private readonly AgentSettings _settings;

    // TODO: Step 1.1 - Add the System Prompt here
    // The system prompt should define the discount rules:
    // - GOLD: 20% discount
    // - SILVER: 10% discount  
    // - NORMAL: no discount
    // See docs/04_speaker-demo-walkthrough.md#step-11-add-the-system-prompt

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
    /// TODO: Implement AI-powered discount calculation.
    /// See docs/04_speaker-demo-walkthrough.md#step-12-replace-the-computediscountasync-method
    /// </summary>
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

    // TODO: Step 1.3 - Add helper methods here
    // - ParseAgentResponse: Parse the JSON response from the AI
    // - ComputeFallbackDiscount: Deterministic fallback when AI is unavailable
    // See docs/04_speaker-demo-walkthrough.md#step-13-add-helper-methods
}
