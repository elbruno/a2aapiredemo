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
/// 
/// TODO: This is the starting point for the live demo.
/// During the demo, you will implement AI-powered stock checking using Microsoft Agent Framework's AIAgent.
/// See docs/04_speaker-demo-walkthrough.md for step-by-step instructions.
/// </summary>
public class StockAgentService : IStockAgentService
{
    private readonly IChatClient? _chatClient;
    private readonly ILogger<StockAgentService> _logger;
    private readonly AgentSettings _settings;

    // DEMO: Agent name for identification in logs and debugging
    private const string AgentName = "StockAgent";

    // TODO: Step 2.1 - Add the Agent Instructions here
    // The instructions should instruct the AI agent to generate friendly stock status messages.
    // Used with Microsoft Agent Framework's CreateAIAgent method:
    // var agent = _chatClient.CreateAIAgent(instructions: AgentInstructions, name: AgentName);
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
    /// TODO: Implement AI-powered stock checking using Microsoft Agent Framework.
    /// 
    /// Example pattern using Agent Framework:
    /// 1. Create agent: var agent = _chatClient.CreateAIAgent(instructions: AgentInstructions, name: AgentName);
    /// 2. Run agent: var response = await agent.RunAsync(userMessage);
    /// 3. Get text: var content = response.Text;
    /// 
    /// See docs/04_speaker-demo-walkthrough.md#step-22-replace-the-checkstockasync-method
    /// </summary>
    public Task<StockCheckResult> CheckStockAsync(StockCheckRequest request)
    {
        _logger.LogInformation("TODO: {AgentName} not implemented...", AgentName);
        
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
    // This method should use Microsoft Agent Framework's AIAgent to generate friendly stock status messages.
    // Pattern: var agent = _chatClient.CreateAIAgent(...); var response = await agent.RunAsync(...);
    // See docs/04_speaker-demo-walkthrough.md#step-23-add-the-generatesummarymessage-method
}
