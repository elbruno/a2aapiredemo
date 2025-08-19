using SharedEntities;

namespace MultiAgentDemo.Services;

public interface ILocationAgentService
{
    Task<LocationResult> FindProductLocationAsync(string productQuery);
}