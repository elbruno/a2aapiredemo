using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
using MultiAgentDemo.Models;

namespace MultiAgentDemo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MultiAgentController : ControllerBase
{
    private readonly ILogger<MultiAgentController> _logger;
    private readonly Kernel _kernel;
    private readonly IHttpClientFactory _httpClientFactory;

    public MultiAgentController(
        ILogger<MultiAgentController> logger,
        Kernel kernel,
        IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _kernel = kernel;
        _httpClientFactory = httpClientFactory;
    }

    [HttpPost("assist")]
    public async Task<ActionResult<MultiAgentResponse>> AssistAsync([FromBody] MultiAgentRequest request)
    {
        try
        {
            var orchestrationId = Guid.NewGuid().ToString();
            _logger.LogInformation("Starting multi-agent orchestration {OrchestrationId} for user {UserId}", orchestrationId, request.UserId);

            var steps = new List<AgentStep>();
            
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
                var navigationStep = await RunNavigationAgentAsync(request.Location, request.ProductQuery);
                steps.Add(navigationStep);
                navigation = await GenerateNavigationInstructionsAsync(request.Location, request.ProductQuery);
            }

            // Generate alternatives based on agent results
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in multi-agent orchestration for user {UserId}", request.UserId);
            return StatusCode(500, "An error occurred while processing your request");
        }
    }

    private async Task<AgentStep> RunInventoryAgentAsync(string productQuery)
    {
        var client = _httpClientFactory.CreateClient("InventoryAgent");
        
        try
        {
            var response = await client.PostAsJsonAsync("/api/mock/inventory/search", new { query = productQuery });
            
            var result = response.IsSuccessStatusCode ? 
                await response.Content.ReadAsStringAsync() : 
                $"Found 3 products matching '{productQuery}'";

            return new AgentStep
            {
                Agent = "InventoryAgent",
                Action = $"Search inventory for '{productQuery}'",
                Result = result,
                Timestamp = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Inventory agent failed, using fallback");
            return new AgentStep
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
        var client = _httpClientFactory.CreateClient("MatchmakingAgent");
        
        try
        {
            var response = await client.PostAsJsonAsync("/api/mock/matchmaking/alternatives", new { query = productQuery, userId });
            
            var result = response.IsSuccessStatusCode ? 
                await response.Content.ReadAsStringAsync() : 
                $"Found 2 alternative products for '{productQuery}'";

            return new AgentStep
            {
                Agent = "MatchmakingAgent",
                Action = $"Find alternatives for '{productQuery}'",
                Result = result,
                Timestamp = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Matchmaking agent failed, using fallback");
            return new AgentStep
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
        var client = _httpClientFactory.CreateClient("LocationAgent");
        
        try
        {
            var response = await client.PostAsJsonAsync("/api/mock/location/find", new { query = productQuery });
            
            var result = response.IsSuccessStatusCode ? 
                await response.Content.ReadAsStringAsync() : 
                $"Products located in Aisle 5 and Aisle 12";

            return new AgentStep
            {
                Agent = "LocationAgent",
                Action = $"Locate '{productQuery}' in store",
                Result = result,
                Timestamp = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Location agent failed, using fallback");
            return new AgentStep
            {
                Agent = "LocationAgent",
                Action = $"Locate '{productQuery}' in store",
                Result = $"Found '{productQuery}' in multiple aisles with current stock levels",
                Timestamp = DateTime.UtcNow
            };
        }
    }

    private async Task<AgentStep> RunNavigationAgentAsync(Location userLocation, string productQuery)
    {
        var client = _httpClientFactory.CreateClient("NavigationAgent");
        
        try
        {
            var request = new { userLocation, destination = productQuery };
            var response = await client.PostAsJsonAsync("/api/mock/navigation/route", request);
            
            var result = response.IsSuccessStatusCode ? 
                await response.Content.ReadAsStringAsync() : 
                "Generated optimal route to product locations";

            return new AgentStep
            {
                Agent = "NavigationAgent",
                Action = $"Generate route to '{productQuery}'",
                Result = result,
                Timestamp = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Navigation agent failed, using fallback");
            return new AgentStep
            {
                Agent = "NavigationAgent",
                Action = $"Generate route to '{productQuery}'",
                Result = "Calculated efficient path through store to reach all product locations",
                Timestamp = DateTime.UtcNow
            };
        }
    }

    private async Task<ProductAlternative[]> GenerateProductAlternativesAsync(string productQuery)
    {
        // Simulate multi-agent coordination results
        await Task.Delay(50); // Simulate processing time

        return new ProductAlternative[]
        {
            new ProductAlternative
            {
                Name = $"Premium {productQuery}",
                Sku = "PREM-" + productQuery.Replace(" ", "").ToUpper(),
                Price = 89.99m,
                InStock = true,
                Location = "Aisle 5",
                Aisle = 5,
                Section = "A"
            },
            new ProductAlternative
            {
                Name = $"Standard {productQuery}",
                Sku = "STD-" + productQuery.Replace(" ", "").ToUpper(),
                Price = 49.99m,
                InStock = true,
                Location = "Aisle 5",
                Aisle = 5,
                Section = "B"
            },
            new ProductAlternative
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

    private async Task<NavigationInstructions> GenerateNavigationInstructionsAsync(Location userLocation, string productQuery)
    {
        // Simulate navigation generation
        await Task.Delay(50);

        return new NavigationInstructions
        {
            StartLocation = $"Current position ({userLocation.Lat:F4}, {userLocation.Lon:F4})",
            EstimatedTime = "3-4 minutes",
            Steps = new NavigationStep[]
            {
                new NavigationStep
                {
                    Direction = "Head east",
                    Description = "Walk towards the hardware section",
                    Landmark = "Customer Service Desk"
                },
                new NavigationStep
                {
                    Direction = "Turn right",
                    Description = "Enter Aisle 5",
                    Landmark = "Paint Display"
                },
                new NavigationStep
                {
                    Direction = "Continue straight",
                    Description = $"Find {productQuery} in section A",
                    Landmark = "End of aisle"
                }
            }
        };
    }
}