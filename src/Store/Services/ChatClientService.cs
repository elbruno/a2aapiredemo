// ...existing code...
using SearchEntities;
using System.Text.Json;

namespace Store.Services;

public interface IChatClientService
{
    Task<ChatResponse> SendMessageAsync(string message, string? sessionId = null, ChatContext? context = null);
    string? CurrentSessionId { get; }
}

public class ChatClientService : IChatClientService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ChatClientService> _logger;
    public string? CurrentSessionId { get; private set; }

    public ChatClientService(HttpClient httpClient, ILogger<ChatClientService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<ChatResponse> SendMessageAsync(string message, string? sessionId = null, ChatContext? context = null)
    {
        try
        {
            sessionId ??= CurrentSessionId ?? Guid.NewGuid().ToString();
            CurrentSessionId = sessionId;

            var chatMessage = new ChatMessage
            {
                Message = message,
                SessionId = sessionId,
                Context = context
            };

            _logger.LogInformation("Sending chat message: {Message}, SessionId: {SessionId}", message, sessionId);

            var response = await _httpClient.PostAsJsonAsync("https://chat/api/v1/chat/message", chatMessage);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var chatResponse = JsonSerializer.Deserialize<ChatResponse>(jsonResponse, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (chatResponse != null)
                {
                    _logger.LogInformation("Chat response received: {ResponseLength} chars, SessionId: {SessionId}",
                        chatResponse.Response.Length, chatResponse.SessionId);
                    CurrentSessionId = chatResponse.SessionId;
                    return chatResponse;
                }
            }

            throw new HttpRequestException($"Chat API returned {response.StatusCode}: {await response.Content.ReadAsStringAsync()}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send chat message: {Message}", message);
            return new ChatResponse
            {
                SessionId = sessionId ?? CurrentSessionId ?? Guid.NewGuid().ToString(),
                Response = "I'm sorry, I'm having trouble responding right now. Please try again in a moment.",
                Metadata = new ChatMetadata
                {
                    ResponseTime = 0,
                    Confidence = 0.0,
                    Sources = new List<string>()
                }
            };
        }
    }

    // SignalR connection methods removed
}