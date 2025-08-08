using SearchEntities;

namespace Search.Services;

public interface INlWebClient
{
    Task<SearchResponse> QueryAsync(string query, int top = 10, int skip = 0, CancellationToken cancellationToken = default);
    Task<ReindexResponse> ReindexAsync(string? siteBaseUrl = null, bool force = false, CancellationToken cancellationToken = default);
    
    // Chat methods
    Task<ChatResponse> ChatAsync(string message, string? sessionId = null, ChatContext? context = null, CancellationToken cancellationToken = default);
    Task<ChatSession> GetChatSessionAsync(string sessionId, CancellationToken cancellationToken = default);
}