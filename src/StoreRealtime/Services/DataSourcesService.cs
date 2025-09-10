using DataSources.Models;
using SearchEntities;
using System.Text.Json;

namespace StoreRealtime.Services;

/// <summary>
/// Service for communicating with the DataSources API
/// </summary>
public class DataSourcesService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<DataSourcesService> _logger;

    public DataSourcesService(HttpClient httpClient, ILogger<DataSourcesService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    /// <summary>
    /// Index a list of URLs for semantic search
    /// </summary>
    public async Task<IndexUrlsResponse?> IndexUrlsAsync(IndexUrlsRequest request)
    {
        try
        {
            _logger.LogInformation("Indexing {Count} URLs", request.Urls.Count);

            var response = await _httpClient.PostAsJsonAsync("/api/datasources/index", request);
            var responseText = await response.Content.ReadAsStringAsync();

            _logger.LogInformation($"HTTP status code: {response.StatusCode}");
            _logger.LogInformation($"HTTP response content: {responseText}");

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<IndexUrlsResponse>();
                _logger.LogInformation("Successfully indexed URLs");
                return result;
            }
            else
            {
                _logger.LogWarning("Failed to index URLs. Status: {StatusCode}, Response: {Response}", 
                    response.StatusCode, responseText);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error indexing URLs");
        }

        return null;
    }

    /// <summary>
    /// Search indexed web content
    /// </summary>
    public async Task<DataSourcesSearchResponse?> SearchAsync(string query)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return new DataSourcesSearchResponse { Response = "Query cannot be empty" };
            }

            _logger.LogInformation("Searching web content for: {Query}", query);

            var encodedQuery = Uri.EscapeDataString(query);
            var response = await _httpClient.GetAsync($"/api/datasources/search/{encodedQuery}");
            var responseText = await response.Content.ReadAsStringAsync();

            _logger.LogInformation($"HTTP status code: {response.StatusCode}");
            _logger.LogInformation($"HTTP response content: {responseText}");

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<DataSourcesSearchResponse>();
                _logger.LogInformation("Successfully searched web content, found {SourceCount} sources", 
                    result?.SourceCount ?? 0);
                return result;
            }
            else
            {
                _logger.LogWarning("Failed to search web content. Status: {StatusCode}, Response: {Response}", 
                    response.StatusCode, responseText);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching web content");
        }

        return new DataSourcesSearchResponse { Response = "Error searching web content" };
    }

    /// <summary>
    /// Get all indexed URLs
    /// </summary>
    public async Task<List<IndexedUrl>> GetIndexedUrlsAsync()
    {
        try
        {
            _logger.LogInformation("Getting indexed URLs");

            var response = await _httpClient.GetAsync("/api/datasources/indexed");
            var responseText = await response.Content.ReadAsStringAsync();

            _logger.LogInformation($"HTTP status code: {response.StatusCode}");
            _logger.LogInformation($"HTTP response content: {responseText}");

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<List<IndexedUrl>>();
                _logger.LogInformation("Successfully retrieved {Count} indexed URLs", result?.Count ?? 0);
                return result ?? new List<IndexedUrl>();
            }
            else
            {
                _logger.LogWarning("Failed to get indexed URLs. Status: {StatusCode}, Response: {Response}", 
                    response.StatusCode, responseText);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting indexed URLs");
        }

        return new List<IndexedUrl>();
    }
}