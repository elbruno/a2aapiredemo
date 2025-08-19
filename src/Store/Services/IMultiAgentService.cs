using SharedEntities;

namespace Store.Services;

public interface IMultiAgentService
{
    Task<MultiAgentResponse?> AssistAsync(MultiAgentRequest request);
}