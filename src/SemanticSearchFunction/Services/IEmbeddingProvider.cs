using System.Threading;
using System.Threading.Tasks;

namespace SemanticSearchFunction.Services;

public interface IEmbeddingProvider
{
    /// <summary>
    /// True when an embedding backend is available (either registered IEmbeddingGenerator or Azure OpenAI settings present).
    /// </summary>
    bool IsAvailable { get; }

    /// <summary>
    /// Generate an embedding vector for the provided input. Returns a float[] of the embedding.
    /// Returns null if embedding provider not available.
    /// </summary>
    Task<float[]?> GenerateEmbeddingAsync(string input, int dimensions, CancellationToken cancellationToken = default);
}
