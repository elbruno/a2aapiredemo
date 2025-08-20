using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using MultiAgentDemo.Services;
using SharedEntities;

namespace MultiAgentDemo.Controllers
{
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

        // Strongly-typed kernel instance injected via DI.
        // Use `_kernel` directly for any operations that require Semantic Kernel functionality.

        [HttpPost("assist")]
        public async Task<ActionResult<MultiAgentResponse>> AssistAsync([FromBody] MultiAgentRequest? request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.ProductQuery))
            {
                return BadRequest("Request body is required and must include a ProductQuery.");
            }

            var orchestrationId = Guid.NewGuid().ToString();
            _logger.LogInformation("Starting orchestration {OrchestrationId}", orchestrationId);

            var steps = new List<AgentStep>
            {
                await RunInventoryAgentAsync(request.ProductQuery),
                await RunMatchmakingAgentAsync(request.ProductQuery, request.UserId),
                await RunLocationAgentAsync(request.ProductQuery)
            };

            NavigationInstructions? navigation = null;
            if (request.Location != null)
            {
                steps.Add(await RunNavigationAgentAsync(request.Location, request.ProductQuery));
                navigation = await GenerateNavigationInstructionsAsync(request.Location, request.ProductQuery);
            }

            var alternatives = await GenerateProductAlternativesAsync(request.ProductQuery);

            var response = new MultiAgentResponse
            {
                OrchestrationId = orchestrationId,
                Steps = steps.ToArray(),
                Alternatives = alternatives,
                NavigationInstructions = navigation
            };

            return Ok(response);
        }

        private async Task<AgentStep> RunInventoryAgentAsync(string productQuery)
        {
            try
            {
                var result = await _inventoryAgentService.SearchProductsAsync(productQuery);
                var names = result?.ProductsFound?.Select(p => p.Name) ?? Enumerable.Empty<string>();
                var desc = $"Found {result?.TotalCount ?? 0} products: {string.Join(", ", names)}";
                return new AgentStep { Agent = "InventoryAgent", Action = $"Search {productQuery}", Result = desc, Timestamp = DateTime.UtcNow };
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Inventory agent failed");
                return new AgentStep { Agent = "InventoryAgent", Action = $"Search {productQuery}", Result = "Fallback inventory result", Timestamp = DateTime.UtcNow };
            }
        }

        private async Task<AgentStep> RunMatchmakingAgentAsync(string productQuery, string userId)
        {
            try
            {
                var result = await _matchmakingAgentService.FindAlternativesAsync(productQuery, userId);
                var count = result?.Alternatives?.Length ?? 0;
                return new AgentStep { Agent = "MatchmakingAgent", Action = $"Find alternatives {productQuery}", Result = $"{count} alternatives found", Timestamp = DateTime.UtcNow };
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Matchmaking agent failed");
                return new AgentStep { Agent = "MatchmakingAgent", Action = $"Find alternatives {productQuery}", Result = "Fallback alternatives", Timestamp = DateTime.UtcNow };
            }
        }

        private async Task<AgentStep> RunLocationAgentAsync(string productQuery)
        {
            try
            {
                var result = await _locationAgentService.FindProductLocationAsync(productQuery);
                var loc = result?.StoreLocations?.FirstOrDefault();
                var desc = loc != null ? $"Located in {loc.Section} Aisle {loc.Aisle}" : "Location not found";
                return new AgentStep { Agent = "LocationAgent", Action = $"Locate {productQuery}", Result = desc, Timestamp = DateTime.UtcNow };
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Location agent failed");
                return new AgentStep { Agent = "LocationAgent", Action = $"Locate {productQuery}", Result = "Fallback location", Timestamp = DateTime.UtcNow };
            }
        }

        private async Task<AgentStep> RunNavigationAgentAsync(Location? location, string productQuery)
        {
            try
            {
                if (location == null) return new AgentStep { Agent = "NavigationAgent", Action = "Navigate", Result = "No start location", Timestamp = DateTime.UtcNow };
                var dest = new Location { Lat = 0, Lon = 0 };
                var nav = await _navigationAgentService.GenerateDirectionsAsync(location, dest);
                var steps = nav?.Steps?.Length ?? 0;
                return new AgentStep { Agent = "NavigationAgent", Action = "Navigate to product", Result = $"{steps} steps", Timestamp = DateTime.UtcNow };
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Navigation agent failed");
                return new AgentStep { Agent = "NavigationAgent", Action = "Navigate", Result = "Fallback navigation", Timestamp = DateTime.UtcNow };
            }
        }

        private async Task<NavigationInstructions> GenerateNavigationInstructionsAsync(Location? location, string productQuery)
        {
            if (location == null) return new NavigationInstructions { Steps = Array.Empty<NavigationStep>(), StartLocation = string.Empty, EstimatedTime = string.Empty };
            var dest = new Location { Lat = 0, Lon = 0 };
            try
            {
                return await _navigationAgentService.GenerateDirectionsAsync(location, dest);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "GenerateNavigationInstructions failed");
                return new NavigationInstructions { Steps = new[] { new NavigationStep { Direction = "General", Description = $"Head to the area where {productQuery} is typically located", Landmark = new NavigationLandmark { Description = "General area" } } }, StartLocation = string.Empty, EstimatedTime = string.Empty };
            }
        }

        private async Task<ProductAlternative[]> GenerateProductAlternativesAsync(string productQuery)
        {
            await Task.Delay(10);
            return new[]
            {
                new ProductAlternative { Name = $"Standard {productQuery}", Sku = "STD-" + productQuery.Replace(" ", "").ToUpper(), Price = 49.99m, InStock = true, Location = "Aisle 5", Aisle = 5, Section = "B" },
                new ProductAlternative { Name = $"Budget {productQuery}", Sku = "BDG-" + productQuery.Replace(" ", "").ToUpper(), Price = 24.99m, InStock = false, Location = "Aisle 12", Aisle = 12, Section = "C" }
            };
        }
    }
}
