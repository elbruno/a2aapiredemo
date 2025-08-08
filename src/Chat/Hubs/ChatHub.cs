using Microsoft.AspNetCore.SignalR;
using SearchEntities;

namespace Chat.Hubs;

public class ChatHub : Hub
{
    private readonly ILogger<ChatHub> _logger;

    public ChatHub(ILogger<ChatHub> logger)
    {
        _logger = logger;
    }

    public async Task JoinSession(string sessionId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, sessionId);
        _logger.LogInformation("Connection {ConnectionId} joined session {SessionId}", Context.ConnectionId, sessionId);
        
        await Clients.Group(sessionId).SendAsync("UserJoined", Context.ConnectionId);
    }

    public async Task LeaveSession(string sessionId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, sessionId);
        _logger.LogInformation("Connection {ConnectionId} left session {SessionId}", Context.ConnectionId, sessionId);
        
        await Clients.Group(sessionId).SendAsync("UserLeft", Context.ConnectionId);
    }

    public async Task TypingIndicator(string sessionId, bool isTyping)
    {
        await Clients.Group(sessionId).SendAsync("TypingIndicator", Context.ConnectionId, isTyping);
    }

    public async Task SendMessage(string sessionId, string message)
    {
        _logger.LogInformation("Received message in session {SessionId}: {Message}", sessionId, message);
        
        // Echo message to all clients in the session (will be handled by the API endpoint)
        await Clients.Group(sessionId).SendAsync("MessageReceived", Context.ConnectionId, message);
    }

    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation("Client connected: {ConnectionId}", Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation("Client disconnected: {ConnectionId}, Exception: {Exception}", 
            Context.ConnectionId, exception?.Message);
        await base.OnDisconnectedAsync(exception);
    }
}