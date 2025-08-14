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

    public SearchFunction(ILogger<SearchFunction> logger, ISemanticSearchRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    [Function("SemanticSearch")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "semanticsearch")] HttpRequestData req,
        CancellationToken cancellationToken = default)
    {
        var traceId = System.Diagnostics.Activity.Current?.Id ?? Guid.NewGuid().ToString();
        
        try
        {
            _logger.LogInformation("Semantic search request received. TraceId: {TraceId}", traceId);

            // Parse request body
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync(cancellationToken);
            
            if (string.IsNullOrWhiteSpace(requestBody))
            {
                return await CreateErrorResponse(req, HttpStatusCode.BadRequest, "Request body is required", traceId);
            }

            SearchRequest? searchRequest;
            try
            {
                searchRequest = JsonSerializer.Deserialize<SearchRequest>(requestBody, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (JsonException ex)
            {
                _logger.LogWarning(ex, "Invalid JSON in request. TraceId: {TraceId}", traceId);
                return await CreateErrorResponse(req, HttpStatusCode.BadRequest, "Invalid JSON format", traceId);
            }

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

            // Execute search
            var results = await _repository.SearchAsync(searchRequest.Query, top, cancellationToken);

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
            
            _logger.LogInformation("Semantic search completed successfully. Found {Count} results. TraceId: {TraceId}", 
                response.Results.Count, traceId);

            return httpResponse;
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