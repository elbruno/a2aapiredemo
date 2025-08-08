using SearchEntities;

namespace Search.Services;

public class MockNlWebClient : INlWebClient
{
    private readonly ILogger<MockNlWebClient> _logger;
    private readonly HttpClient _httpClient;
    
    // Chat session storage (in-memory for demo)
    private static readonly Dictionary<string, ChatSession> ChatSessions = new();

    // Mock data for demonstration
    private static readonly List<SearchResult> MockResults = new()
    {
        new SearchResult
        {
            Title = "Men's Running Shoes – Budget Picks",
            Url = "/products/123",
            Snippet = "Lightweight trainers ideal for daily runs and casual workouts...",
            Score = 0.87,
            Metadata = new Dictionary<string, object> { { "category", "running" }, { "price", 59.99 } }
        },
        new SearchResult
        {
            Title = "Women's Hiking Boots Collection", 
            Url = "/products/456",
            Snippet = "Waterproof and durable hiking boots for outdoor adventures...",
            Score = 0.82,
            Metadata = new Dictionary<string, object> { { "category", "hiking" }, { "price", 129.99 } }
        },
        new SearchResult
        {
            Title = "Outdoor Gear FAQ - Returns Policy",
            Url = "/support/returns",
            Snippet = "Learn about our 30-day return policy for all outdoor equipment...",
            Score = 0.75,
            Metadata = new Dictionary<string, object> { { "category", "support" }, { "type", "policy" } }
        },
        new SearchResult
        {
            Title = "Trail Running Guide",
            Url = "/guides/trail-running",
            Snippet = "Essential tips and gear recommendations for trail running enthusiasts...",
            Score = 0.68,
            Metadata = new Dictionary<string, object> { { "category", "guides" }, { "type", "content" } }
        }
    };

    private static readonly Dictionary<string, string[]> ChatResponses = new()
    {
        { "greeting", new[] { "Hello! Welcome to eShopLite. How can I help you find the perfect outdoor gear today?", "Hi there! I'm here to help you explore our amazing collection of outdoor equipment. What are you looking for?" } },
        { "running", new[] { "Great choice! For running, I'd recommend our lightweight running shoes starting at $59.99. They're perfect for both beginners and experienced runners.", "Looking for running gear? Our running shoes collection includes budget-friendly options and premium performance shoes. What's your experience level?" } },
        { "hiking", new[] { "Perfect for outdoor adventures! Our hiking boots start at $129.99 and are waterproof and durable for any terrain.", "Hiking gear is our specialty! From boots to backpacks, we have everything you need. Are you planning day hikes or multi-day trips?" } },
        { "price", new[] { "We have options for every budget! Our running shoes start at $59.99, and hiking boots at $129.99. What price range works for you?", "Great question about pricing! We offer budget-friendly options without compromising on quality. Would you like to see our deals?" } },
        { "help", new[] { "I'm here to help! I can assist with product recommendations, sizing, returns policy, or any questions about our outdoor gear.", "Absolutely! Whether you need gear recommendations, want to know about our return policy, or have sizing questions, I'm here to help." } },
        { "return", new[] { "We have a hassle-free 30-day return policy for all outdoor equipment. Items need to be in original condition with tags.", "Our return policy is customer-friendly - 30 days to return any item in original condition. Need help with a specific return?" } }
    };

    public MockNlWebClient(ILogger<MockNlWebClient> logger, HttpClient httpClient)
    {
        _logger = logger;
        _httpClient = httpClient;
    }

    public async Task<SearchResponse> QueryAsync(string query, int top = 10, int skip = 0, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("MockNlWebClient: Processing search query: {Query}, top: {Top}, skip: {Skip}", query, top, skip);

        // Simulate network delay
        await Task.Delay(TimeSpan.FromMilliseconds(100), cancellationToken);

        // Filter results based on query (simple mock logic)
        var filteredResults = MockResults
            .Where(r => string.IsNullOrEmpty(query) || 
                       r.Title.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                       r.Snippet.Contains(query, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(r => r.Score)
            .Skip(skip)
            .Take(top)
            .ToList();

        var response = new SearchResponse
        {
            Query = query,
            Count = filteredResults.Count,
            Results = filteredResults,
            Response = $"Found {filteredResults.Count} results for '{query}'"
        };

        _logger.LogInformation("MockNlWebClient: Returning {Count} results", response.Count);
        return response;
    }

    public async Task<ReindexResponse> ReindexAsync(string? siteBaseUrl = null, bool force = false, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("MockNlWebClient: Reindexing site: {SiteBaseUrl}, force: {Force}", siteBaseUrl, force);

        // Simulate reindex operation
        await Task.Delay(TimeSpan.FromMilliseconds(500), cancellationToken);

        var response = new ReindexResponse
        {
            OperationId = Guid.NewGuid().ToString(),
            Status = "started",
            Message = $"Reindex operation started for {siteBaseUrl ?? "default site"}",
            StartedAt = DateTime.UtcNow
        };

        _logger.LogInformation("MockNlWebClient: Reindex operation started with ID: {OperationId}", response.OperationId);
        return response;
    }

    public async Task<ChatResponse> ChatAsync(string message, string? sessionId = null, ChatContext? context = null, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("MockNlWebClient: Processing chat message: {Message}, sessionId: {SessionId}", message, sessionId);

        var startTime = DateTime.UtcNow;

        // Simulate network delay for realistic experience
        await Task.Delay(TimeSpan.FromMilliseconds(Random.Shared.Next(500, 1200)), cancellationToken);

        // Generate or use existing session ID
        sessionId ??= Guid.NewGuid().ToString();

        // Get or create chat session
        if (!ChatSessions.TryGetValue(sessionId, out var session))
        {
            session = new ChatSession
            {
                Id = sessionId,
                Context = context
            };
            ChatSessions[sessionId] = session;
        }

        // Add user message to session
        session.Messages.Add(new ChatMessage { Message = message, Context = context });
        session.LastActivity = DateTime.UtcNow;

        // Generate response based on message content
        var response = GenerateChatResponse(message, context);
        var suggestedActions = GenerateSuggestedActions(message, context);

        var chatResponse = new ChatResponse
        {
            SessionId = sessionId,
            Response = response,
            SuggestedActions = suggestedActions,
            Metadata = new ChatMetadata
            {
                ResponseTime = (int)(DateTime.UtcNow - startTime).TotalMilliseconds,
                Confidence = Random.Shared.NextDouble() * 0.3 + 0.7, // 0.7-1.0 range
                Sources = GetRelevantSources(message)
            }
        };

        _logger.LogInformation("MockNlWebClient: Chat response generated for session: {SessionId}", sessionId);
        return chatResponse;
    }

    public async Task<ChatSession> GetChatSessionAsync(string sessionId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("MockNlWebClient: Retrieving chat session: {SessionId}", sessionId);
        
        await Task.Delay(50, cancellationToken); // Simulate small delay

        if (ChatSessions.TryGetValue(sessionId, out var session))
        {
            return session;
        }

        throw new KeyNotFoundException($"Chat session not found: {sessionId}");
    }

    private string GenerateChatResponse(string message, ChatContext? context)
    {
        var lowerMessage = message.ToLowerInvariant();

        // Determine response category
        string category = lowerMessage switch
        {
            var msg when msg.Contains("hello") || msg.Contains("hi") || msg.Contains("hey") => "greeting",
            var msg when msg.Contains("running") || msg.Contains("run") || msg.Contains("jog") => "running",
            var msg when msg.Contains("hiking") || msg.Contains("hike") || msg.Contains("boots") => "hiking",
            var msg when msg.Contains("price") || msg.Contains("cost") || msg.Contains("$") || msg.Contains("cheap") => "price",
            var msg when msg.Contains("return") || msg.Contains("refund") || msg.Contains("exchange") => "return",
            var msg when msg.Contains("help") || msg.Contains("assist") || msg.Contains("support") => "help",
            _ => "help"
        };

        if (ChatResponses.TryGetValue(category, out var responses))
        {
            return responses[Random.Shared.Next(responses.Length)];
        }

        return $"I understand you're asking about {message}. Let me help you find what you're looking for in our outdoor gear collection. You can browse our products or ask me about specific items, pricing, or our policies.";
    }

    private List<SuggestedAction> GenerateSuggestedActions(string message, ChatContext? context)
    {
        var lowerMessage = message.ToLowerInvariant();

        return lowerMessage switch
        {
            var msg when msg.Contains("running") || msg.Contains("run") => new List<SuggestedAction>
            {
                new() { Text = "View Running Shoes", Url = "/products?category=running" },
                new() { Text = "Running Gear Guide", Url = "/guides/running" }
            },
            var msg when msg.Contains("hiking") || msg.Contains("hike") => new List<SuggestedAction>
            {
                new() { Text = "View Hiking Boots", Url = "/products?category=hiking" },
                new() { Text = "Hiking Essentials", Url = "/guides/hiking" }
            },
            var msg when msg.Contains("return") || msg.Contains("refund") => new List<SuggestedAction>
            {
                new() { Text = "Return Policy", Url = "/support/returns" },
                new() { Text = "Start Return", Url = "/support/return-form" }
            },
            _ => new List<SuggestedAction>
            {
                new() { Text = "Browse All Products", Url = "/products" },
                new() { Text = "Customer Support", Url = "/support" }
            }
        };
    }

    private List<string> GetRelevantSources(string message)
    {
        var lowerMessage = message.ToLowerInvariant();

        return lowerMessage switch
        {
            var msg when msg.Contains("running") => new List<string> { "/products/running", "/guides/running" },
            var msg when msg.Contains("hiking") => new List<string> { "/products/hiking", "/guides/hiking" },
            var msg when msg.Contains("return") => new List<string> { "/support/returns", "/support/policy" },
            var msg when msg.Contains("price") => new List<string> { "/products", "/support/pricing" },
            _ => new List<string> { "/products", "/support" }
        };
    }
}