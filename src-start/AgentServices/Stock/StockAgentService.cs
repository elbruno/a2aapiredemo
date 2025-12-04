using AgentServices.Configuration;
using AgentServices.Models;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

namespace AgentServices.Stock;

/// <summary>
/// TODO: DEMO START POINT - Stock Agent Service
/// 
/// This is the starting point for the agentic demo. Implement the AI-powered
/// stock availability check with friendly summaries.
/// 
/// INSTRUCTIONS:
/// 1. Create a system prompt that asks the AI to generate friendly stock status messages
/// 
/// 2. For demo purposes, assume all items are in stock
/// 
/// 3. Use the IChatClient to generate a friendly confirmation message
/// 
/// 4. Handle errors gracefully with fallback messages
/// 
/// See src-complete for the full implementation.
/// </summary>
public class StockAgentService : IStockAgentService
{
    private readonly IChatClient? _chatClient;
    private readonly ILogger<StockAgentService> _logger;
    private readonly AgentSettings _settings;

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
    /// TODO: Implement AI-powered stock check with friendly messages.
    /// Currently returns a simple static message - implement the agent logic here.
    /// </summary>
    public Task<StockCheckResult> CheckStockAsync(StockCheckRequest request)
    {
        _logger.LogInformation("TODO: StockAgent not implemented - Checking {ItemCount} items", request.Items.Count);

        // TODO: Implement the following:
        // 1. Build a system prompt for generating friendly stock messages
        // 2. Create a user message listing the items to check
        // 3. Call _chatClient.GetResponseAsync() with the messages
        // 4. Return the AI-generated friendly message

        // Placeholder: Return static response (implement agent logic above)
        var result = new StockCheckResult
        {
            HasStockIssues = false,
            Issues = new List<StockIssue>(),
            SummaryMessage = "Stock check completed - Agent not implemented",
            Success = true
        };

        return Task.FromResult(result);
    }
}
