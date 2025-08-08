using SearchEntities;

namespace Search.Services;

public interface INlWebClient
{
    Task<SearchResponse> QueryAsync(string query, int top = 10, int skip = 0, CancellationToken cancellationToken = default);
    Task<ReindexResponse> ReindexAsync(string? siteBaseUrl = null, bool force = false, CancellationToken cancellationToken = default);
}