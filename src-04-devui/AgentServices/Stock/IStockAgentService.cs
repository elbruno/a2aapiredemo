using AgentServices.Models;

namespace AgentServices.Stock;

/// <summary>
/// DEMO: Interface for the Stock Agent Service.
/// </summary>
public interface IStockAgentService
{
    /// <summary>
    /// Checks stock availability for cart items.
    /// </summary>
    Task<StockCheckResult> CheckStockAsync(StockCheckRequest request);
}
