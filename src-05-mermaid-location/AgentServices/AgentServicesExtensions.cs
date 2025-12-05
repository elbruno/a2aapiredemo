using System;
using AgentServices.Checkout;
using AgentServices.Configuration;
using AgentServices.Discount;
using AgentServices.Stock;
using AgentServices.Stock.Tools;
using Azure.AI.Projects;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Hosting;
using Microsoft.Agents.AI.Workflows;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenAI.Chat;

namespace AgentServices;

/// <summary>
/// DEMO Step 4: Extension methods to register Agent Services in DI.
/// 
/// This approach follows the pattern from the Agent Framework's AgentWebChat sample:
/// https://github.com/microsoft/agent-framework/blob/main/dotnet/samples/AgentWebChat/AgentWebChat.AgentHost/Program.cs
/// 
/// Benefits of registering agents through DI:
/// 1. Proper lifecycle management (scoped services per request)
/// 2. Easier unit testing through interface injection
/// 3. Centralized configuration via AgentSettings
/// 4. Support for OpenTelemetry tracing and logging
/// 5. Integration with DevUI for debugging
/// </summary>
public static class AgentServicesExtensions
{
    public static WebApplicationBuilder AddAgentSettings(this WebApplicationBuilder builder)
    {
        var settings = new AgentSettings();
        builder.Configuration.GetSection(AgentSettings.SectionName).Bind(settings);

        // Also check for connection string directly if not in AgentSettings section
        if (string.IsNullOrEmpty(settings.MicrosoftFoundryConnectionString))
        {
            settings.MicrosoftFoundryConnectionString = builder.Configuration.GetConnectionString("microsoftfoundry");
        }

        // Get deployment name from configuration
        var chatDeploymentName = builder.Configuration["AI_ChatDeploymentName"];
        if (!string.IsNullOrEmpty(chatDeploymentName))
        {
            settings.ChatDeploymentName = chatDeploymentName;
        }

        // Register settings as singleton (configuration doesn't change at runtime)
        builder.Services.AddSingleton(settings);

        return builder;
    }

    /// <summary>
    /// Registers the StockSearchTool as a scoped service with an HttpClient configured 
    /// to call the Products API.
    /// </summary>
    public static WebApplicationBuilder AddStockSearchTool(this WebApplicationBuilder builder, string productsApiBaseUrl)
    {
        builder.Services.AddHttpClient<StockSearchTool>(client =>
        {
            client.BaseAddress = new Uri(productsApiBaseUrl);
        });

        return builder;
    }

    public static WebApplicationBuilder AddeShopLiteAIAgents(this WebApplicationBuilder builder)
    {
        // add Discount Agent
        builder.AddAIAgent(DiscountAgentService.AgentName, (sp, key) =>
        {
            // create agent
            var chatClient = sp.GetRequiredService<IChatClient>();

            return chatClient.CreateAIAgent(
                name: DiscountAgentService.AgentName,
                instructions: DiscountAgentService.AgentInstructions);
        });

        // add Stock Agent with external stock search tool
        builder.AddAIAgent(StockAgentService.AgentName, (sp, key) =>
        {
            // create agent
            var chatClient = sp.GetRequiredService<IChatClient>();
            
            // Get the StockSearchTool if registered via AddStockSearchTool().
            // The tool is intentionally optional - if not registered, the agent 
            // will work without the external stock search capability.
            var stockSearchTool = sp.GetService<StockSearchTool>();
            
            // Create the agent with the stock search tool if available
            if (stockSearchTool != null)
            {
                return chatClient.CreateAIAgent(
                    name: StockAgentService.AgentName,
                    instructions: StockAgentService.AgentInstructions,
                    tools: [AIFunctionFactory.Create(stockSearchTool.SearchProductStockAsync)]);
            }

            // Fallback: Create agent without tools
            return chatClient.CreateAIAgent(
                name: StockAgentService.AgentName,
                instructions: StockAgentService.AgentInstructions);
        });

        builder.AddWorkflow(AgentCheckoutOrchestrator.WorkflowName, (sp, key) =>
        {
            var stockAgent = sp.GetRequiredKeyedService<AIAgent>(StockAgentService.AgentName);
            var discountAgent = sp.GetRequiredKeyedService<AIAgent>(DiscountAgentService.AgentName);

            var workflow = AgentWorkflowBuilder.BuildSequential(
                workflowName: AgentCheckoutOrchestrator.WorkflowName,
                agents: [stockAgent, discountAgent]);

            return workflow;
        });

        return builder;
    }


    public static WebApplicationBuilder AddeShopLiteFoundryAgents(this WebApplicationBuilder builder)
    {
        var microsoftFoundryProjectEndpoint = "https://bruno-netdayagentmodernization-r.services.ai.azure.com/api/projects/bruno-netdayagentmodernization";

        AIAgent stockAgent = null;
        AIAgent discountAgent = null;

        AIProjectClient projectClient = new(
            endpoint: new Uri(microsoftFoundryProjectEndpoint),
            tokenProvider: new DefaultAzureCredential());


        // add Discount Agent
        builder.AddAIAgent(DiscountAgentService.AgentName, (sp, key) =>
        {
            // create agent
            discountAgent = projectClient.GetAIAgent(DiscountAgentService.AgentName);
            return discountAgent;
        });

        // add Stock Agent with external stock search tool
        builder.AddAIAgent(StockAgentService.AgentName, (sp, key) =>
        {
            // create agent
            stockAgent = projectClient.GetAIAgent(StockAgentService.AgentName);
            return stockAgent;
        });

        builder.AddWorkflow(AgentCheckoutOrchestrator.WorkflowName, (sp, key) =>
        {
            var workflow = AgentWorkflowBuilder.BuildSequential(
                workflowName: "AgentCheckoutOrchestrator.WorkflowName",
                agents: [stockAgent, discountAgent]);

            return workflow;
        });

        return builder;
    }
}
