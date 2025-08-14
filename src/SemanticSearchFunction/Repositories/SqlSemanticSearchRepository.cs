using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SemanticSearchFunction.Models;

namespace SemanticSearchFunction.Repositories;

public class SqlSemanticSearchRepository : ISemanticSearchRepository
{
    private readonly string _connectionString;
    private readonly ILogger<SqlSemanticSearchRepository> _logger;
    private readonly int _defaultTop;

    public SqlSemanticSearchRepository(IConfiguration configuration, ILogger<SqlSemanticSearchRepository> logger)
    {
        _connectionString = configuration.GetConnectionString("productsDb") 
            ?? throw new InvalidOperationException("Connection string 'productsDb' not found.");
        _logger = logger;
        _defaultTop = int.Parse(configuration["SEMANTIC_SEARCH_TOP_DEFAULT"] ?? "10");
    }

    public async Task<IEnumerable<SearchResult>> SearchAsync(string query, int top, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return Enumerable.Empty<SearchResult>();
        }

        var actualTop = top > 0 ? Math.Min(top, 50) : _defaultTop; // Cap at 50 for performance
        var results = new List<SearchResult>();

        try
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            // Use text similarity search for now - can be enhanced with vector search later
            var sql = @"
                SELECT TOP(@top) 
                    p.Id, 
                    p.Name as Title, 
                    p.Description,
                    p.Price,
                    CASE 
                        WHEN p.Name LIKE '%' + @query + '%' THEN 0.9
                        WHEN p.Description LIKE '%' + @query + '%' THEN 0.7
                        ELSE 0.5
                    END AS Score
                FROM Product p
                WHERE p.Name LIKE '%' + @query + '%' 
                   OR p.Description LIKE '%' + @query + '%'
                ORDER BY Score DESC, p.Id";

            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@top", actualTop);
            command.Parameters.AddWithValue("@query", query);
            command.CommandTimeout = 15; // 15 second timeout

            using var reader = await command.ExecuteReaderAsync(cancellationToken);
            
            while (await reader.ReadAsync(cancellationToken))
            {
                var result = new SearchResult
                {
                    Id = reader.GetInt32(0), // Id
                    Title = reader.GetString(1), // Title 
                    Score = reader.GetDouble(4), // Score
                    Snippet = TruncateDescription(reader.GetString(2), 100), // Description
                    Metadata = new Dictionary<string, string>
                    {
                        ["price"] = reader.GetDecimal(3).ToString("C"), // Price
                        ["source"] = "products"
                    }
                };
                results.Add(result);
            }

            _logger.LogInformation("Semantic search for '{Query}' returned {Count} results", query, results.Count);
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "SQL error during semantic search for query '{Query}'", query);
            throw new InvalidOperationException("Database connectivity issue", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during semantic search for query '{Query}'", query);
            throw;
        }

        return results;
    }

    private static string TruncateDescription(string description, int maxLength)
    {
        if (string.IsNullOrEmpty(description) || description.Length <= maxLength)
            return description;

        var truncated = description.Substring(0, maxLength);
        var lastSpace = truncated.LastIndexOf(' ');
        
        return lastSpace > 0 ? truncated.Substring(0, lastSpace) + "..." : truncated + "...";
    }
}