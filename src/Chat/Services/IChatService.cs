using SearchEntities;

namespace Chat.Services;

public interface IChatService
{
    Task<ChatResponse> SendMessageAsync(ChatMessage message, CancellationToken cancellationToken = default);
    Task<ChatSession> GetSessionAsync(string sessionId, CancellationToken cancellationToken = default);
    Task<List<ChatSession>> GetActiveSessionsAsync(CancellationToken cancellationToken = default);
}