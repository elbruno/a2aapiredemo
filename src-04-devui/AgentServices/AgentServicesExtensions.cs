using AgentServices.Checkout;
using AgentServices.Configuration;
using AgentServices.Discount;
using AgentServices.Stock;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Hosting;
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

        // add Stock Agent
        builder.AddAIAgent(StockAgentService.AgentName, (sp, key) =>
        {
            // create agent
            var chatClient = sp.GetRequiredService<IChatClient>();

            return chatClient.CreateAIAgent(
                name: StockAgentService.AgentName,
                instructions: StockAgentService.AgentInstructions);
        });

        return builder;
    }
}
