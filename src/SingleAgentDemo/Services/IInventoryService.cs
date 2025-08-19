using SingleAgentDemo.Models;

namespace SingleAgentDemo.Services;

public interface IInventoryService
{
    Task<InternalToolRecommendation[]> EnrichWithInventoryAsync(InternalToolRecommendation[] tools);
}