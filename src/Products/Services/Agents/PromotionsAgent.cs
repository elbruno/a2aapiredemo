using Microsoft.SemanticKernel;
using Products.Models;
using SearchEntities;
using System.ComponentModel;

namespace Products.Services.Agents;

/// <summary>
/// Promotions Agent using Semantic Kernel Agents framework
/// </summary>
public class PromotionsAgent
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<PromotionsAgent> _logger;

    public PromotionsAgent(IHttpClientFactory httpClientFactory, ILogger<PromotionsAgent> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    [KernelFunction("get_promotions")]
    [Description("Get active promotions for a product")]
    public async Task<PromotionsResponse?> GetPromotionsAsync(
        [Description("The product ID to get promotions for")] string productId)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("PromotionsAgent");
            var request = new PromotionsRequest(productId);
            
            var response = await client.PostAsJsonAsync("/api/promotions/active", request);
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<PromotionsResponse>();
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get promotions for product {productId}", productId);
        }
        
        return null;
    }
}

// Agent request/response models
public record PromotionsRequest(string ProductId);
public record PromotionsResponse(string ProductId, List<A2APromotion> Promotions);