using Microsoft.SemanticKernel;
using Products.Models;
using SearchEntities;
using System.ComponentModel;

namespace Products.Services.Agents;

/// <summary>
/// Researcher Agent using Semantic Kernel Agents framework
/// </summary>
public class ResearcherAgent
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ResearcherAgent> _logger;

    public ResearcherAgent(IHttpClientFactory httpClientFactory, ILogger<ResearcherAgent> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    [KernelFunction("get_insights")]
    [Description("Get product insights and reviews")]
    public async Task<ResearchResponse?> GetInsightsAsync(
        [Description("The product ID to get insights for")] string productId)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("ResearcherAgent");
            var request = new ResearchRequest(productId);
            
            var response = await client.PostAsJsonAsync("/api/researcher/insights", request);
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<ResearchResponse>();
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get insights for product {productId}", productId);
        }
        
        return null;
    }
}

// Agent request/response models
public record ResearchRequest(string ProductId);
public record ResearchResponse(string ProductId, List<A2AInsight> Insights);