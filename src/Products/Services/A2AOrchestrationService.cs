using DataEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel;
using Products.Models;
using Products.Services.Agents;
using SearchEntities;
using System.Text.Json;

namespace Products.Services;

public class A2AOrchestrationService : IA2AOrchestrationService
{
    private readonly Context _db;
    private readonly InventoryAgent _inventoryAgent;
    private readonly PromotionsAgent _promotionsAgent;
    private readonly ResearcherAgent _researcherAgent;
    private readonly ILogger<A2AOrchestrationService> _logger;

    public A2AOrchestrationService(
        Context db,
        InventoryAgent inventoryAgent,
        PromotionsAgent promotionsAgent,
        ResearcherAgent researcherAgent,
        ILogger<A2AOrchestrationService> logger)
    {
        _db = db;
        _inventoryAgent = inventoryAgent;
        _promotionsAgent = promotionsAgent;
        _researcherAgent = researcherAgent;
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

                // Step 3: Call agents using Semantic Kernel Agents framework
                var inventoryTask = _inventoryAgent.CheckInventoryAsync(product.Id.ToString());
                
                // Step 4: Call Promotions Agent  
                var promotionsTask = _promotionsAgent.GetPromotionsAsync(product.Id.ToString());
                
                // Step 5: Call Researcher Agent
                var insightsTask = _researcherAgent.GetInsightsAsync(product.Id.ToString());

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
}