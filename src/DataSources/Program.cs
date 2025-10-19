using Azure.Identity;
using Microsoft.Extensions.AI;
using DataSources.Endpoints;
using DataSources.Memory;
using DataSources.Services;

public class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Disable Globalization Invariant Mode
        Environment.SetEnvironmentVariable("DOTNET_SYSTEM_GLOBALIZATION_INVARIANT", "false");

        // Add Aspire service defaults
        builder.AddServiceDefaults();
        builder.Services.AddProblemDetails();

        // Add services to the container
        builder.Services.AddOpenApi();

        // Configure Azure OpenAI
        var azureOpenAIConnectionName = "openai";
        var chatDeploymentName = builder.Configuration["AI_ChatDeploymentName"] ?? "gpt-5-mini";
        var embeddingsDeploymentName = builder.Configuration["AI_embeddingsDeploymentName"] ?? "text-embedding-ada-002";

        builder.AddAzureOpenAIClient(connectionName: azureOpenAIConnectionName,
            configureSettings: settings =>
            {
                if (string.IsNullOrEmpty(settings.Key))
                {
                    settings.Credential = new DefaultAzureCredential();
                }
            }).AddChatClient(chatDeploymentName);

        builder.AddAzureOpenAIClient(azureOpenAIConnectionName,
            configureSettings: settings =>
            {
                if (string.IsNullOrEmpty(settings.Key))
                {
                    settings.Credential = new DefaultAzureCredential();
                }
            }).AddEmbeddingGenerator(embeddingsDeploymentName);

        // Add HTTP client for web crawling
        builder.Services.AddHttpClient<WebCrawlerService>(client =>
        {
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        // Register services
        builder.Services.AddSingleton<WebCrawlerService>();

        // Add memory context
        builder.Services.AddSingleton(sp =>
        {
            var logger = sp.GetService<ILogger<DataSourcesMemoryContext>>();
            var chatClient = sp.GetService<IChatClient>();
            var embeddingGenerator = sp.GetService<IEmbeddingGenerator<string, Embedding<float>>>();
            var webCrawlerService = sp.GetService<WebCrawlerService>();

            logger?.LogInformation("Creating DataSources memory context");
            return new DataSourcesMemoryContext(logger!, chatClient, embeddingGenerator, webCrawlerService!);
        });

        var app = builder.Build();

        // Add Aspire default endpoints
        app.MapDefaultEndpoints();

        // Configure the HTTP request pipeline
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();

        // Map DataSources endpoints
        app.MapDataSourcesEndpoints();

        // Log configuration
        app.Logger.LogInformation($"DataSources service starting");
        app.Logger.LogInformation($"Azure OpenAI Client Name: {azureOpenAIConnectionName}");
        app.Logger.LogInformation($"Chat Deployment: {chatDeploymentName}");
        app.Logger.LogInformation($"Embeddings Deployment: {embeddingsDeploymentName}");

        // Enable OpenTelemetry for OpenAI
        AppContext.SetSwitch("OpenAI.Experimental.EnableOpenTelemetry", true);

        // Initialize memory context
        using (var scope = app.Services.CreateScope())
        {
            var memoryContext = scope.ServiceProvider.GetRequiredService<DataSourcesMemoryContext>();
            await memoryContext.InitMemoryContextAsync();
            app.Logger.LogInformation("DataSources memory context initialized");
        }

        app.Run();
    }
}