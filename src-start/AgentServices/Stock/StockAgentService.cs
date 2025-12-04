using AgentServices.Configuration;
using AgentServices.Models;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

namespace AgentServices.Stock;

/// <summary>
/// DEMO: Stock Agent Service
/// Validates stock availability and generates human-friendly summaries.
/// For demo purposes, stock is always available (deterministic), but uses AI for friendly messages.
/// 
/// TODO: This is the starting point for the live demo.
/// During the demo, you will implement AI-powered stock checking.
/// See docs/04_speaker-demo-walkthrough.md for step-by-step instructions.
/// </summary>
public class StockAgentService : IStockAgentService
{
    private readonly IChatClient? _chatClient;
    private readonly ILogger<StockAgentService> _logger;
    private readonly AgentSettings _settings;

    // TODO: Step 2.1 - Add the System Prompt here
    // The prompt should instruct the AI to generate friendly stock status messages.
    // See docs/04_speaker-demo-walkthrough.md#step-21-add-the-system-prompt

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
    /// TODO: Implement AI-powered stock checking.
    /// See docs/04_speaker-demo-walkthrough.md#step-22-replace-the-checkstockasync-method
    /// </summary>
    public Task<StockCheckResult> CheckStockAsync(StockCheckRequest request)
    {
        _logger.LogInformation("TODO: StockAgent not implemented...");
        
        var result = new StockCheckResult
        {
            HasStockIssues = false,
            Issues = new List<StockIssue>(),
            SummaryMessage = "Stock check completed - Agent not implemented",
            Success = true
        };
        return Task.FromResult(result);
    }

    // TODO: Step 2.3 - Add the GenerateSummaryMessage method here
    // This method should use AI to generate friendly stock status messages.
    // See docs/04_speaker-demo-walkthrough.md#step-23-add-the-generatesummarymessage-method
}
