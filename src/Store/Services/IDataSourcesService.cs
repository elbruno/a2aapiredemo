using DataSources.Models;

namespace Store.Services;

/// <summary>
/// Interface for DataSources service communication
/// </summary>
public interface IDataSourcesService
{
    /// <summary>
    /// Index a list of URLs for semantic search
    /// </summary>
    Task<IndexUrlsResponse?> IndexUrlsAsync(IndexUrlsRequest request);

    /// <summary>
    /// Search indexed web content
    /// </summary>
    Task<SearchEntities.SearchResponse?> SearchAsync(string query);

    /// <summary>
    /// Get all indexed URLs
    /// </summary>
    Task<List<IndexedUrl>> GetIndexedUrlsAsync();
}