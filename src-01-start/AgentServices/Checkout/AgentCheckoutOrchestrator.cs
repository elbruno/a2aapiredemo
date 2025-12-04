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
    /// TODO: Implement the multi-agent checkout workflow.
    /// See docs/04_speaker-demo-walkthrough.md#step-31-replace-the-processcheckoutasync-method
    /// </summary>
    public Task<AgentCheckoutResult> ProcessCheckoutAsync(AgentCheckoutRequest request)
    {
        _logger.LogInformation("TODO: Agent checkout orchestrator not implemented");
        
        var result = new AgentCheckoutResult
        {
            Subtotal = request.Cart.Subtotal,
            DiscountAmount = 0,
            DiscountReason = "Agent checkout not implemented",
            TotalAfterDiscount = request.Cart.Subtotal,
            AgentSteps = new List<AgentStep>
            {
                new AgentStep
                {
                    Name = "Orchestrator",
                    Status = "Pending",
                    Message = "Agent checkout not yet implemented - see TODO instructions",
                    Timestamp = DateTime.UtcNow
                }
            },
            Success = true
        };
        return Task.FromResult(result);
    }

    // TODO: Step 3.2 - Add the RunStockAgent method here
    // This method should call the StockAgent and return an AgentStep.
    // See docs/04_speaker-demo-walkthrough.md#step-32-add-the-runstockagent-method

    // TODO: Step 3.3 - Add the RunDiscountAgent method here
    // This method should call the DiscountAgent and return an AgentStep.
    // See docs/04_speaker-demo-walkthrough.md#step-33-add-the-rundiscountagent-method
}
