using System.Threading;
using System.Threading.Tasks;

namespace SemanticSearchFunction.Services;

// Stub provider kept for future wiring to Azure OpenAI. Currently disabled to avoid hard dependency on Azure SDK types.
public class AzureOpenAiEmbeddingProvider : IEmbeddingProvider
{
    public bool IsAvailable => false;

    public Task<float[]?> GenerateEmbeddingAsync(string input, int dimensions, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<float[]?>(null);
    }
}
