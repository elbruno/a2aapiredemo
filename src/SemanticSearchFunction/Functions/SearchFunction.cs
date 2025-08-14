using System.Net;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using SemanticSearchFunction.Models;
using SemanticSearchFunction.Repositories;

namespace SemanticSearchFunction.Functions;

public class SearchFunction
{
    private readonly ILogger<SearchFunction> _logger;
    private readonly ISemanticSearchRepository _repository;
    private readonly SemanticSearchFunction.Services.IEmbeddingProvider _embeddingProvider;

    public SearchFunction(ILogger<SearchFunction> logger, ISemanticSearchRepository repository, SemanticSearchFunction.Services.IEmbeddingProvider embeddingProvider)
    {
        _logger = logger;
        _repository = repository;
        _embeddingProvider = embeddingProvider;
    }

    [Function("SemanticSearch")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "semanticsearch")] HttpRequestData req,
        CancellationToken cancellationToken = default)
    {
        var traceId = System.Diagnostics.Activity.Current?.Id ?? Guid.NewGuid().ToString();

        try
        {
            _logger.LogInformation("Semantic search request received. TraceId: {TraceId}", traceId);

            var queryParameters = req.Query;
            SearchRequest searchRequest = new()
            {
                Query = queryParameters["query"]
            };

            int.TryParse(queryParameters["top"], out int topParam);
            searchRequest.Top = topParam;

            if (searchRequest == null || string.IsNullOrWhiteSpace(searchRequest.Query))
            {
                return await CreateErrorResponse(req, HttpStatusCode.BadRequest, "Query parameter is required", traceId);
            }

            var top = searchRequest.Top ?? 10;
            if (top <= 0 || top > 50)
            {
                return await CreateErrorResponse(req, HttpStatusCode.BadRequest, "Top parameter must be between 1 and 50", traceId);
            }

            _logger.LogInformation("Executing semantic search for query: '{Query}', top: {Top}, TraceId: {TraceId}",
                searchRequest.Query, top, traceId);

            // Execute search. If an embedding provider is available, pass a delegate that uses it.
            if (_embeddingProvider?.IsAvailable == true)
            {
                var results = await _repository.SearchAsync(searchRequest.Query, top, async (q, dims, ct) =>
                {
                    var v = await _embeddingProvider.GenerateEmbeddingAsync(q, dims, ct);
                    return v ?? Array.Empty<float>();
                }, 1536, cancellationToken);

                var response = new SearchResponse
                {
                    Results = results.ToList(),
                    TraceId = traceId
                };

                // Create successful response
                var httpResponse = req.CreateResponse(HttpStatusCode.OK);
                httpResponse.Headers.Add("Content-Type", "application/json");
                var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
                await httpResponse.WriteStringAsync(jsonResponse, cancellationToken);

                _logger.LogInformation("Semantic search completed successfully. Found {Count} results. TraceId: {TraceId}", response.Results.Count, traceId);
                return httpResponse;
            }

            // No embedding provider available: call repository without embeddings
            var resultsNoEmbedding = await _repository.SearchAsync(searchRequest.Query, top, cancellationToken);

            var responseNoEmbedding = new SearchResponse
            {
                Results = resultsNoEmbedding.ToList(),
                TraceId = traceId
            };

            // Create successful response
            var httpResponseNoEmbedding = req.CreateResponse(HttpStatusCode.OK);
            httpResponseNoEmbedding.Headers.Add("Content-Type", "application/json");

            var jsonResponseNoEmbedding = JsonSerializer.Serialize(responseNoEmbedding, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await httpResponseNoEmbedding.WriteStringAsync(jsonResponseNoEmbedding, cancellationToken);

            _logger.LogInformation("Semantic search completed successfully. Found {Count} results. TraceId: {TraceId}",
                responseNoEmbedding.Results.Count, traceId);

            return httpResponseNoEmbedding;
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("Database connectivity"))
        {
            _logger.LogError(ex, "Database connectivity issue. TraceId: {TraceId}", traceId);
            return await CreateErrorResponse(req, HttpStatusCode.ServiceUnavailable, "Database temporarily unavailable", traceId);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Request was cancelled. TraceId: {TraceId}", traceId);
            return await CreateErrorResponse(req, HttpStatusCode.RequestTimeout, "Request timeout", traceId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during semantic search. TraceId: {TraceId}", traceId);
            return await CreateErrorResponse(req, HttpStatusCode.InternalServerError, "Internal server error", traceId);
        }
    }

    private async Task<HttpResponseData> CreateErrorResponse(HttpRequestData req, HttpStatusCode statusCode, string message, string traceId)
    {
        var errorResponse = new
        {
            error = message,
            traceId = traceId
        };

        var httpResponse = req.CreateResponse(statusCode);
        httpResponse.Headers.Add("Content-Type", "application/json");

        var jsonResponse = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await httpResponse.WriteStringAsync(jsonResponse);
        return httpResponse;
    }
}