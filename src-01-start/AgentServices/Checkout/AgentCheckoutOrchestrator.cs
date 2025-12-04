using AgentServices.Configuration;
using AgentServices.Discount;
using AgentServices.Models;
using AgentServices.Stock;
using CartEntities;
using Microsoft.Extensions.Logging;

namespace AgentServices.Checkout;

/// <summary>
/// DEMO: Agent Checkout Orchestrator
/// Coordinates the multi-agent checkout workflow:
/// 1. StockAgent → validates availability
/// 2. DiscountAgent → applies tier-based discount
/// 3. Returns combined result with agent steps log
/// 
/// TODO: This is the starting point for the live demo.
/// During the demo, you will implement the multi-agent checkout workflow.
/// See docs/04_speaker-demo-walkthrough.md for step-by-step instructions.
/// </summary>
public class AgentCheckoutOrchestrator : IAgentCheckoutOrchestrator
{
    private readonly IStockAgentService _stockAgent;
    private readonly IDiscountAgentService _discountAgent;
    private readonly AgentSettings _settings;
    private readonly ILogger<AgentCheckoutOrchestrator> _logger;

    public AgentCheckoutOrchestrator(
        IStockAgentService stockAgent,
        IDiscountAgentService discountAgent,
        AgentSettings settings,
        ILogger<AgentCheckoutOrchestrator> logger)
    {
        _stockAgent = stockAgent;
        _discountAgent = discountAgent;
        _settings = settings;
        _logger = logger;
    }

    /// <summary>
    /// DEMO: Execute the checkout workflow with discount calculation.
    /// For Demo 1, this orchestrator calls the DiscountAgent to apply membership discounts.
    /// </summary>
    public async Task<AgentCheckoutResult> ProcessCheckoutAsync(AgentCheckoutRequest request)
    {
        _logger.LogInformation("DEMO: Starting checkout workflow");
        _logger.LogInformation("DEMO: Membership tier: {Tier}, Cart subtotal: {Subtotal:C}", 
            request.MembershipTier, request.Cart.Subtotal);

        var result = new AgentCheckoutResult
        {
            Subtotal = request.Cart.Subtotal,
            AgentSteps = new List<AgentStep>()
        };

        try
        {
            // DEMO: Run DiscountAgent to calculate membership discount
            _logger.LogInformation("DEMO: Running DiscountAgent");
            var discountStep = await RunDiscountAgent(request, result);
            result.AgentSteps.Add(discountStep);

            // Finalize result
            result.Success = true;
            _logger.LogInformation("DEMO: Checkout completed successfully");
            _logger.LogInformation("DEMO: Final - Subtotal: {Subtotal:C}, Discount: {Discount:C}, Total: {Total:C}",
                result.Subtotal, result.DiscountAmount, result.TotalAfterDiscount);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "DEMO: Checkout workflow failed");
            
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
}
