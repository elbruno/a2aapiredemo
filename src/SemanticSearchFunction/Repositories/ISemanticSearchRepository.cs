using SemanticSearchFunction.Models;

namespace SemanticSearchFunction.Repositories;

public interface ISemanticSearchRepository
{
    Task<IEnumerable<SearchResult>> SearchAsync(string query, int top, CancellationToken cancellationToken = default);
}