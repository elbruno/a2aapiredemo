using AgentServices.Configuration;
using AgentServices.Models;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

namespace AgentServices.Stock;

/// <summary>
/// DEMO: Stock Agent Service
/// Uses Microsoft Agent Framework to validate stock availability and generate human-friendly summaries.
/// For demo purposes, stock is always available (deterministic), but uses AI for friendly messages.
/// </summary>
public class StockAgentService : IStockAgentService
{
    private readonly IChatClient? _chatClient;
    private readonly ILogger<StockAgentService> _logger;
    private readonly AgentSettings _settings;

    // DEMO: Agent name for identification in logs and debugging
    private const string AgentName = "StockAgent";

    // DEMO: Agent instructions (system prompt) for stock agent message generation
    private const string AgentInstructions = """
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
    /// DEMO: Check stock availability for cart items using Microsoft Agent Framework.
    /// For demo purposes, all items are considered in stock.
    /// </summary>
    public async Task<StockCheckResult> CheckStockAsync(StockCheckRequest request)
    {
        _logger.LogInformation("DEMO: {AgentName} starting - Checking {ItemCount} items", AgentName, request.Items.Count);

        // DEMO: For demo purposes, all items are in stock
        // In a real scenario, this would query the Products API or database
        var result = new StockCheckResult
        {
            HasStockIssues = false,
            Issues = new List<StockIssue>(),
            Success = true
        };

        // Generate a friendly summary message using the Agent Framework
        result.SummaryMessage = await GenerateSummaryMessage(request, result);
        
        _logger.LogInformation("DEMO: {AgentName} completed - HasIssues: {HasIssues}, Message: {Message}", 
            AgentName, result.HasStockIssues, result.SummaryMessage);

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
            // DEMO: Create an AIAgent using Microsoft Agent Framework
            // The agent encapsulates the instructions and provides a clean abstraction
            var stockAgent = _chatClient.CreateAIAgent(
                instructions: AgentInstructions,
                name: AgentName);

            var itemsList = string.Join("\n", request.Items.Select(i => $"- {i.Name} (Qty: {i.Quantity}): In Stock"));
            var userMessage = $"""
                Items to check:
                {itemsList}
                
                All items are available. Generate a brief, friendly confirmation message.
                """;

            _logger.LogDebug("DEMO: Sending request to {AgentName} via Agent Framework for summary", AgentName);
            
            // DEMO: Use the Agent Framework's RunAsync method
            // This encapsulates the message building and response handling
            var response = await stockAgent.RunAsync(userMessage);
            var content = response.Text?.Trim() ?? "";

            if (!string.IsNullOrEmpty(content))
            {
                return content;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "DEMO: {AgentName} error, using fallback message", AgentName);
        }

        // Fallback
        return "All items are in stock and ready to ship!";
    }
}
