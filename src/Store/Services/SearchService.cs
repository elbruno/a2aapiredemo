using SearchEntities;

namespace Store.Services;

public interface ISearchService
{
    Task<SearchResponse> SearchAsync(string query, int top = 10, int skip = 0);
}

public class SearchService : ISearchService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<SearchService> _logger;

    public SearchService(HttpClient httpClient, ILogger<SearchService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<SearchResponse> SearchAsync(string query, int top = 10, int skip = 0)
    {
        try
        {
            var url = $"/api/v1/search?q={Uri.EscapeDataString(query)}&top={top}&skip={skip}";
            _logger.LogInformation("Making search request to: {Url}", url);

            var response = await _httpClient.GetAsync(url);
            
            if (response.IsSuccessStatusCode)
            {
                var searchResponse = await response.Content.ReadFromJsonAsync<SearchResponse>();
                return searchResponse ?? new SearchResponse { Query = query, Count = 0, Results = new List<SearchResult>() };
            }
            else
            {
                _logger.LogError("Search API returned error: {StatusCode}", response.StatusCode);
                return new SearchResponse 
                { 
                    Query = query, 
                    Count = 0, 
                    Results = new List<SearchResult>(),
                    Response = $"Search service error: {response.StatusCode}"
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling search API for query: {Query}", query);
            return new SearchResponse 
            { 
                Query = query, 
                Count = 0, 
                Results = new List<SearchResult>(),
                Response = "Search service unavailable"
            };
        }
    }
}