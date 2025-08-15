using Azure;
using Azure.AI.OpenAI;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenAI.Embeddings;

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
            CreateEmbeddingClient(context.Configuration, embeddingsDeploymentName));
    })
    .Build();

host.Run();

// Added static factory method for creating an embedding generator from the configured OpenAI connection string
static IEmbeddingGenerator<string, Embedding<float>> CreateEmbeddingClient(IConfiguration configuration, string deploymentName)
{
    if (string.IsNullOrWhiteSpace(deploymentName))
        throw new ArgumentException("Deployment name not configured", nameof(deploymentName));

    var conn = configuration.GetConnectionString("openai");
    if (string.IsNullOrWhiteSpace(conn))
        throw new InvalidOperationException("Connection string 'openai' not found. Expected format: Endpoint=https://<resource>.openai.azure.com/models/;Key=<api key>;");

    // Parse connection string (Endpoint=...;Key=...;)
    var parts = conn.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    foreach (var part in parts)
    {
        var kv = part.Split('=', 2, StringSplitOptions.TrimEntries);
        if (kv.Length == 2) dict[kv[0]] = kv[1];
    }

    if (!dict.TryGetValue("Endpoint", out var endpoint) || string.IsNullOrWhiteSpace(endpoint))
        throw new InvalidOperationException("'Endpoint' not found in 'openai' connection string");
    if (!dict.TryGetValue("Key", out var key) || string.IsNullOrWhiteSpace(key))
        throw new InvalidOperationException("'Key' not found in 'openai' connection string");

    // Normalize endpoint (remove trailing /models/ or /models)
    endpoint = endpoint.TrimEnd('/');
    if (endpoint.EndsWith("/models", StringComparison.OrdinalIgnoreCase))
        endpoint = endpoint[..^7]; // remove '/models'


    AzureOpenAIClient client = null;

    if (!string.IsNullOrWhiteSpace(key))
    { 
        client = new AzureOpenAIClient(new Uri(endpoint), new AzureKeyCredential(key)); }
    else
    {
        client = new AzureOpenAIClient(new Uri(endpoint), new Azure.Identity.DefaultAzureCredential());
    }

    // Microsoft.Extensions.AI provides extension to adapt AzureOpenAIClient to IEmbeddingGenerator
    var embeddingClient = client.GetEmbeddingClient(deploymentName);

    return embeddingClient.AsIEmbeddingGenerator();
}
