var builder = DistributedApplication.CreateBuilder(args);

var sql = builder.AddSqlServer("sql")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithImageTag("2025-latest")
    .WithEnvironment("ACCEPT_EULA", "Y");

var productsDb = sql
    .WithDataVolume()
    .AddDatabase("productsDb");

// Add PaymentsService database - using SQLite for local development
var paymentsDb = builder.AddConnectionString("PaymentsDb");

IResourceBuilder<IResourceWithConnectionString>? aifoundry;

var chatDeploymentName = "gpt-4.1-mini";
var embeddingsDeploymentName = "text-embedding-ada-002";

var products = builder.AddProject<Projects.Products>("products")
    .WithReference(productsDb)
    .WaitFor(productsDb);

// Register PaymentsService with Aspire for service discovery, health checks, and telemetry
var paymentsService = builder.AddProject<Projects.PaymentsService>("payments-service")
    .WithReference(paymentsDb)
    .WithEnvironment("ASPNETCORE_URLS", "http://localhost:5004");

var store = builder.AddProject<Projects.Store>("store")
    .WithReference(products)
    .WithReference(paymentsService) // Store can discover PaymentsService via Aspire
    .WaitFor(products)
    .WithExternalHttpEndpoints();

if (builder.ExecutionContext.IsPublishMode)
{
    // production code uses Azure services, so we need to add them here
    var appInsights = builder.AddAzureApplicationInsights("appInsights");
    var aoai = builder.AddAzureOpenAI("aifoundry");

    var gpt5mini = aoai.AddDeployment(name: chatDeploymentName,
            modelName: "gpt-4.1-mini",
            modelVersion: "2025-04-14");
    gpt5mini.Resource.SkuName = "GlobalStandard";

    var embeddingsDeployment = aoai.AddDeployment(name: embeddingsDeploymentName,
        modelName: "text-embedding-ada-002",
        modelVersion: "1");

    products.WithReference(appInsights);

    store.WithReference(appInsights)
        .WithExternalHttpEndpoints();

    aifoundry = aoai;
}
else
{
    aifoundry = builder.AddConnectionString("aifoundry");
}

// Configure OpenAI references for all services that need AI capabilities
products.WithReference(aifoundry)
    .WithEnvironment("AI_ChatDeploymentName", chatDeploymentName)
    .WithEnvironment("AI_embeddingsDeploymentName", embeddingsDeploymentName);

builder.Build().Run();
