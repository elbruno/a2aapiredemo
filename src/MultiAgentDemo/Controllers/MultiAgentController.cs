using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
using MultiAgentDemo.Models;
using MultiAgentDemo.Services;
using SharedEntities;

namespace MultiAgentDemo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MultiAgentController : ControllerBase
{
    private readonly ILogger<MultiAgentController> _logger;
    private readonly Kernel _kernel;
    private readonly IInventoryAgentService _inventoryAgentService;
    private readonly IMatchmakingAgentService _matchmakingAgentService;
    private readonly ILocationAgentService _locationAgentService;
    private readonly INavigationAgentService _navigationAgentService;

    public MultiAgentController(
        ILogger<MultiAgentController> logger,
        Kernel kernel,
        IInventoryAgentService inventoryAgentService,
        IMatchmakingAgentService matchmakingAgentService,
        ILocationAgentService locationAgentService,
        INavigationAgentService navigationAgentService)
    {
        _logger = logger;
        _kernel = kernel;
        _inventoryAgentService = inventoryAgentService;
        _matchmakingAgentService = matchmakingAgentService;
        _locationAgentService = locationAgentService;
        _navigationAgentService = navigationAgentService;
    }

    [HttpPost("assist")]
    public async Task<ActionResult<SharedEntities.MultiAgentResponse>> AssistAsync([FromBody] SharedEntities.MultiAgentRequest request)
    {
        try
        {
            var orchestrationId = Guid.NewGuid().ToString();
            _logger.LogInformation("Starting multi-agent orchestration {OrchestrationId} for user {UserId}", orchestrationId, request.UserId);

            var steps = new List<SharedEntities.AgentStep>();
            
            // Step 1: Inventory Agent - Search for products
            var inventoryStep = await RunInventoryAgentAsync(request.ProductQuery);
            steps.Add(inventoryStep);
            
            // Step 2: Matchmaking Agent - Find alternatives and similar products
            var matchmakingStep = await RunMatchmakingAgentAsync(request.ProductQuery, request.UserId);
            steps.Add(matchmakingStep);
            
            // Step 3: Location Agent - Find product locations in store
            var locationStep = await RunLocationAgentAsync(request.ProductQuery);
            steps.Add(locationStep);
            
            // Step 4: Navigation Agent - Generate in-store directions
            NavigationInstructions? navigation = null;
            if (request.Location != null)
            {
                navigation = await _navigationAgentService.GenerateDirectionsAsync(request.Location, locationStep.Data?.ToString() ?? "Product Section");
            }
            {
                var navigationStep = await RunNavigationAgentAsync(request.Location, request.ProductQuery);
                steps.Add(navigationStep);
                navigation = await GenerateNavigationInstructionsAsync(request.Location, request.ProductQuery);
            }

            // Generate alternatives based on agent results
            var alternatives = await GenerateProductAlternativesAsync(request.ProductQuery);

            var response = new SharedEntities.MultiAgentResponse
            {
                OrchestrationId = orchestrationId,
                Steps = steps.ToArray(),
                Alternatives = alternatives,
                NavigationInstructions = navigation
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in multi-agent orchestration for user {UserId}", request.UserId);
            return StatusCode(500, "An error occurred while processing your request");
        }
    }

    private async Task<AgentStep> RunInventoryAgentAsync(string productQuery)
    {
        try
        {
            var result = await _inventoryAgentService.SearchProductsAsync(productQuery);
            
            var resultDescription = $"Found {result.TotalCount} products matching '{productQuery}': {string.Join(", ", result.ProductsFound.Select(p => p.Name))}";

            return new SharedEntities.AgentStep
            {
                Agent = "InventoryAgent",
                Action = $"Search inventory for '{productQuery}'",
                Result = resultDescription,
                Data = result,
                Timestamp = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Inventory agent failed, using fallback");
            return new SharedEntities.AgentStep
            {
                Agent = "InventoryAgent",
                Action = $"Search inventory for '{productQuery}'",
                Result = $"Found products matching '{productQuery}' in hardware and paint sections",
                Timestamp = DateTime.UtcNow
            };
        }
    }

    private async Task<AgentStep> RunMatchmakingAgentAsync(string productQuery, string userId)
    {
        try
        {
            var result = await _matchmakingAgentService.FindAlternativesAsync(productQuery, userId);
            
            var resultDescription = $"Found {result.Alternatives.Length} alternatives and {result.SimilarProducts.Length} similar products for '{productQuery}'";

            return new SharedEntities.AgentStep
            {
                Agent = "MatchmakingAgent",
                Action = $"Find alternatives for '{productQuery}'",
                Result = resultDescription,
                Data = result,
                Timestamp = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Matchmaking agent failed, using fallback");
            return new SharedEntities.AgentStep
            {
                Agent = "MatchmakingAgent",
                Action = $"Find alternatives for '{productQuery}'",
                Result = $"Identified premium and budget alternatives for '{productQuery}'",
                Timestamp = DateTime.UtcNow
            };
        }
    }

    private async Task<AgentStep> RunLocationAgentAsync(string productQuery)
    {
        try
        {
            var result = await _locationAgentService.FindProductLocationAsync(productQuery);
            
            var locationInfo = result.StoreLocations.FirstOrDefault();
            var resultDescription = locationInfo != null ? 
                $"Products located in {locationInfo.Section}, {locationInfo.Aisle}" : 
                $"Products located in store for '{productQuery}'";

            return new SharedEntities.AgentStep
            {
                Agent = "LocationAgent",
                Action = $"Find location for '{productQuery}'",
                Result = resultDescription,
                Data = locationInfo?.Section,
                Timestamp = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Location agent failed, using fallback");
            return new SharedEntities.AgentStep
            {
                Agent = "LocationAgent",
                Action = $"Find location for '{productQuery}'",
                Result = $"Products located in Aisle 5 and Aisle 12",
                Timestamp = DateTime.UtcNow
            };
        }
    }

    private async Task<SharedEntities.ProductAlternative[]> GenerateProductAlternativesAsync(string productQuery)
    {
        // Simulate multi-agent coordination results
        await Task.Delay(50); // Simulate processing time

        return new SharedEntities.ProductAlternative[]
        {
            new SharedEntities.ProductAlternative
            {
                Name = $"Premium {productQuery}",
                Sku = "PREM-" + productQuery.Replace(" ", "").ToUpper(),
                Price = 89.99m,
                InStock = true,
                Location = "Aisle 5",
                Aisle = 5,
                Section = "A"
            },
            new SharedEntities.ProductAlternative
            {
                Name = $"Standard {productQuery}",
                Sku = "STD-" + productQuery.Replace(" ", "").ToUpper(),
                Price = 49.99m,
                InStock = true,
                Location = "Aisle 5",
                Aisle = 5,
                Section = "B"
            },
            new SharedEntities.ProductAlternative
            {
                Name = $"Budget {productQuery}",
                Sku = "BDG-" + productQuery.Replace(" ", "").ToUpper(),
                Price = 24.99m,
                InStock = false,
                Location = "Aisle 12",
                Aisle = 12,
                Section = "C"
            }
        };
    }
}