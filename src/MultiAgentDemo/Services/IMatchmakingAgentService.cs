using SharedEntities;

namespace MultiAgentDemo.Services;

public interface IMatchmakingAgentService
{
    Task<MatchmakingResult> FindAlternativesAsync(string productQuery, string userId);
}