using System.Threading;
using System.Threading.Tasks;

namespace SemanticSearchFunction.Services;

public class NullEmbeddingProvider : IEmbeddingProvider
{
    public bool IsAvailable => false;

    public Task<float[]?> GenerateEmbeddingAsync(string input, int dimensions, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<float[]?>(null);
    }
}
