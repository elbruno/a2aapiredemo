using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenAI;
using OpenAI.Embeddings;
using Microsoft.EntityFrameworkCore;

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

        // add openai client
        var azureOpenAiClientName = "openai";
        var embeddingsDeploymentName = context.Configuration["AI_embeddingsDeploymentName"] ?? "text-embedding-3-small";
        var chatDeploymentName = context.Configuration["AI_ChatDeploymentName"] ?? "gpt-4.1-mini";

        services.AddEmbeddingGenerator(embeddingsDeploymentName); 

        //builder.AddAzureOpenAIClient(azureOpenAiClientName, configureSettings: settings =>
        //{
        //    settings.Credential = new AzureCliCredential();
        //}).AddChatClient(chatDeploymentName);

        // Register repositories
        //services.AddScoped<ISemanticSearchRepository, SqlSemanticSearchRepository>();
    })
    .Build();

host.Run();