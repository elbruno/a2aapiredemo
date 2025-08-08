using Azure.Provisioning.CognitiveServices;

var builder = DistributedApplication.CreateBuilder(args);

var sql = builder.AddSqlServer("sql")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithImageTag("2025-latest")
    .WithEnvironment("ACCEPT_EULA", "Y");

var productsDb = sql
    .WithDataVolume()
    .AddDatabase("productsDb");

// Add NLWeb Docker container with proper configuration
var nlweb = builder.AddDockerfile(
    "nlweb", ".", "NLWeb.Dockerfile")
    .WithHttpEndpoint(port: 8000, targetPort: 8000, name: "http")
    .WithEnvironment("NLWEB_PORT", "8000")
    .WithEnvironment("NLWEB_HOST", "0.0.0.0")
    .WithEnvironment("NLWEB_LOG_LEVEL", "INFO")
    .WithEnvironment("NLWEB_VECTOR_DB", "in_memory") // Use in-memory for demo
    .WithBindMount("./nlweb-data", "/app/data") // Persistent data storage
    .WithBindMount("./nlweb-config", "/app/config", isReadOnly: true) // Configuration
    .WithLifetime(ContainerLifetime.Persistent);

var products = builder.AddProject<Projects.Products>("products")
    .WithReference(productsDb)
    .WaitFor(productsDb)
    .WithExternalHttpEndpoints();

var search = builder.AddProject<Projects.Search>("search")
    .WithEnvironment("ConnectionStrings__nlweb", nlweb.GetEndpoint("http"))
    .WaitFor(nlweb)
    .WithExternalHttpEndpoints();

var chat = builder.AddProject<Projects.Chat>("chat")
    .WithEnvironment("ConnectionStrings__nlweb", nlweb.GetEndpoint("http"))
    .WaitFor(nlweb)
    .WithExternalHttpEndpoints();

var store = builder.AddProject<Projects.Store>("store")
    .WithReference(products)
    .WithReference(search)
    .WithReference(chat)
    .WaitFor(products)
    .WaitFor(search)
    .WaitFor(chat)
    .WithExternalHttpEndpoints();

if (builder.ExecutionContext.IsPublishMode)
{
    // production code uses Azure services, so we need to add them here
    var appInsights = builder.AddAzureApplicationInsights("appInsights");
    var chatDeploymentName = "gpt-5-mini";
    var embeddingsDeploymentName = "text-embedding-ada-002";
    var aoai = builder.AddAzureOpenAI("openai");

    var gpt5mini = aoai.AddDeployment(name: chatDeploymentName,
            modelName: "gpt-5-mini",
            modelVersion: "2025-08-07");
    gpt5mini.Resource.SkuCapacity = 10;
    //gpt5mini.Resource.SkuName = "GlobalStandard";

    var embeddingsDeployment = aoai.AddDeployment(name: embeddingsDeploymentName,
        modelName: "text-embedding-ada-002",
        modelVersion: "2");


    products.WithReference(appInsights)
        .WithReference(aoai)
        .WithEnvironment("AI_ChatDeploymentName", chatDeploymentName)
        .WithEnvironment("AI_embeddingsDeploymentName", embeddingsDeploymentName);

    search.WithReference(appInsights);

    chat.WithReference(appInsights);

    store.WithReference(appInsights)
        .WithExternalHttpEndpoints();
}

builder.Build().Run();
