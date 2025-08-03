using Microsoft.SemanticKernel;
using Products.Models;
using System.ComponentModel;

namespace Products.Services.Agents;

/// <summary>
/// Inventory Agent using Semantic Kernel Agents framework
/// </summary>
public class InventoryAgent
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<InventoryAgent> _logger;

    public InventoryAgent(IHttpClientFactory httpClientFactory, ILogger<InventoryAgent> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    [KernelFunction("check_inventory")]
    [Description("Check inventory levels for a product")]
    public async Task<InventoryResponse?> CheckInventoryAsync(
        [Description("The product ID to check inventory for")] string productId)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("InventoryAgent");
            var request = new InventoryRequest(productId);
            
            var response = await client.PostAsJsonAsync("/api/inventory/check", request);
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<InventoryResponse>();
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get inventory for product {productId}", productId);
        }
        
        return null;
    }
}

// Agent request/response models
public record InventoryRequest(string ProductId);
public record InventoryResponse(string ProductId, int Stock);