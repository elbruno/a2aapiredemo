using Search.Services;
using SearchEntities;
using Microsoft.AspNetCore.Mvc;

public class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add Aspire service defaults
        builder.AddServiceDefaults();

        // Add services to the container
        builder.Services.AddOpenApi();

        // Configure HttpClient with resilience and service discovery
        builder.Services.AddHttpClient<INlWebClient, NlWebNetClient>("nlweb", (serviceProvider, client) =>
        {
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            var timeout = configuration.GetValue<TimeSpan?>("NLWeb:Timeout") ?? TimeSpan.FromSeconds(30);
            client.Timeout = timeout;
            client.DefaultRequestHeaders.Add("User-Agent", "eShopLite-Search/1.0");
        });

        // Register NLWebNet client (real implementation)
        builder.Services.AddScoped<INlWebClient, NlWebNetClient>();

        // Add CORS
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowStore", policy =>
            {
                policy.WithOrigins("https://localhost:7147", "https://store") // Allow Store service
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });
        });

        // Add rate limiting
        builder.Services.AddRateLimiter(options =>
        {
            options.GlobalLimiter = System.Threading.RateLimiting.PartitionedRateLimiter.Create<HttpContext, string>(
                httpContext => System.Threading.RateLimiting.RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: httpContext.User.Identity?.Name ?? httpContext.Request.Headers.Host.ToString(),
                    factory: partition => new System.Threading.RateLimiting.FixedWindowRateLimiterOptions
                    {
                        AutoReplenishment = true,
                        PermitLimit = 100,
                        Window = TimeSpan.FromMinutes(1)
                    }));
        });

        var app = builder.Build();

        // Map Aspire default endpoints
        app.MapDefaultEndpoints();

        // Configure the HTTP request pipeline
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();
        app.UseCors("AllowStore");
        app.UseRateLimiter();

        // Search API endpoint
        app.MapGet("/api/v1/search", async (HttpContext context, INlWebClient nlWebClient, ILogger<Program> logger) =>
        {
            var correlationId = Guid.NewGuid().ToString();
            var query = context.Request.Query;

            var q = query["q"].ToString();
            var topStr = query["top"].ToString();
            var skipStr = query["skip"].ToString();

            // Parse with defaults
            int top = 10;
            int skip = 0;

            if (!string.IsNullOrEmpty(topStr) && !int.TryParse(topStr, out top))
            {
                top = 10;
            }

            if (!string.IsNullOrEmpty(skipStr) && !int.TryParse(skipStr, out skip))
            {
                skip = 0;
            }

            logger.LogInformation("Search request: query={Query}, top={Top}, skip={Skip}, correlationId={CorrelationId}",
                q, top, skip, correlationId);

            try
            {
                // Input validation
                if (string.IsNullOrWhiteSpace(q))
                {
                    return Results.BadRequest(new SearchErrorResponse
                    {
                        Code = "INVALID_QUERY",
                        Message = "Query parameter 'q' is required and cannot be empty",
                        CorrelationId = correlationId
                    });
                }

                if (q.Length > 1000)
                {
                    return Results.BadRequest(new SearchErrorResponse
                    {
                        Code = "QUERY_TOO_LONG",
                        Message = "Query cannot exceed 1000 characters",
                        CorrelationId = correlationId
                    });
                }

                if (top < 1 || top > 50)
                {
                    return Results.BadRequest(new SearchErrorResponse
                    {
                        Code = "INVALID_TOP",
                        Message = "Top parameter must be between 1 and 50",
                        CorrelationId = correlationId
                    });
                }

                if (skip < 0)
                {
                    return Results.BadRequest(new SearchErrorResponse
                    {
                        Code = "INVALID_SKIP",
                        Message = "Skip parameter must be >= 0",
                        CorrelationId = correlationId
                    });
                }

                // Call NLWeb client
                var response = await nlWebClient.QueryAsync(q, top, skip, context.RequestAborted);

                logger.LogInformation("Search completed: count={Count}, correlationId={CorrelationId}",
                    response.Count, correlationId);

                return Results.Ok(response);
            }
            catch (OperationCanceledException)
            {
                logger.LogWarning("Search request cancelled: correlationId={CorrelationId}", correlationId);
                return Results.StatusCode(499);
            }
            catch (TimeoutException)
            {
                logger.LogError("Search request timeout: correlationId={CorrelationId}", correlationId);
                return Results.Problem(
                    statusCode: 502,
                    detail: "NLWeb service timeout",
                    title: "NLWEB_TIMEOUT",
                    instance: $"correlation:{correlationId}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Search request failed: correlationId={CorrelationId}", correlationId);
                return Results.Problem(
                    statusCode: 500,
                    detail: "An internal error occurred",
                    title: "INTERNAL_ERROR",
                    instance: $"correlation:{correlationId}");
            }
        })
        .WithName("Search")
        .WithSummary("Search the site using natural language")
        .WithOpenApi();

        // Reindex API endpoint (protected)
        app.MapPost("/api/v1/search/reindex", async (
            ReindexRequest? request,
            INlWebClient nlWebClient,
            ILogger<Program> logger,
            HttpContext context) =>
        {
            var correlationId = Guid.NewGuid().ToString();
            logger.LogInformation("Reindex request: siteBaseUrl={SiteBaseUrl}, force={Force}, correlationId={CorrelationId}",
                request?.SiteBaseUrl, request?.Force, correlationId);

            try
            {
                // TODO: Add authentication/authorization here
                // For now, we'll proceed without auth for demo purposes

                var response = await nlWebClient.ReindexAsync(request?.SiteBaseUrl, request?.Force ?? false, context.RequestAborted);

                logger.LogInformation("Reindex started: operationId={OperationId}, correlationId={CorrelationId}",
                    response.OperationId, correlationId);

                return Results.Accepted($"/api/v1/search/reindex/{response.OperationId}", response);
            }
            catch (OperationCanceledException)
            {
                logger.LogWarning("Reindex request cancelled: correlationId={CorrelationId}", correlationId);
                return Results.StatusCode(499);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Reindex request failed: correlationId={CorrelationId}", correlationId);
                return Results.Problem(
                    statusCode: 500,
                    detail: "Reindex operation failed",
                    title: "REINDEX_FAILED",
                    instance: $"correlation:{correlationId}");
            }
        })
        .WithName("Reindex")
        .WithSummary("Trigger a reindex of the site content")
        .WithOpenApi();

        app.Run();
    }
}

// Make Program class visible to tests
public partial class SearchProgram { }
