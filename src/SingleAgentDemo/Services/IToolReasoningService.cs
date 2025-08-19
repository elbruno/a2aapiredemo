using SingleAgentDemo.Models;

namespace SingleAgentDemo.Services;

public interface IToolReasoningService
{
    Task<string> GenerateReasoningAsync(ReasoningRequest request);
}