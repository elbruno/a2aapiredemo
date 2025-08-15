using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SemanticSearchFunction.Services;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((context, services) =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        // add db
        var productsDbConnectionString = context.Configuration.GetConnectionString("productsDb");
        services.AddDbContext<Context>(options =>
            options.UseSqlServer(productsDbConnectionString, o => o.UseVectorSearch()));

        var embeddingsDeploymentName = context.Configuration["AI_embeddingsDeploymentName"] ?? "text-embedding-3-small";

        services.AddSingleton(_ =>
            AzureOpenAiEmbeddingProvider.CreateEmbeddingClient(context.Configuration, embeddingsDeploymentName));
    })
    .Build();

host.Run();