using Search.Services;
using SearchEntities;

namespace Chat.Services;

public class ChatService : IChatService
{
    private readonly INlWebClient _nlWebClient;
    private readonly ILogger<ChatService> _logger;

    public ChatService(INlWebClient nlWebClient, ILogger<ChatService> logger)
    {
        _nlWebClient = nlWebClient;
        _logger = logger;
    }

    public async Task<ChatResponse> SendMessageAsync(ChatMessage message, CancellationToken cancellationToken = default)
    {
        var correlationId = Guid.NewGuid().ToString();
        _logger.LogInformation("Processing chat message: sessionId={SessionId}, correlationId={CorrelationId}",
            message.SessionId, correlationId);

        try
        {
            // Call NLWeb for conversational response
            var response = await _nlWebClient.ChatAsync(
                message.Message,
                message.SessionId,
                message.Context,
                cancellationToken);

            _logger.LogInformation("Chat response generated: sessionId={SessionId}, responseTime={ResponseTime}ms",
                response.SessionId, response.Metadata.ResponseTime);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process chat message: sessionId={SessionId}, correlationId={CorrelationId}",
                message.SessionId, correlationId);
            throw;
        }
    }

    public async Task<ChatSession> GetSessionAsync(string sessionId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving chat session: {SessionId}", sessionId);

        try
        {
            return await _nlWebClient.GetChatSessionAsync(sessionId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve chat session: {SessionId}", sessionId);
            throw;
        }
    }

    public async Task<List<ChatSession>> GetActiveSessionsAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving active chat sessions");

        // For demo purposes, return empty list (would be implemented with proper storage)
        await Task.Delay(50, cancellationToken);
        return new List<ChatSession>();
    }
}