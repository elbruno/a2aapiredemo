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
