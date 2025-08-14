using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SemanticSearchFunction.Models;

namespace SemanticSearchFunction.Repositories;

public class SqlSemanticSearchRepository
{
    private readonly string _connectionString;
    private readonly ILogger<SqlSemanticSearchRepository> _logger;
    private readonly IEmbeddingGenerator<string, Embedding<float>> _embeddingGenerator;
    private readonly int _dimensions = 1536;

    public SqlSemanticSearchRepository(
        IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator,
        int dimensions,
        IConfiguration configuration, 
        ILogger<SqlSemanticSearchRepository> logger)
    {
        _connectionString = configuration.GetConnectionString("productsDb")
            ?? throw new InvalidOperationException("Connection string 'productsDb' not found.");
        _logger = logger;
        _embeddingGenerator = embeddingGenerator ?? throw new ArgumentNullException(nameof(embeddingGenerator));
        _dimensions = dimensions;
    }

    public async Task<IEnumerable<SearchResult>> SearchAsync(string query, int top, CancellationToken cancellationToken = default)
    {
        var results = new List<SearchResult>();

                // Generate embedding for query using the provided delegate
                var queryVector = await _embeddingGenerator.GenerateVectorAsync(query, new() { Dimensions = _dimensions });


        return results;
    }

}