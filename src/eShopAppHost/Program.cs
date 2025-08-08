using Azure.Provisioning.CognitiveServices;

var builder = DistributedApplication.CreateBuilder(args);

var sqldb = builder.AddSqlServer("sql")
    .WithDataVolume()
    .AddDatabase("sqldb");

// Add NLWeb Docker container with proper configuration
var nlweb = builder.AddContainer("nlweb", "nlweb/nlweb")
    .WithHttpEndpoint(port: 8000, targetPort: 8000, name: "http")
    .WithEnvironment("NLWEB_PORT", "8000")
    .WithEnvironment("NLWEB_HOST", "0.0.0.0")
    .WithEnvironment("NLWEB_LOG_LEVEL", "INFO")
    .WithEnvironment("NLWEB_VECTOR_DB", "in_memory") // Use in-memory for demo
    .WithBindMount("./nlweb-data", "/app/data") // Persistent data storage
    .WithBindMount("./nlweb-config", "/app/config", isReadOnly: true) // Configuration
    .WithLifetime(ContainerLifetime.Persistent);

var products = builder.AddProject<Projects.Products>("products")
    .WithReference(sqldb)
    .WaitFor(sqldb)
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
    var chatDeploymentName = "gpt-41-mini";
    var embeddingsDeploymentName = "text-embedding-ada-002";
    var aoai = builder.AddAzureOpenAI("openai");

    var gpt41mini = aoai.AddDeployment(name: chatDeploymentName,
            modelName: "gpt-4.1-mini",
            modelVersion: "2025-04-14");
    gpt41mini.Resource.SkuCapacity = 10;
    gpt41mini.Resource.SkuName = "GlobalStandard";

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
