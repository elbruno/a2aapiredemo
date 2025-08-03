using DataEntities;
using Microsoft.EntityFrameworkCore;
using Products.Models;
using SearchEntities;
using System.Text.Json;

namespace Products.Services;

public class A2AOrchestrationService : IA2AOrchestrationService
{
    private readonly Context _db;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<A2AOrchestrationService> _logger;

    public A2AOrchestrationService(
        Context db,
        IHttpClientFactory httpClientFactory,
        ILogger<A2AOrchestrationService> logger)
    {
        _db = db;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<A2ASearchResponse> ExecuteA2ASearchAsync(string searchTerm)
    {
        try
        {
            // Step 1: Find relevant products using standard search
            var products = await _db.Product
                .Where(p => p.Name.Contains(searchTerm) || p.Description.Contains(searchTerm))
                .Take(10)
                .ToListAsync();

            var enrichedProducts = new List<A2AEnrichedProduct>();

            // Step 2: For each product, orchestrate calls to agents
            foreach (var product in products)
            {
                var enrichedProduct = new A2AEnrichedProduct
                {
                    ProductId = product.Id.ToString(),
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    ImageUrl = product.ImageUrl
                };

                // Step 3: Call Inventory Agent
                var inventoryTask = GetInventoryAsync(product.Id.ToString());
                
                // Step 4: Call Promotions Agent  
                var promotionsTask = GetPromotionsAsync(product.Id.ToString());
                
                // Step 5: Call Researcher Agent
                var insightsTask = GetInsightsAsync(product.Id.ToString());

                // Wait for all agent calls to complete
                await Task.WhenAll(inventoryTask, promotionsTask, insightsTask);

                var inventory = await inventoryTask;
                var promotions = await promotionsTask;
                var insights = await insightsTask;

                // Step 6: Aggregate results
                enrichedProduct.Stock = inventory?.Stock ?? 0;
                enrichedProduct.Promotions = promotions?.Promotions ?? new List<A2APromotion>();
                enrichedProduct.Insights = insights?.Insights ?? new List<A2AInsight>();

                enrichedProducts.Add(enrichedProduct);
            }

            return new A2ASearchResponse
            {
                Products = enrichedProducts,
                Response = $"Found {enrichedProducts.Count} products enriched with inventory, promotions, and insights data."
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing A2A search for term: {searchTerm}", searchTerm);
            return new A2ASearchResponse
            {
                Products = new List<A2AEnrichedProduct>(),
                Response = "Error occurred during A2A search. Please try again."
            };
        }
    }

    private async Task<InventoryResponse?> GetInventoryAsync(string productId)
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

    private async Task<PromotionsResponse?> GetPromotionsAsync(string productId)
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

    private async Task<ResearchResponse?> GetInsightsAsync(string productId)
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
public record InventoryRequest(string ProductId);
public record InventoryResponse(string ProductId, int Stock);

public record PromotionsRequest(string ProductId);
public record PromotionsResponse(string ProductId, List<A2APromotion> Promotions);

public record ResearchRequest(string ProductId);
public record ResearchResponse(string ProductId, List<A2AInsight> Insights);