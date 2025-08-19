using SingleAgentDemo.Models;

namespace SingleAgentDemo.Services;

public interface ICustomerInformationService
{
    Task<CustomerInformation> GetCustomerInformationAsync(string customerId);
    Task<ToolMatchResult> MatchToolsAsync(string customerId, string[] detectedMaterials, string prompt);
}