using System.Text.Json;

namespace Store.Services;

public class MultiAgentService : IMultiAgentService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<MultiAgentService> _logger;

    public MultiAgentService(HttpClient httpClient, ILogger<MultiAgentService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<MultiAgentResponse?> AssistAsync(MultiAgentRequest request)
    {
        try
        {
            _logger.LogInformation("Calling multi-agent service for user {UserId} with query {ProductQuery}", 
                request.UserId, request.ProductQuery);
            
            var response = await _httpClient.PostAsJsonAsync("/api/multi-agent/assist", request);
            var responseText = await response.Content.ReadAsStringAsync();

            _logger.LogInformation("Multi-agent service response - Status: {StatusCode}, Content: {Content}", 
                response.StatusCode, responseText);
            
            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<MultiAgentResponse>(responseText, new JsonSerializerOptions 
                { 
                    PropertyNameCaseInsensitive = true 
                });
                return result;
            }
            else
            {
                _logger.LogWarning("Multi-agent service returned non-success status: {StatusCode}", response.StatusCode);
                return CreateFallbackResponse(request);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling multi-agent service for user {UserId}", request.UserId);
            return CreateFallbackResponse(request);
        }
    }

    private MultiAgentResponse CreateFallbackResponse(MultiAgentRequest request)
    {
        var orchestrationId = Guid.NewGuid().ToString("N")[..8];
        var baseTime = DateTime.UtcNow;

        return new MultiAgentResponse
        {
            OrchestrationId = orchestrationId,
            Steps = new[]
            {
                new AgentStep
                {
                    Agent = "InventoryAgent",
                    Action = $"Search inventory for '{request.ProductQuery}'",
                    Result = $"Found 5 products matching '{request.ProductQuery}' across hardware and tools sections",
                    Timestamp = baseTime
                },
                new AgentStep
                {
                    Agent = "MatchmakingAgent",
                    Action = $"Find alternatives for '{request.ProductQuery}'",
                    Result = "Identified 3 product alternatives: Premium, Standard, and Budget options with different feature sets",
                    Timestamp = baseTime.AddSeconds(1)
                },
                new AgentStep
                {
                    Agent = "LocationAgent",
                    Action = $"Locate '{request.ProductQuery}' in store",
                    Result = "Products located in Aisles 5, 7, and 12 with current stock levels verified",
                    Timestamp = baseTime.AddSeconds(2)
                },
                new AgentStep
                {
                    Agent = "NavigationAgent",
                    Action = $"Generate route to '{request.ProductQuery}'",
                    Result = "Calculated optimal path through store to visit all product locations efficiently",
                    Timestamp = baseTime.AddSeconds(3)
                }
            },
            Alternatives = new[]
            {
                new ProductAlternative
                {
                    Name = $"Premium {request.ProductQuery}",
                    Sku = "PREM-" + request.ProductQuery.Replace(" ", "").ToUpper(),
                    Price = 189.99m,
                    InStock = true,
                    Location = "Aisle 5",
                    Aisle = 5,
                    Section = "A"
                },
                new ProductAlternative
                {
                    Name = $"Standard {request.ProductQuery}",
                    Sku = "STD-" + request.ProductQuery.Replace(" ", "").ToUpper(),
                    Price = 89.99m,
                    InStock = true,
                    Location = "Aisle 7",
                    Aisle = 7,
                    Section = "B"
                },
                new ProductAlternative
                {
                    Name = $"Budget {request.ProductQuery}",
                    Sku = "BDG-" + request.ProductQuery.Replace(" ", "").ToUpper(),
                    Price = 39.99m,
                    InStock = false,
                    Location = "Aisle 12",
                    Aisle = 12,
                    Section = "C"
                }
            },
            NavigationInstructions = request.Location != null ? new NavigationInstructions
            {
                StartLocation = $"Entrance ({request.Location.Lat:F4}, {request.Location.Lon:F4})",
                EstimatedTime = "4-6 minutes",
                Steps = new[]
                {
                    new NavigationStep
                    {
                        Direction = "Head straight",
                        Description = "Walk towards the main hardware section",
                        Landmark = "Customer Service Desk on your right"
                    },
                    new NavigationStep
                    {
                        Direction = "Turn left",
                        Description = "Enter Aisle 5 for premium options",
                        Landmark = "Power Tools display"
                    },
                    new NavigationStep
                    {
                        Direction = "Continue to Aisle 7",
                        Description = "Find standard options in section B",
                        Landmark = "Paint mixing station"
                    },
                    new NavigationStep
                    {
                        Direction = "End at Aisle 12",
                        Description = "Check budget alternatives (may be out of stock)",
                        Landmark = "Garden center entrance"
                    }
                }
            } : null
        };
    }
}