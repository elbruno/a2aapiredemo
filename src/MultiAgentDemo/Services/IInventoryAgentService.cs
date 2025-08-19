using SharedEntities;

namespace MultiAgentDemo.Services;

public interface IInventoryAgentService
{
    Task<InventorySearchResult> SearchProductsAsync(string productQuery);
}