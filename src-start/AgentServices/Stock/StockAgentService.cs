using AgentServices.Configuration;
using AgentServices.Models;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

namespace AgentServices.Stock;

/// <summary>
/// DEMO: Stock Agent Service
/// Validates stock availability and generates human-friendly summaries.
/// For demo purposes, stock is always available (deterministic), but uses AI for friendly messages.
/// </summary>
public class StockAgentService : IStockAgentService
{
    private readonly IChatClient? _chatClient;
    private readonly ILogger<StockAgentService> _logger;
    private readonly AgentSettings _settings;

    // DEMO: System prompt for stock agent message generation
    private const string SystemPrompt = """
        You are a friendly e-commerce stock checker assistant.
        Given a list of items and their stock status, generate a brief, friendly summary message.
        Be concise and positive. If all items are available, say something like "Great news! All items are in stock and ready to ship."
        If there are issues, mention them briefly.
        Respond with just the summary message, no JSON or formatting.
        """;

    public StockAgentService(
        IChatClient? chatClient,
        AgentSettings settings,
        ILogger<StockAgentService> logger)
    {
        _chatClient = chatClient;
        _settings = settings;
        _logger = logger;
    }

    /// <summary>
    /// DEMO: Check stock availability for cart items.
    /// For demo purposes, all items are considered in stock.
    /// </summary>
    public async Task<StockCheckResult> CheckStockAsync(StockCheckRequest request)
    {
        _logger.LogInformation("DEMO: StockAgent starting - Checking {ItemCount} items", request.Items.Count);

        // DEMO: For demo purposes, all items are in stock
        // In a real scenario, this would query the Products API or database
        var result = new StockCheckResult
        {
            HasStockIssues = false,
            Issues = new List<StockIssue>(),
            Success = true
        };

        // Generate a friendly summary message
        result.SummaryMessage = await GenerateSummaryMessage(request, result);
        
        _logger.LogInformation("DEMO: StockAgent completed - HasIssues: {HasIssues}, Message: {Message}", 
            result.HasStockIssues, result.SummaryMessage);

        return result;
    }

    private async Task<string> GenerateSummaryMessage(StockCheckRequest request, StockCheckResult result)
    {
        // If AI is not available, use fallback message
        if (_chatClient == null || !_settings.AgentsEnabled)
        {
            _logger.LogDebug("DEMO: AI not available, using fallback stock message");
            return result.HasStockIssues 
                ? "Some items have limited availability. Please review your cart."
                : "All items are in stock and ready to ship!";
        }

        try
        {
            var itemsList = string.Join("\n", request.Items.Select(i => $"- {i.Name} (Qty: {i.Quantity}): In Stock"));
            var userMessage = $"""
                Items to check:
                {itemsList}
                
                All items are available. Generate a brief, friendly confirmation message.
                """;

            var messages = new List<ChatMessage>
            {
                new(ChatRole.System, SystemPrompt),
                new(ChatRole.User, userMessage)
            };

            _logger.LogDebug("DEMO: Sending request to StockAgent AI for summary");
            var response = await _chatClient.GetResponseAsync(messages);
            var content = response.Text?.Trim() ?? "";

            if (!string.IsNullOrEmpty(content))
            {
                return content;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "DEMO: StockAgent AI error, using fallback message");
        }

        // Fallback
        return "All items are in stock and ready to ship!";
    }
}
