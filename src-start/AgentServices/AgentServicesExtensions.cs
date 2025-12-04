using AgentServices.Checkout;
using AgentServices.Configuration;
using AgentServices.Discount;
using AgentServices.Stock;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AgentServices;

/// <summary>
/// DEMO: Extension methods to register Agent Services in DI.
/// </summary>
public static class AgentServicesExtensions
{
    /// <summary>
    /// Registers all agent services for the agentic checkout demo.
    /// </summary>
    public static IServiceCollection AddAgentServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Register configuration
        var settings = new AgentSettings();
        configuration.GetSection(AgentSettings.SectionName).Bind(settings);
        
        // Also check for connection string directly if not in AgentSettings section
        if (string.IsNullOrEmpty(settings.MicrosoftFoundryConnectionString))
        {
            settings.MicrosoftFoundryConnectionString = configuration.GetConnectionString("microsoftfoundry");
        }

        // Get deployment name from configuration
        var chatDeploymentName = configuration["AI_ChatDeploymentName"];
        if (!string.IsNullOrEmpty(chatDeploymentName))
        {
            settings.ChatDeploymentName = chatDeploymentName;
        }

        services.AddSingleton(settings);

        // Register agent services
        services.AddScoped<IStockAgentService, StockAgentService>();
        services.AddScoped<IDiscountAgentService, DiscountAgentService>();
        services.AddScoped<IAgentCheckoutOrchestrator, AgentCheckoutOrchestrator>();

        return services;
    }
}
