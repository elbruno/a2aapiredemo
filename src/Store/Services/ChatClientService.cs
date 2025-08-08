using Microsoft.AspNetCore.SignalR.Client;
using SearchEntities;
using System.Text.Json;

namespace Store.Services;

public interface IChatClientService
{
    Task<ChatResponse> SendMessageAsync(string message, string? sessionId = null, ChatContext? context = null);
    Task StartConnectionAsync();
    Task StopConnectionAsync();
    event Action<ChatResponse>? MessageReceived;
    event Action<bool>? ConnectionStateChanged;
    bool IsConnected { get; }
    string? CurrentSessionId { get; }
}

public class ChatClientService : IChatClientService, IAsyncDisposable
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ChatClientService> _logger;
    private HubConnection? _hubConnection;
    private bool _disposed = false;

    public event Action<ChatResponse>? MessageReceived;
    public event Action<bool>? ConnectionStateChanged;

    public bool IsConnected => _hubConnection?.State == HubConnectionState.Connected;
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
            // Use existing session ID or generate new one
            sessionId ??= CurrentSessionId ?? Guid.NewGuid().ToString();
            CurrentSessionId = sessionId;

            var chatMessage = new ChatMessage
            {
                Message = message,
                SessionId = sessionId,
                Context = context
            };

            _logger.LogInformation("Sending chat message: {Message}, SessionId: {SessionId}", message, sessionId);

            // Send message to Chat API
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
                    
                    // Update current session ID
                    CurrentSessionId = chatResponse.SessionId;
                    
                    return chatResponse;
                }
            }

            throw new HttpRequestException($"Chat API returned {response.StatusCode}: {await response.Content.ReadAsStringAsync()}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send chat message: {Message}", message);
            
            // Return error response
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

    public async Task StartConnectionAsync()
    {
        if (_hubConnection != null && IsConnected)
        {
            return;
        }

        try
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl("https://chat/chat-hub")
                .WithAutomaticReconnect()
                .Build();

            // Subscribe to hub events
            _hubConnection.On<ChatResponse>("ReceiveMessage", (response) =>
            {
                _logger.LogInformation("Received message from hub: SessionId {SessionId}", response.SessionId);
                MessageReceived?.Invoke(response);
            });

            _hubConnection.On<string, bool>("TypingIndicator", (connectionId, isTyping) =>
            {
                _logger.LogDebug("Typing indicator: {ConnectionId} is typing: {IsTyping}", connectionId, isTyping);
                // Could implement typing indicators here
            });

            // Subscribe to connection state changes
            _hubConnection.Closed += async (error) =>
            {
                _logger.LogWarning("SignalR connection closed: {Error}", error?.Message);
                ConnectionStateChanged?.Invoke(false);
                
                // Attempt to reconnect after delay
                await Task.Delay(5000);
                if (!_disposed)
                {
                    await StartConnectionAsync();
                }
            };

            _hubConnection.Reconnected += (connectionId) =>
            {
                _logger.LogInformation("SignalR reconnected: {ConnectionId}", connectionId);
                ConnectionStateChanged?.Invoke(true);
                
                // Rejoin session if we have one
                if (!string.IsNullOrEmpty(CurrentSessionId))
                {
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            await _hubConnection.InvokeAsync("JoinSession", CurrentSessionId);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Failed to rejoin session after reconnect");
                        }
                    });
                }
                
                return Task.CompletedTask;
            };

            await _hubConnection.StartAsync();
            _logger.LogInformation("SignalR connection started");
            ConnectionStateChanged?.Invoke(true);

            // Join session if we have one
            if (!string.IsNullOrEmpty(CurrentSessionId))
            {
                await _hubConnection.InvokeAsync("JoinSession", CurrentSessionId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start SignalR connection");
            ConnectionStateChanged?.Invoke(false);
        }
    }

    public async Task StopConnectionAsync()
    {
        if (_hubConnection != null)
        {
            try
            {
                // Leave session before disconnecting
                if (!string.IsNullOrEmpty(CurrentSessionId))
                {
                    await _hubConnection.InvokeAsync("LeaveSession", CurrentSessionId);
                }

                await _hubConnection.StopAsync();
                _logger.LogInformation("SignalR connection stopped");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error stopping SignalR connection");
            }
            finally
            {
                ConnectionStateChanged?.Invoke(false);
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (!_disposed)
        {
            _disposed = true;
            await StopConnectionAsync();
            
            if (_hubConnection != null)
            {
                await _hubConnection.DisposeAsync();
                _hubConnection = null;
            }
        }
    }
}