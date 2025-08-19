using MultiAgentDemo.Models;

namespace MultiAgentDemo.Services;

public class InventoryAgentService : IInventoryAgentService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<InventoryAgentService> _logger;

    public InventoryAgentService(HttpClient httpClient, ILogger<InventoryAgentService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<InventorySearchResult> SearchProductsAsync(string productQuery)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/inventory/search?query={Uri.EscapeDataString(productQuery)}");
            
            _logger.LogInformation($"InventoryAgentService HTTP status code: {response.StatusCode}");
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<InventorySearchResult>();
                return result ?? CreateFallbackInventoryResult(productQuery);
            }
            
            _logger.LogWarning("InventoryAgentService returned non-success status: {StatusCode}", response.StatusCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling InventoryAgentService");
        }

        return CreateFallbackInventoryResult(productQuery);
    }

    private InventorySearchResult CreateFallbackInventoryResult(string productQuery)
    {
        return new InventorySearchResult
        {
            ProductsFound = new[]
            {
                new ProductInfo { Name = $"Demo Product for {productQuery}", Sku = "DEMO-001", Price = 29.99m, IsAvailable = true }
            },
            TotalCount = 1,
            SearchQuery = productQuery
        };
    }
}

public class MatchmakingAgentService : IMatchmakingAgentService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<MatchmakingAgentService> _logger;

    public MatchmakingAgentService(HttpClient httpClient, ILogger<MatchmakingAgentService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<MatchmakingResult> FindAlternativesAsync(string productQuery, string userId)
    {
        try
        {
            var request = new { ProductQuery = productQuery, UserId = userId };
            var response = await _httpClient.PostAsJsonAsync("/api/matchmaking/alternatives", request);
            
            _logger.LogInformation($"MatchmakingAgentService HTTP status code: {response.StatusCode}");
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<MatchmakingResult>();
                return result ?? CreateFallbackMatchmakingResult(productQuery);
            }
            
            _logger.LogWarning("MatchmakingAgentService returned non-success status: {StatusCode}", response.StatusCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling MatchmakingAgentService");
        }

        return CreateFallbackMatchmakingResult(productQuery);
    }

    private MatchmakingResult CreateFallbackMatchmakingResult(string productQuery)
    {
        return new MatchmakingResult
        {
            Alternatives = new[]
            {
                new ProductInfo { Name = $"Alternative for {productQuery}", Sku = "ALT-001", Price = 19.99m, IsAvailable = true }
            },
            SimilarProducts = new[]
            {
                new ProductInfo { Name = $"Similar to {productQuery}", Sku = "SIM-001", Price = 24.99m, IsAvailable = true }
            }
        };
    }
}

public class LocationAgentService : ILocationAgentService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<LocationAgentService> _logger;

    public LocationAgentService(HttpClient httpClient, ILogger<LocationAgentService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<LocationResult> FindProductLocationAsync(string productQuery)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/location/find?product={Uri.EscapeDataString(productQuery)}");
            
            _logger.LogInformation($"LocationAgentService HTTP status code: {response.StatusCode}");
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<LocationResult>();
                return result ?? CreateFallbackLocationResult(productQuery);
            }
            
            _logger.LogWarning("LocationAgentService returned non-success status: {StatusCode}", response.StatusCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling LocationAgentService");
        }

        return CreateFallbackLocationResult(productQuery);
    }

    private LocationResult CreateFallbackLocationResult(string productQuery)
    {
        return new LocationResult
        {
            StoreLocations = new[]
            {
                new StoreLocation { Section = "Hardware", Aisle = "A1", Shelf = "Top", Description = $"Location for {productQuery}" }
            }
        };
    }
}

public class NavigationAgentService : INavigationAgentService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<NavigationAgentService> _logger;

    public NavigationAgentService(HttpClient httpClient, ILogger<NavigationAgentService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<NavigationInstructions> GenerateDirectionsAsync(string fromLocation, string toLocation)
    {
        try
        {
            var request = new { From = fromLocation, To = toLocation };
            var response = await _httpClient.PostAsJsonAsync("/api/navigation/directions", request);
            
            _logger.LogInformation($"NavigationAgentService HTTP status code: {response.StatusCode}");
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<NavigationInstructions>();
                return result ?? CreateFallbackNavigationInstructions(fromLocation, toLocation);
            }
            
            _logger.LogWarning("NavigationAgentService returned non-success status: {StatusCode}", response.StatusCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling NavigationAgentService");
        }

        return CreateFallbackNavigationInstructions(fromLocation, toLocation);
    }

    private NavigationInstructions CreateFallbackNavigationInstructions(string fromLocation, string toLocation)
    {
        return new NavigationInstructions
        {
            Steps = new[]
            {
                $"Head towards {toLocation} from {fromLocation}",
                "Follow the main pathway",
                $"You will find your destination at {toLocation}"
            }
        };
    }
}