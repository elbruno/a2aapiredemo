using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using SearchEntities;

namespace SemanticSearchFunction.Repositories;

public class SqlSemanticSearchRepository
{
    private readonly ILogger<SqlSemanticSearchRepository> _logger;
    private readonly IEmbeddingGenerator<string, Embedding<float>> _embeddingGenerator;
    private readonly int _dimensions = 1536;
    private readonly Context _db;

    public SqlSemanticSearchRepository(
        IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator,
        int dimensions,
        Context db,
        ILogger<SqlSemanticSearchRepository> logger)
    {
        _logger = logger;
        _embeddingGenerator = embeddingGenerator ?? throw new ArgumentNullException(nameof(embeddingGenerator));
        _dimensions = dimensions;
        _db = db ?? throw new ArgumentNullException(nameof(db));
    }

    public async Task<SearchResponse> SearchAsync(SearchRequest searchRequest, CancellationToken cancellationToken = default)
    {
        var response = new SearchResponse();

        // Generate embedding for query using the provided delegate
        var queryVector = await _embeddingGenerator.GenerateVectorAsync(searchRequest.query, new() { Dimensions = _dimensions });

        var vectorSearch = queryVector.ToArray();
        var products = await _db.Product
            .OrderBy(p => EF.Functions.VectorDistance("cosine", p.Embedding, vectorSearch))
            .Take(3)
            .ToListAsync();

        response.Products = products;
        response.Response = products.Count > 0 ?
                $"{products.Count} Products found for [{searchRequest.query}]" :
                $"No products found for [{searchRequest.query}]";       
        return response;
    }
}