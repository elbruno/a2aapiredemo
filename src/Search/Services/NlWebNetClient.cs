using SearchEntities;
using NLWebNet;
using NLWebNet.Models;
using Microsoft.Extensions.AI;
using System.Text.Json;

namespace Search.Services;

/// <summary>
/// NLWebNet implementation of the INlWebClient interface
/// Integrates with the NLWebNet library for natural language web interactions
/// </summary>
public class NlWebNetClient : INlWebClient
{
    private readonly ILogger<NlWebNetClient> _logger;
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly string _nlwebEndpoint;

    public NlWebNetClient(ILogger<NlWebNetClient> logger, HttpClient httpClient, IConfiguration configuration)
    {
        _logger = logger;
        _httpClient = httpClient;
        _configuration = configuration;
        
        // Get NLWeb endpoint from configuration or use the HttpClient's base address
        _nlwebEndpoint = _httpClient.BaseAddress?.ToString() 
                        ?? _configuration.GetConnectionString("nlweb") 
                        ?? _configuration["NLWeb:Endpoint"] 
                        ?? "http://nlweb:8000"; // Default for Docker container
    }

    public async Task<SearchResponse> QueryAsync(string query, int top = 10, int skip = 0, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("NlWebNetClient: Processing search query: {Query}, top: {Top}, skip: {Skip}", query, top, skip);

        try
        {
            // Call NLWeb /ask endpoint for natural language search
            var nlwebRequest = new
            {
                query = query,
                mode = "list", // Use list mode for search results
                max_results = top,
                offset = skip
            };

            var requestUri = "/ask";
            _logger.LogDebug("Calling NLWeb endpoint: {BaseAddress}{RequestUri}", _httpClient.BaseAddress, requestUri);

            var response = await _httpClient.PostAsJsonAsync(requestUri, nlwebRequest, cancellationToken);
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("NLWeb request failed with status: {StatusCode}, content: {Content}", 
                    response.StatusCode, await response.Content.ReadAsStringAsync(cancellationToken));
                
                // Return fallback results if NLWeb is unavailable
                return await GetFallbackResults(query, top);
            }

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogDebug("NLWeb response: {Response}", responseContent);

            var nlwebResponse = JsonSerializer.Deserialize<NLWebAskResponse>(responseContent, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                PropertyNameCaseInsensitive = true
            });

            // Convert NLWeb response to SearchResponse format
            var searchResults = ConvertNLWebResults(nlwebResponse, query);

            var searchResponse = new SearchResponse
            {
                Query = query,
                Count = searchResults.Count,
                Results = searchResults,
                Response = nlwebResponse?.Response ?? $"Found {searchResults.Count} results for '{query}'"
            };

            _logger.LogInformation("NlWebNetClient: Returning {Count} results from NLWeb", searchResponse.Count);
            return searchResponse;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error calling NLWeb: {Message}", ex.Message);
            return await GetFallbackResults(query, top);
        }
        catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
        {
            _logger.LogError(ex, "Timeout calling NLWeb for query: {Query}", query);
            return await GetFallbackResults(query, top);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "NlWebNetClient: Error processing query: {Query}", query);
            throw;
        }
    }

    public async Task<ReindexResponse> ReindexAsync(string? siteBaseUrl = null, bool force = false, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("NlWebNetClient: Reindexing site: {SiteBaseUrl}, force: {Force}", siteBaseUrl, force);

        try
        {
            // Use Store service URL as default site base URL
            siteBaseUrl ??= _configuration.GetConnectionString("store") ?? "https://store";

            // Call NLWeb data loading endpoint
            var nlwebRequest = new
            {
                url = siteBaseUrl,
                name = "eShopLite",
                force_reindex = force
            };

            var requestUri = "/admin/load_data";
            _logger.LogDebug("Calling NLWeb data loading endpoint: {BaseAddress}{RequestUri}", _httpClient.BaseAddress, requestUri);

            var response = await _httpClient.PostAsJsonAsync(requestUri, nlwebRequest, cancellationToken);
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("NLWeb reindex request failed with status: {StatusCode}", response.StatusCode);
                throw new InvalidOperationException($"NLWeb reindex failed with status: {response.StatusCode}");
            }

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var nlwebResponse = JsonSerializer.Deserialize<NLWebReindexResponse>(responseContent, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                PropertyNameCaseInsensitive = true
            });

            var reindexResponse = new ReindexResponse
            {
                OperationId = nlwebResponse?.OperationId ?? Guid.NewGuid().ToString(),
                Status = nlwebResponse?.Status ?? "started",
                Message = nlwebResponse?.Message ?? $"NLWeb reindex operation started for {siteBaseUrl}",
                StartedAt = DateTime.UtcNow
            };

            _logger.LogInformation("NlWebNetClient: Reindex operation started with ID: {OperationId}", reindexResponse.OperationId);
            return reindexResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "NlWebNetClient: Error during reindex operation");
            throw;
        }
    }

    // Chat methods - keeping simple implementation for now
    public async Task<SearchEntities.ChatResponse> ChatAsync(string message, string? sessionId = null, ChatContext? context = null, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("NlWebNetClient: Processing chat message: {Message}, sessionId: {SessionId}", message, sessionId);

        try
        {
            sessionId ??= Guid.NewGuid().ToString();

            // For now, use a simple chat response
            // In the future, this would integrate with NLWeb's chat capabilities
            var response = new SearchEntities.ChatResponse
            {
                SessionId = sessionId,
                Response = $"Thank you for your message: '{message}'. How can I help you find what you're looking for?",
                SuggestedActions = new List<SuggestedAction>
                {
                    new() { Text = "Search Products", Url = "/products" },
                    new() { Text = "About Us", Url = "/about-us" },
                    new() { Text = "Careers", Url = "/careers" }
                },
                Metadata = new ChatMetadata
                {
                    ResponseTime = 500,
                    Confidence = 0.8,
                    Sources = new List<string> { "/search" }
                }
            };

            _logger.LogInformation("NlWebNetClient: Chat response generated for session: {SessionId}", sessionId);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "NlWebNetClient: Error processing chat message: {Message}", message);
            throw;
        }
    }

    public async Task<ChatSession> GetChatSessionAsync(string sessionId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("NlWebNetClient: Retrieving chat session: {SessionId}", sessionId);
        
        await Task.Delay(50, cancellationToken);

        // For demo purposes, return a basic session
        return new ChatSession
        {
            Id = sessionId,
            CreatedAt = DateTime.UtcNow.AddMinutes(-30),
            LastActivity = DateTime.UtcNow,
            Messages = new List<SearchEntities.ChatMessage>(),
            Context = null
        };
    }

    /// <summary>
    /// Convert NLWeb response to SearchResult format
    /// </summary>
    private List<SearchResult> ConvertNLWebResults(NLWebAskResponse? nlwebResponse, string query)
    {
        if (nlwebResponse?.Results == null || !nlwebResponse.Results.Any())
        {
            return new List<SearchResult>();
        }

        return nlwebResponse.Results.Select((result, index) => new SearchResult
        {
            Title = result.Title ?? $"Result {index + 1}",
            Url = result.Url ?? "/",
            Snippet = result.Content ?? result.Summary ?? "No description available",
            Score = result.Score ?? (1.0 - (index * 0.1)), // Fallback scoring
            Metadata = new Dictionary<string, object>
            {
                { "source", result.Source ?? "nlweb" },
                { "type", result.Type ?? "page" },
                { "relevance", result.Score ?? 0.0 }
            }
        }).ToList();
    }

    /// <summary>
    /// Provide fallback results when NLWeb is unavailable
    /// </summary>
    private Task<SearchResponse> GetFallbackResults(string query, int top)
    {
        _logger.LogWarning("Using fallback results for query: {Query}", query);
        
        var fallbackResults = new List<SearchResult>
        {
            new SearchResult
            {
                Title = "Search Service Temporarily Unavailable",
                Url = "/search",
                Snippet = "The search service is currently unavailable. Please try again later or browse our products directly.",
                Score = 1.0,
                Metadata = new Dictionary<string, object> { { "type", "system" }, { "fallback", true } }
            },
            new SearchResult
            {
                Title = "Browse All Products",
                Url = "/products",
                Snippet = "Explore our complete product catalog including outdoor gear, apparel, and accessories.",
                Score = 0.9,
                Metadata = new Dictionary<string, object> { { "type", "navigation" }, { "fallback", true } }
            },
            new SearchResult
            {
                Title = "About Contoso",
                Url = "/about-us",
                Snippet = "Learn more about Contoso, our mission, and our commitment to quality outdoor equipment.",
                Score = 0.8,
                Metadata = new Dictionary<string, object> { { "type", "about" }, { "fallback", true } }
            }
        };

        return Task.FromResult(new SearchResponse
        {
            Query = query,
            Count = fallbackResults.Count,
            Results = fallbackResults.Take(top).ToList(),
            Response = "Search service temporarily unavailable. Showing fallback results."
        });
    }
}

/// <summary>
/// NLWeb /ask endpoint response model
/// </summary>
public class NLWebAskResponse
{
    public string? Response { get; set; }
    public List<NLWebResult>? Results { get; set; }
    public NLWebMetadata? Metadata { get; set; }
}

/// <summary>
/// Individual result from NLWeb
/// </summary>
public class NLWebResult
{
    public string? Title { get; set; }
    public string? Url { get; set; }
    public string? Content { get; set; }
    public string? Summary { get; set; }
    public double? Score { get; set; }
    public string? Source { get; set; }
    public string? Type { get; set; }
}

/// <summary>
/// NLWeb response metadata
/// </summary>
public class NLWebMetadata
{
    public int? TotalResults { get; set; }
    public double? ProcessingTime { get; set; }
    public string? Model { get; set; }
}

/// <summary>
/// NLWeb reindex response model
/// </summary>
public class NLWebReindexResponse
{
    public string? OperationId { get; set; }
    public string? Status { get; set; }
    public string? Message { get; set; }
}