using AgentServices.Checkout;
using AgentServices.Configuration;
using AgentServices.Discount;
using AgentServices.Stock;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AgentServices;

/// <summary>
/// DEMO Step 3: Extension methods to register Agent Services in DI.
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
    /// <summary>
    /// Registers all agent services for the agentic checkout demo.
    /// 
    /// This method demonstrates best practices for agent registration:
    /// - Uses scoped lifetime for request-isolated agent instances
    /// - Centralizes configuration binding
    /// - Enables proper dependency injection for testing
    /// </summary>
    public static IServiceCollection AddAgentServices(this IServiceCollection services, IConfiguration configuration)
    {
        // DEMO Step 3: Register configuration with proper binding
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

        // Register settings as singleton (configuration doesn't change at runtime)
        services.AddSingleton(settings);

        // DEMO Step 3: Register agent services with scoped lifetime
        // Scoped lifetime ensures:
        // - Each HTTP request gets its own agent instance
        // - Agent state is isolated between requests
        // - Proper disposal of resources
        services.AddScoped<IStockAgentService, StockAgentService>();
        services.AddScoped<IDiscountAgentService, DiscountAgentService>();
        services.AddScoped<IAgentCheckoutOrchestrator, AgentCheckoutOrchestrator>();

        return services;
    }
}
