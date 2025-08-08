# Chat API Reference

The eShopLite Chat API provides real-time conversational capabilities powered by NLWeb integration. This API supports both RESTful endpoints and SignalR real-time communication.

## Base URL

```
https://your-domain.com/api/v1/chat
```

## Authentication

Currently, the Chat API operates without authentication for demonstration purposes. In production deployments, implement appropriate authentication mechanisms.

## Rate Limiting

- **Limit:** 100 requests per minute per client
- **Headers:** Standard rate limiting headers are included in responses
  - `X-RateLimit-Limit`: Maximum requests per window
  - `X-RateLimit-Remaining`: Remaining requests in current window
  - `X-RateLimit-Reset`: Time when the rate limit resets

## Content Type

All API endpoints accept and return `application/json` unless otherwise specified.

## Error Handling

All errors follow a consistent format:

```json
{
  "code": "ERROR_CODE",
  "message": "Human-readable error description",
  "correlationId": "unique-request-identifier"
}
```

### Common Error Codes

| Code | HTTP Status | Description |
|------|-------------|-------------|
| `INVALID_MESSAGE` | 400 | Message content is empty or invalid |
| `MESSAGE_TOO_LONG` | 400 | Message exceeds 5000 character limit |
| `INVALID_SESSION_ID` | 400 | Session ID format is invalid |
| `SESSION_NOT_FOUND` | 404 | Chat session does not exist |
| `RATE_LIMIT_EXCEEDED` | 429 | Too many requests |
| `NLWEB_TIMEOUT` | 502 | NLWeb service timeout |
| `INTERNAL_ERROR` | 500 | Unexpected server error |

## REST API Endpoints

### Send Chat Message

Send a message to the chat assistant and receive an AI-generated response.

#### Request

```http
POST /api/v1/chat/message
Content-Type: application/json
```

```json
{
  "message": "What running shoes do you recommend for beginners?",
  "sessionId": "session-uuid-123",
  "context": {
    "currentPage": "/products",
    "userPreferences": {
      "budget": "under-100",
      "activity": "running"
    }
  }
}
```

#### Parameters

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| `message` | string | Yes | User message (1-5000 characters) |
| `sessionId` | string | No | Session identifier (UUID format) |
| `context` | object | No | Additional context for better responses |
| `context.currentPage` | string | No | Current page URL |
| `context.userPreferences` | object | No | User preferences and filters |

#### Response

```http
HTTP/1.1 200 OK
Content-Type: application/json
```

```json
{
  "sessionId": "session-uuid-123",
  "response": "For beginners, I'd recommend starting with our comfort-focused running shoes. The Trail Runner Comfort series offers excellent cushioning and support, starting at $59.99. They're perfect for building up your running routine without breaking the bank.",
  "suggestedActions": [
    {
      "text": "View Beginner Running Shoes",
      "url": "/products/category/running-beginner"
    },
    {
      "text": "Read Running Guide",
      "url": "/guides/running-basics"
    }
  ],
  "metadata": {
    "responseTime": 850,
    "confidence": 0.92,
    "sources": ["/products/running", "/guides/running-basics"]
  }
}
```

#### Response Fields

| Field | Type | Description |
|-------|------|-------------|
| `sessionId` | string | Session identifier for conversation continuity |
| `response` | string | AI-generated response text |
| `suggestedActions` | array | List of suggested actions/links |
| `suggestedActions[].text` | string | Display text for the action |
| `suggestedActions[].url` | string | URL for the action |
| `metadata` | object | Response metadata |
| `metadata.responseTime` | number | Response generation time (ms) |
| `metadata.confidence` | number | Confidence score (0.0-1.0) |
| `metadata.sources` | array | Source pages used for response |

### Get Chat Session

Retrieve chat session history and metadata.

#### Request

```http
GET /api/v1/chat/session/{sessionId}
```

#### Parameters

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `sessionId` | string | Yes | Session identifier (UUID format) |

#### Response

```http
HTTP/1.1 200 OK
Content-Type: application/json
```

```json
{
  "id": "session-uuid-123",
  "createdAt": "2024-01-15T10:30:00Z",
  "lastActivity": "2024-01-15T10:35:23Z",
  "messages": [
    {
      "message": "What running shoes do you recommend?",
      "sessionId": "session-uuid-123",
      "context": {
        "currentPage": "/products"
      }
    }
  ],
  "context": {
    "currentPage": "/products",
    "userPreferences": {
      "budget": "under-100",
      "activity": "running"
    }
  }
}
```

## SignalR Real-Time API

The Chat API supports real-time communication via SignalR for instant messaging experiences.

### Hub Connection

#### Connection URL
```
wss://your-domain.com/chat-hub
```

#### Connection Setup (JavaScript)

```javascript
import { HubConnectionBuilder } from '@microsoft/signalr';

const connection = new HubConnectionBuilder()
    .withUrl('/chat-hub')
    .withAutomaticReconnect()
    .build();

// Start connection
await connection.start();
```

### Hub Methods

#### Join Session

Join a chat session to receive real-time updates.

```javascript
await connection.invoke('JoinSession', sessionId);
```

**Parameters:**
- `sessionId` (string): Session identifier

#### Leave Session

Leave a chat session.

```javascript
await connection.invoke('LeaveSession', sessionId);
```

**Parameters:**
- `sessionId` (string): Session identifier

#### Send Message

Send a message through the real-time connection.

```javascript
await connection.invoke('SendMessage', sessionId, message);
```

**Parameters:**
- `sessionId` (string): Session identifier
- `message` (string): Message content

#### Typing Indicator

Send typing status to other clients in the session.

```javascript
await connection.invoke('TypingIndicator', sessionId, isTyping);
```

**Parameters:**
- `sessionId` (string): Session identifier
- `isTyping` (boolean): Whether user is currently typing

### Hub Events

#### Receive Message

Fired when a new message is received in the session.

```javascript
connection.on('ReceiveMessage', (response) => {
    console.log('New message:', response);
    // response follows the same format as REST API response
});
```

#### Message Received

Fired when a message is acknowledged by the hub.

```javascript
connection.on('MessageReceived', (connectionId, message) => {
    console.log('Message acknowledged:', message);
});
```

#### Typing Indicator

Fired when someone in the session starts/stops typing.

```javascript
connection.on('TypingIndicator', (connectionId, isTyping) => {
    console.log('Typing status:', isTyping);
});
```

#### User Joined/Left

Fired when users join or leave the session.

```javascript
connection.on('UserJoined', (connectionId) => {
    console.log('User joined:', connectionId);
});

connection.on('UserLeft', (connectionId) => {
    console.log('User left:', connectionId);
});
```

## Code Examples

### JavaScript/TypeScript Client

```typescript
import { HubConnectionBuilder, HubConnection } from '@microsoft/signalr';

class ChatClient {
    private connection: HubConnection;
    private sessionId: string;

    constructor(private baseUrl: string) {
        this.sessionId = this.generateSessionId();
        this.connection = new HubConnectionBuilder()
            .withUrl(`${baseUrl}/chat-hub`)
            .withAutomaticReconnect()
            .build();
        
        this.setupEventHandlers();
    }

    private setupEventHandlers(): void {
        this.connection.on('ReceiveMessage', (response) => {
            this.onMessageReceived(response);
        });

        this.connection.on('TypingIndicator', (connectionId, isTyping) => {
            this.onTypingIndicator(connectionId, isTyping);
        });
    }

    async connect(): Promise<void> {
        await this.connection.start();
        await this.connection.invoke('JoinSession', this.sessionId);
    }

    async sendMessage(message: string, context?: any): Promise<any> {
        const response = await fetch(`${this.baseUrl}/api/v1/chat/message`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                message,
                sessionId: this.sessionId,
                context
            })
        });

        if (!response.ok) {
            throw new Error(`HTTP ${response.status}: ${response.statusText}`);
        }

        return await response.json();
    }

    async setTyping(isTyping: boolean): Promise<void> {
        await this.connection.invoke('TypingIndicator', this.sessionId, isTyping);
    }

    private onMessageReceived(response: any): void {
        // Handle received message
        console.log('Received:', response);
    }

    private onTypingIndicator(connectionId: string, isTyping: boolean): void {
        // Handle typing indicator
        console.log(`${connectionId} is ${isTyping ? 'typing' : 'not typing'}`);
    }

    private generateSessionId(): string {
        return 'session-' + Math.random().toString(36).substr(2, 9);
    }
}

// Usage
const chatClient = new ChatClient('https://api.eshoplite.com');
await chatClient.connect();

const response = await chatClient.sendMessage('What running shoes do you recommend?', {
    currentPage: '/products',
    userPreferences: { activity: 'running', budget: 'under-100' }
});

console.log(response);
```

### C# Client

```csharp
using Microsoft.AspNetCore.SignalR.Client;
using System.Text.Json;

public class ChatClient : IAsyncDisposable
{
    private readonly HttpClient _httpClient;
    private readonly HubConnection _hubConnection;
    private readonly string _sessionId;

    public ChatClient(string baseUrl)
    {
        _httpClient = new HttpClient { BaseAddress = new Uri(baseUrl) };
        _sessionId = GenerateSessionId();
        
        _hubConnection = new HubConnectionBuilder()
            .WithUrl($"{baseUrl}/chat-hub")
            .WithAutomaticReconnect()
            .Build();
            
        SetupEventHandlers();
    }

    private void SetupEventHandlers()
    {
        _hubConnection.On<ChatResponse>("ReceiveMessage", OnMessageReceived);
        _hubConnection.On<string, bool>("TypingIndicator", OnTypingIndicator);
    }

    public async Task ConnectAsync()
    {
        await _hubConnection.StartAsync();
        await _hubConnection.InvokeAsync("JoinSession", _sessionId);
    }

    public async Task<ChatResponse> SendMessageAsync(string message, object? context = null)
    {
        var request = new
        {
            message,
            sessionId = _sessionId,
            context
        };

        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var response = await _httpClient.PostAsync("/api/v1/chat/message", content);
        response.EnsureSuccessStatusCode();
        
        var responseJson = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<ChatResponse>(responseJson);
    }

    public async Task SetTypingAsync(bool isTyping)
    {
        await _hubConnection.InvokeAsync("TypingIndicator", _sessionId, isTyping);
    }

    private void OnMessageReceived(ChatResponse response)
    {
        Console.WriteLine($"Received: {response.Response}");
    }

    private void OnTypingIndicator(string connectionId, bool isTyping)
    {
        Console.WriteLine($"{connectionId} is {(isTyping ? "typing" : "not typing")}");
    }

    private string GenerateSessionId()
    {
        return $"session-{Guid.NewGuid():N}";
    }

    public async ValueTask DisposeAsync()
    {
        await _hubConnection.DisposeAsync();
        _httpClient.Dispose();
    }
}

// Usage
var chatClient = new ChatClient("https://api.eshoplite.com");
await chatClient.ConnectAsync();

var response = await chatClient.SendMessageAsync("What running shoes do you recommend?", new
{
    currentPage = "/products",
    userPreferences = new { activity = "running", budget = "under-100" }
});

Console.WriteLine(response.Response);
```

## Performance Considerations

### Response Times
- **Target P50:** < 1 second
- **Target P95:** < 3 seconds
- **Timeout:** 30 seconds

### Concurrency
- **Max concurrent connections:** 1000 per instance
- **Max messages per session:** No limit
- **Session timeout:** 30 minutes of inactivity

### Caching
- Session data cached for 30 minutes
- Conversation context maintained during session
- Frequently requested responses may be cached

## Health and Monitoring

### Health Check Endpoint

```http
GET /health
```

Returns service health status and dependencies.

### Metrics

The Chat API exposes metrics for monitoring:
- Message throughput (messages/second)
- Response time percentiles
- Error rates
- Active session count
- SignalR connection count

## Security

### Input Validation
- Message length limited to 5000 characters
- HTML content is sanitized
- SQL injection protection
- XSS prevention

### Rate Limiting
- 100 requests per minute per client IP
- Configurable limits per environment
- Temporary bans for abuse

### CORS Policy
- Restricted to allowed origins
- Credentials support for authenticated requests
- Preflight request handling

---

*For technical support or API questions, please contact the development team or create an issue in the repository.*