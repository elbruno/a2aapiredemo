using System.Text.Json;
using AgentServices.Configuration;
using AgentServices.Discount;
using AgentServices.Models;
using AgentServices.Stock;
using CartEntities;
using DataEntities;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AgentServices.Checkout;

/// <summary>
/// DEMO: Agent Checkout Orchestrator using Microsoft Agent Framework Workflows Orchestrations - Sequential
/// Coordinates the multi-agent checkout workflow using AgentWorkflowBuilder.BuildSequential:
/// 1. StockAgent → validates availability
/// 2. DiscountAgent → applies tier-based discount
/// 3. Returns combined result with agent steps log
/// 
/// This implementation uses the registered AIAgents from the ServiceProvider directly,
/// following the pattern from: https://learn.microsoft.com/en-us/agent-framework/user-guide/workflows/orchestrations/sequential
/// </summary>
public class AgentCheckoutOrchestrator
{
    private readonly IServiceProvider _serviceProvider;
    private readonly AgentSettings _settings;
    private readonly ILogger<AgentCheckoutOrchestrator> _logger;

    public AgentCheckoutOrchestrator(
        IServiceProvider serviceProvider,
        AgentSettings settings,
        ILogger<AgentCheckoutOrchestrator> logger)
    {
        _serviceProvider = serviceProvider;
        _settings = settings;
        _logger = logger;
    }

    /// <summary>
    /// DEMO: Execute the multi-agent checkout workflow using Microsoft Agent Framework Sequential Orchestration.
    /// Uses AgentWorkflowBuilder.BuildSequential to chain the StockAgent and DiscountAgent.
    /// </summary>
    public async Task<AgentCheckoutResult> ProcessCheckoutAsync(AgentCheckoutRequest request)
    {
        _logger.LogInformation("DEMO: Starting agentic checkout pipeline using Sequential Workflow Orchestration");
        _logger.LogInformation("DEMO: Membership tier: {Tier}, Cart subtotal: {Subtotal:C}", 
            request.MembershipTier, request.Cart.Subtotal);

        var result = new AgentCheckoutResult
        {
            Subtotal = request.Cart.Subtotal,
            AgentSteps = []
        };

        try
        {
            // DEMO: Get the registered AIAgents from the ServiceProvider
            var stockAgent = _serviceProvider.GetRequiredKeyedService<AIAgent>(StockAgentService.AgentName);
            var discountAgent = _serviceProvider.GetRequiredKeyedService<AIAgent>(DiscountAgentService.AgentName);

            _logger.LogInformation("DEMO: Building Sequential Workflow with StockAgent → DiscountAgent");

            // DEMO: Build the sequential workflow using AgentWorkflowBuilder
            // The output of StockAgent flows into DiscountAgent
            var workflow = AgentWorkflowBuilder.BuildSequential(
                workflowName: "CheckoutWorkflow",
                agents: [stockAgent, discountAgent]);

            // DEMO: Build the input message for the workflow
            var itemsSummary = string.Join(", ", request.Cart.Items.Select(i => $"{i.Name} (${i.Price:F2} x {i.Quantity})"));
            var inputMessage = $$"""
                Customer checkout request:
                Membership tier: {{request.MembershipTier}}
                Cart subtotal: ${{request.Cart.Subtotal:F2}}
                Items: {{itemsSummary}}
                
                Step 1 (StockAgent): Check stock availability for all items. All items are in stock for this demo.
                Step 2 (DiscountAgent): Calculate the discount based on the membership tier rules:
                - GOLD: 20% discount
                - SILVER: 10% discount
                - NORMAL: No discount
                
                Respond with JSON: { "discountAmount": <number>, "reason": "<string>" }
                """;

            _logger.LogInformation("DEMO: Executing Sequential Workflow");

            // DEMO: Execute the workflow using InProcessExecution
            var messages = new List<ChatMessage> { new(ChatRole.User, inputMessage) };
            var run = await InProcessExecution.RunAsync(workflow, messages);

            // DEMO: Process all workflow events from both agents
            var workflowEvents = run.OutgoingEvents
                .OfType<AgentRunResponseEvent>()
                .ToList();

            // DEMO: Record StockAgent step from workflow events
            var stockEvent = workflowEvents.FirstOrDefault(e => 
                e.Response.Text?.Contains("stock", StringComparison.OrdinalIgnoreCase) == true ||
                e.Response.Text?.Contains("ship", StringComparison.OrdinalIgnoreCase) == true);
            
            var stockStep = new AgentStep
            {
                Name = StockAgentService.AgentName,
                Timestamp = DateTime.UtcNow,
                Status = "Success",
                Message = stockEvent?.Response.Text?.Trim() ?? "All items are in stock and ready to ship!"
            };
            result.AgentSteps.Add(stockStep);
            _logger.LogInformation("DEMO: StockAgent completed - Status: {Status}", stockStep.Status);

            // DEMO: Record DiscountAgent step from workflow events (last event is typically from the last agent)
            var discountEvent = workflowEvents.LastOrDefault();

            var discountStep = new AgentStep
            {
                Name = DiscountAgentService.AgentName,
                Timestamp = DateTime.UtcNow
            };

            if (discountEvent?.Response.Text != null)
            {
                _logger.LogInformation("DEMO: Sequential Workflow completed. Parsing response...");
                var discountResult = ParseDiscountResponse(discountEvent.Response.Text, request.Cart.Subtotal, request.MembershipTier);
                
                result.DiscountAmount = discountResult.DiscountAmount;
                result.DiscountReason = discountResult.DiscountReason;
                result.TotalAfterDiscount = discountResult.TotalAfterDiscount;
                
                discountStep.Status = discountResult.Success ? "Success" : "Warning";
                discountStep.Message = discountResult.DiscountReason;

                _logger.LogInformation("DEMO: DiscountAgent completed - Discount: {Discount:C}", 
                    discountResult.DiscountAmount);
            }
            else
            {
                _logger.LogWarning("DEMO: No response from workflow, using fallback discount calculation");
                var fallbackResult = ComputeFallbackDiscount(request.MembershipTier, request.Cart.Subtotal);
                
                result.DiscountAmount = fallbackResult.DiscountAmount;
                result.DiscountReason = fallbackResult.DiscountReason;
                result.TotalAfterDiscount = fallbackResult.TotalAfterDiscount;
                
                discountStep.Status = "Warning";
                discountStep.Message = fallbackResult.DiscountReason;
            }

            result.AgentSteps.Add(discountStep);

            // Finalize result
            result.Success = true;
            _logger.LogInformation("DEMO: Sequential Workflow Orchestration completed successfully");
            _logger.LogInformation("DEMO: Final - Subtotal: {Subtotal:C}, Discount: {Discount:C}, Total: {Total:C}",
                result.Subtotal, result.DiscountAmount, result.TotalAfterDiscount);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "DEMO: Sequential Workflow Orchestration failed");
            
            result.AgentSteps.Add(new AgentStep
            {
                Name = "Orchestrator",
                Status = "Error",
                Message = $"Workflow failed: {ex.Message}",
                Timestamp = DateTime.UtcNow
            });

            // Fallback to deterministic discount calculation
            var fallbackResult = ComputeFallbackDiscount(request.MembershipTier, request.Cart.Subtotal);
            result.DiscountAmount = fallbackResult.DiscountAmount;
            result.DiscountReason = $"Checkout running in standard mode - {fallbackResult.DiscountReason}";
            result.TotalAfterDiscount = fallbackResult.TotalAfterDiscount;
            result.Success = true; // Still mark as success with fallback
            result.ErrorMessage = ex.Message;

            return result;
        }
    }

    /// <summary>
    /// Parse the discount response from the workflow.
    /// </summary>
    private DiscountResult ParseDiscountResponse(string content, decimal subtotal, MembershipTier tier)
    {
        try
        {
            // Extract JSON from response (handle markdown code blocks if present)
            var jsonContent = content.Trim();
            if (jsonContent.Contains("```json"))
            {
                var start = jsonContent.IndexOf("```json") + 7;
                var end = jsonContent.IndexOf("```", start);
                if (end > start)
                {
                    jsonContent = jsonContent.Substring(start, end - start).Trim();
                }
            }
            else if (jsonContent.Contains("```"))
            {
                var start = jsonContent.IndexOf("```") + 3;
                var end = jsonContent.IndexOf("```", start);
                if (end > start)
                {
                    jsonContent = jsonContent.Substring(start, end - start).Trim();
                }
            }

            using var doc = JsonDocument.Parse(jsonContent);
            var root = doc.RootElement;

            var discountAmount = root.GetProperty("discountAmount").GetDecimal();
            var reason = root.GetProperty("reason").GetString() ?? "Discount applied";

            // Validate and clamp discount
            if (discountAmount < 0) discountAmount = 0;
            if (discountAmount > subtotal) discountAmount = subtotal;

            return new DiscountResult
            {
                DiscountAmount = discountAmount,
                DiscountReason = reason,
                TotalAfterDiscount = subtotal - discountAmount,
                Success = true
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "DEMO: Failed to parse workflow response, using fallback");
            return ComputeFallbackDiscount(tier, subtotal);
        }
    }

    /// <summary>
    /// DEMO: Deterministic fallback when workflow or AI is unavailable.
    /// </summary>
    private static DiscountResult ComputeFallbackDiscount(MembershipTier tier, decimal subtotal)
    {
        var (percentage, reason) = tier switch
        {
            MembershipTier.Gold => (0.20m, "Gold member 20% discount applied"),
            MembershipTier.Silver => (0.10m, "Silver member 10% discount applied"),
            _ => (0m, "No discount - Standard membership")
        };

        var discountAmount = subtotal * percentage;

        return new DiscountResult
        {
            DiscountAmount = discountAmount,
            DiscountReason = reason,
            TotalAfterDiscount = subtotal - discountAmount,
            Success = true
        };
    }
}
