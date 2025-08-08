using SearchEntities;

namespace Search.Services;

public class MockNlWebClient : INlWebClient
{
    private readonly ILogger<MockNlWebClient> _logger;
    private readonly HttpClient _httpClient;

    // Mock data for demonstration
    private static readonly List<SearchResult> MockResults = new()
    {
        new SearchResult
        {
            Title = "Men's Running Shoes – Budget Picks",
            Url = "/products/123",
            Snippet = "Lightweight trainers ideal for daily runs and casual workouts...",
            Score = 0.87,
            Metadata = new Dictionary<string, object> { { "category", "running" }, { "price", 59.99 } }
        },
        new SearchResult
        {
            Title = "Women's Hiking Boots Collection", 
            Url = "/products/456",
            Snippet = "Waterproof and durable hiking boots for outdoor adventures...",
            Score = 0.82,
            Metadata = new Dictionary<string, object> { { "category", "hiking" }, { "price", 129.99 } }
        },
        new SearchResult
        {
            Title = "Outdoor Gear FAQ - Returns Policy",
            Url = "/support/returns",
            Snippet = "Learn about our 30-day return policy for all outdoor equipment...",
            Score = 0.75,
            Metadata = new Dictionary<string, object> { { "category", "support" }, { "type", "policy" } }
        },
        new SearchResult
        {
            Title = "Trail Running Guide",
            Url = "/guides/trail-running",
            Snippet = "Essential tips and gear recommendations for trail running enthusiasts...",
            Score = 0.68,
            Metadata = new Dictionary<string, object> { { "category", "guides" }, { "type", "content" } }
        }
    };

    public MockNlWebClient(ILogger<MockNlWebClient> logger, HttpClient httpClient)
    {
        _logger = logger;
        _httpClient = httpClient;
    }

    public async Task<SearchResponse> QueryAsync(string query, int top = 10, int skip = 0, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("MockNlWebClient: Processing search query: {Query}, top: {Top}, skip: {Skip}", query, top, skip);

        // Simulate network delay
        await Task.Delay(TimeSpan.FromMilliseconds(100), cancellationToken);

        // Filter results based on query (simple mock logic)
        var filteredResults = MockResults
            .Where(r => string.IsNullOrEmpty(query) || 
                       r.Title.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                       r.Snippet.Contains(query, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(r => r.Score)
            .Skip(skip)
            .Take(top)
            .ToList();

        var response = new SearchResponse
        {
            Query = query,
            Count = filteredResults.Count,
            Results = filteredResults,
            Response = $"Found {filteredResults.Count} results for '{query}'"
        };

        _logger.LogInformation("MockNlWebClient: Returning {Count} results", response.Count);
        return response;
    }

    public async Task<ReindexResponse> ReindexAsync(string? siteBaseUrl = null, bool force = false, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("MockNlWebClient: Reindexing site: {SiteBaseUrl}, force: {Force}", siteBaseUrl, force);

        // Simulate reindex operation
        await Task.Delay(TimeSpan.FromMilliseconds(500), cancellationToken);

        var response = new ReindexResponse
        {
            OperationId = Guid.NewGuid().ToString(),
            Status = "started",
            Message = $"Reindex operation started for {siteBaseUrl ?? "default site"}",
            StartedAt = DateTime.UtcNow
        };

        _logger.LogInformation("MockNlWebClient: Reindex operation started with ID: {OperationId}", response.OperationId);
        return response;
    }
}