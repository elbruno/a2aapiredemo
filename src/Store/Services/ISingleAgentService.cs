using SharedEntities;

namespace Store.Services;

public interface ISingleAgentService
{
    Task<SingleAgentAnalysisResponse?> AnalyzeAsync(SingleAgentAnalysisRequest request);
}