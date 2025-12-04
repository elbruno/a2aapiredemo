using AgentServices.Configuration;
using AgentServices.Models;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AgentServices.Stock;

/// <summary>
/// Stock Agent Service
/// Uses Microsoft Agent Framework to validate stock availability and generate human-friendly summaries.
/// For demo purposes, stock is always available (deterministic), but uses AI for friendly messages.
/// </summary>
public class StockAgentService
{
    private readonly ILogger<StockAgentService> _logger;
    private readonly AIAgent _stockAgent;

    // Agent name for identification in logs and debugging
    public const string AgentName = "StockAgent";

    // Agent instructions (system prompt) for stock agent message generation
    public const string AgentInstructions = """
        You are a friendly e-commerce stock checker assistant.
        Given a list of items and their stock status, generate a brief, friendly summary message.
        Be concise and positive. If all items are available, say something like "Great news! All items are in stock and ready to ship."
        If there are issues, mention them briefly.
        Respond with just the summary message, no JSON or formatting.
        """;

    public StockAgentService(
        IServiceProvider serviceProvider,
        ILogger<StockAgentService> logger)
    {
        _logger = logger;
        _stockAgent = serviceProvider.GetRequiredKeyedService<AIAgent>(AgentName);
    }

    /// <summary>
    /// Check stock availability for cart items using Microsoft Agent Framework.
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
        if (_stockAgent == null)
        {
            _logger.LogDebug("AI not available, using fallback stock message");
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

            _logger.LogDebug("DEMO: Sending request to {AgentName} via Agent Framework for summary", AgentName);
            
            // DEMO: Use the Agent Framework's RunAsync method
            // This encapsulates the message building and response handling
            var response = await _stockAgent.RunAsync(userMessage);
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
