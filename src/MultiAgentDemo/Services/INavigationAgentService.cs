using SharedEntities;

namespace MultiAgentDemo.Services;

public interface INavigationAgentService
{
    Task<NavigationInstructions> GenerateDirectionsAsync(Location fromLocation, Location toLocation);
}