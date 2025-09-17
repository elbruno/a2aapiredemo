using DataEntities;
using DataSources.Memory;
using SearchEntities;

namespace DataSources.Endpoints;

/// <summary>
/// API endpoints for the DataSources service
/// </summary>
public static class DataSourcesEndpoints
{
    /// <summary>
    /// Configure DataSources endpoints
    /// </summary>
    public static void MapDataSourcesEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/datasources")
            .WithTags("DataSources")
            .WithOpenApi();

        // Index URLs endpoint
        group.MapPost("/index", IndexUrls)
            .WithName("IndexUrls")
            .WithSummary("Index a list of URLs for semantic search")
            .Produces<IndexUrlsResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);

        // Search web content endpoint
        group.MapGet("/search/{query}", SearchWebContent)
            .WithName("SearchWebContent")
            .WithSummary("Search indexed web content")
            .Produces<DataSourcesSearchResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        // Get indexed URLs endpoint
        group.MapGet("/indexed", GetIndexedUrls)
            .WithName("GetIndexedUrls")
            .WithSummary("Get all indexed URLs")
            .Produces<List<IndexedUrl>>(StatusCodes.Status200OK);

        // Health check endpoint
        group.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "DataSources" }))
            .WithName("DataSourcesHealth")
            .WithSummary("Health check for DataSources service")
            .Produces(StatusCodes.Status200OK);
    }

    /// <summary>
    /// Index URLs for semantic search
    /// </summary>
    private static async Task<IResult> IndexUrls(
        IndexUrlsRequest request, 
        DataSourcesMemoryContext memoryContext,
        ILogger<DataSourcesMemoryContext> logger)
    {
        try
        {
            logger.LogInformation("Received request to index {Count} URLs", request.Urls.Count);

            // Validate URLs
            var validUrls = new List<string>();
            var errors = new List<string>();

            foreach (var url in request.Urls)
            {
                if (string.IsNullOrWhiteSpace(url))
                {
                    errors.Add("Empty URL provided");
                    continue;
                }

                if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) || 
                    (uri.Scheme != "http" && uri.Scheme != "https"))
                {
                    errors.Add($"Invalid URL format: {url}");
                    continue;
                }

                validUrls.Add(url);
            }

            if (validUrls.Count == 0)
            {
                return Results.BadRequest(new IndexUrlsResponse(new List<IndexedUrl>(), errors));
            }

            // Index the URLs
            var indexedUrls = await memoryContext.IndexUrlsAsync(validUrls);

            var response = new IndexUrlsResponse(indexedUrls, errors);

            logger.LogInformation("Indexing completed. Success: {Success}, Errors: {Errors}", 
                indexedUrls.Count(u => u.Success), errors.Count);

            return Results.Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error indexing URLs");
            return Results.Problem("Internal server error occurred while indexing URLs");
        }
    }

    /// <summary>
    /// Search indexed web content
    /// </summary>
    private static async Task<IResult> SearchWebContent(
        string query,
        DataSourcesMemoryContext memoryContext,
        ILogger<DataSourcesMemoryContext> logger)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return Results.BadRequest("Query cannot be empty");
            }

            logger.LogInformation("Searching web content for query: {Query}", query);

            var searchResponse = await memoryContext.SearchAsync(query);
            
            if (string.IsNullOrEmpty(searchResponse.Response))
            {
                return Results.NotFound(new DataSourcesSearchResponse 
                { 
                    Response = "No relevant content found"
                });
            }

            return Results.Ok(searchResponse);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error searching web content for query: {Query}", query);
            return Results.Problem("Internal server error occurred during search");
        }
    }

    /// <summary>
    /// Get all indexed URLs
    /// </summary>
    private static async Task<IResult> GetIndexedUrls(
        DataSourcesMemoryContext memoryContext,
        ILogger<DataSourcesMemoryContext> logger)
    {
        try
        {
            logger.LogInformation("Getting all indexed URLs");

            var indexedUrls = await memoryContext.GetIndexedUrlsAsync();
            
            return Results.Ok(indexedUrls);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting indexed URLs");
            return Results.Problem("Internal server error occurred while getting indexed URLs");
        }
    }
}