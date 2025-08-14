using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using OpenAI.Embeddings;

namespace SemanticSearchFunction.Services;

public class OpenAiEmbeddingProvider : IEmbeddingProvider
{
    private readonly IEmbeddingGenerator<string, Embedding<float>> _generator;
    private readonly ILogger<OpenAiEmbeddingProvider> _logger;

    public OpenAiEmbeddingProvider(IEmbeddingGenerator<string, Embedding<float>> generator, ILogger<OpenAiEmbeddingProvider> logger)
    {
        _generator = generator;
        _logger = logger;
    }

    public bool IsAvailable => _generator is not null;

    public async Task<float[]?> GenerateEmbeddingAsync(string input, int dimensions, CancellationToken cancellationToken = default)
    {
        if (!IsAvailable)
            return null;

        var vec = await _generator.GenerateVectorAsync(input, new() { Dimensions = dimensions });
        return vec.ToArray();
    }
}
