using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var sqldb = builder.AddSqlServer("sql")
    .WithDataVolume()
    .AddDatabase("sqldb");

var products = builder.AddProject<Projects.Products>("products")
    .WithReference(sqldb)
    .WaitFor(sqldb);

var store = builder.AddProject<Projects.Store>("store")
    .WithReference(products)
    .WaitFor(products)
    .WithExternalHttpEndpoints();

var storeRealtime = builder.AddProject<Projects.StoreRealtime>("realtimestore")
    .WithReference(products)
    .WaitFor(products)
    .WithExternalHttpEndpoints();

// OpenAI resource configuration
IResourceBuilder<IResourceWithConnectionString>? openai;
var chatDeploymentName = "gpt-4o-mini";
var realtimeDeploymentName = "gpt-4o-realtime-preview";  // Updated to GA model name
var embeddingsDeploymentName = "text-embedding-ada-002";

if (builder.ExecutionContext.IsPublishMode)
{
    // production code uses Azure services, so we need to add them here
    var appInsights = builder.AddAzureApplicationInsights("appInsights");
    var aoai = builder.AddAzureOpenAI("openai")
        .AddDeployment(new AzureOpenAIDeployment(chatDeploymentName,
        "gpt-4o-mini",
        "2024-07-18",
        "GlobalStandard",
        10))
        .AddDeployment(new AzureOpenAIDeployment(realtimeDeploymentName,
        "gpt-4o-realtime-preview",  // Updated to GA model name
        "2024-12-17",  // Keep version until GA version is confirmed
        "GlobalStandard",
        1))
        .AddDeployment(new AzureOpenAIDeployment(embeddingsDeploymentName,
        "text-embedding-ada-002",
        "2"));

    products.WithReference(appInsights);
    storeRealtime.WithReference(appInsights);
    store.WithReference(appInsights);

    openai = aoai;
}
else
{
    // Development mode uses connection string approach
    openai = builder.AddConnectionString("openai");
}

// Configure OpenAI references for all projects that need it
products.WithReference(openai)
    .WithEnvironment("AI_ChatDeploymentName", chatDeploymentName)
    .WithEnvironment("AI_RealtimeDeploymentName", realtimeDeploymentName)
    .WithEnvironment("AI_embeddingsDeploymentName", embeddingsDeploymentName);

storeRealtime.WithReference(openai)
    .WithEnvironment("AI_ChatDeploymentName", chatDeploymentName)
    .WithEnvironment("AI_RealtimeDeploymentName", realtimeDeploymentName)
    .WithEnvironment("AI_embeddingsDeploymentName", embeddingsDeploymentName);

builder.Build().Run();
