using AgentServices.Configuration;
using AgentServices.Models;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

namespace AgentServices.Discount;

/// <summary>
/// TODO: DEMO START POINT - Discount Agent Service
/// 
/// This is the starting point for the agentic demo. Implement the AI-powered
/// discount computation using Azure OpenAI.
/// 
/// INSTRUCTIONS:
/// 1. Create a system prompt that defines discount rules:
///    - GOLD tier: 20% discount
///    - SILVER tier: 10% discount
///    - NORMAL tier: No discount
/// 
/// 2. Use the IChatClient to send the customer's tier and cart subtotal
///    to the AI model and parse the JSON response.
/// 
/// 3. Handle errors gracefully and fall back to deterministic logic if needed.
/// 
/// See src-complete for the full implementation.
/// </summary>
public class DiscountAgentService : IDiscountAgentService
{
    private readonly IChatClient? _chatClient;
    private readonly ILogger<DiscountAgentService> _logger;
    private readonly AgentSettings _settings;

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
    /// TODO: Implement AI-powered discount computation.
    /// Currently returns no discount - implement the agent logic here.
    /// </summary>
    public Task<DiscountResult> ComputeDiscountAsync(DiscountRequest request)
    {
        _logger.LogInformation("TODO: DiscountAgent not implemented - Tier: {Tier}, Subtotal: {Subtotal:C}", 
            request.Tier, request.Subtotal);

        // TODO: Implement the following:
        // 1. Build a system prompt with discount rules
        // 2. Create a user message with cart context
        // 3. Call _chatClient.GetResponseAsync() with the messages
        // 4. Parse the JSON response to extract discountAmount and reason
        // 5. Return the DiscountResult

        // Placeholder: No discount applied (implement agent logic above)
        var result = new DiscountResult
        {
            DiscountAmount = 0,
            DiscountReason = "No discount applied - Agent not implemented",
            TotalAfterDiscount = request.Subtotal,
            Success = true
        };

        return Task.FromResult(result);
    }
}
