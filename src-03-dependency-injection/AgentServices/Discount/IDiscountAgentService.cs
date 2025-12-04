using AgentServices.Models;

namespace AgentServices.Discount;

/// <summary>
/// DEMO: Interface for the Discount Agent Service.
/// </summary>
public interface IDiscountAgentService
{
    /// <summary>
    /// Computes membership-based discount using AI agent.
    /// </summary>
    Task<DiscountResult> ComputeDiscountAsync(DiscountRequest request);
}
