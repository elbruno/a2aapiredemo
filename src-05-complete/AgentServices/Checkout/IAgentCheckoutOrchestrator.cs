using AgentServices.Models;

namespace AgentServices.Checkout;

/// <summary>
/// DEMO: Interface for the Agent Checkout Orchestrator.
/// </summary>
public interface IAgentCheckoutOrchestrator
{
    /// <summary>
    /// Processes checkout using the multi-agent workflow.
    /// </summary>
    Task<AgentCheckoutResult> ProcessCheckoutAsync(AgentCheckoutRequest request);
}
