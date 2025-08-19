using MultiAgentDemo.Models;

namespace MultiAgentDemo.Services;

public interface IInventoryAgentService
{
    Task<InventorySearchResult> SearchProductsAsync(string productQuery);
}

public interface IMatchmakingAgentService  
{
    Task<MatchmakingResult> FindAlternativesAsync(string productQuery, string userId);
}

public interface ILocationAgentService
{
    Task<LocationResult> FindProductLocationAsync(string productQuery);
}

public interface INavigationAgentService
{
    Task<NavigationInstructions> GenerateDirectionsAsync(string fromLocation, string toLocation);
}