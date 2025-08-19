using Azure.Provisioning.CognitiveServices;

var builder = DistributedApplication.CreateBuilder(args);

var sql = builder.AddSqlServer("sql")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithImageTag("2025-latest")
    .WithEnvironment("ACCEPT_EULA", "Y");

var productsDb = sql
    .WithDataVolume()
    .AddDatabase("productsDb");

IResourceBuilder<IResourceWithConnectionString>? openai;
var chatDeploymentName = "gpt-5-mini";
var embeddingsDeploymentName = "text-embedding-ada-002";

var products = builder.AddProject<Projects.Products>("products")
    .WithReference(productsDb)
    .WaitFor(productsDb);


// Add new microservices for agent functionality
var analyzePhotoService = builder.AddProject<Projects.AnalyzePhotoService>("analyze-photo-service")
    .WithExternalHttpEndpoints();

var customerInformationService = builder.AddProject<Projects.CustomerInformationService>("customer-information-service")
    .WithExternalHttpEndpoints();

var toolReasoningService = builder.AddProject<Projects.ToolReasoningService>("tool-reasoning-service")
    .WithExternalHttpEndpoints();

var inventoryService = builder.AddProject<Projects.InventoryService>("inventory-service")
    .WithExternalHttpEndpoints();

// Add new agent demo services
var singleAgentDemo = builder.AddProject<Projects.SingleAgentDemo>("single-agent-demo")
    .WithReference(analyzePhotoService)
    .WithReference(customerInformationService)
    .WithReference(toolReasoningService)
    .WithReference(inventoryService)
    .WithExternalHttpEndpoints();

var multiAgentDemo = builder.AddProject<Projects.MultiAgentDemo>("multi-agent-demo")
    .WithExternalHttpEndpoints();

var store = builder.AddProject<Projects.Store>("store")
    .WithReference(products)
    .WaitFor(products)
    .WithReference(singleAgentDemo)
    .WaitFor(singleAgentDemo)
    .WithReference(multiAgentDemo)
    .WaitFor(multiAgentDemo)
    .WithExternalHttpEndpoints();


if (builder.ExecutionContext.IsPublishMode)
{
    // production code uses Azure services, so we need to add them here
    var appInsights = builder.AddAzureApplicationInsights("appInsights");
    var aoai = builder.AddAzureOpenAI("openai");

    var gpt5mini = aoai.AddDeployment(name: chatDeploymentName,
            modelName: "gpt-5-mini",
            modelVersion: "2025-08-07");    
    gpt5mini.Resource.SkuName = "GlobalStandard";

    var embeddingsDeployment = aoai.AddDeployment(name: embeddingsDeploymentName,
        modelName: "text-embedding-ada-002",
        modelVersion: "1");

    products.WithReference(appInsights);

    store.WithReference(appInsights)
        .WithExternalHttpEndpoints();

    // Add Application Insights to microservices
    analyzePhotoService.WithReference(appInsights);
    customerInformationService.WithReference(appInsights);
    toolReasoningService.WithReference(appInsights);
    inventoryService.WithReference(appInsights);

    singleAgentDemo.WithReference(appInsights)
        .WithExternalHttpEndpoints();

    multiAgentDemo.WithReference(appInsights)
        .WithExternalHttpEndpoints();

    openai = aoai;
}
else
{
    openai = builder.AddConnectionString("openai");
}

products.WithReference(openai)
    .WithEnvironment("AI_ChatDeploymentName", chatDeploymentName)
    .WithEnvironment("AI_embeddingsDeploymentName", embeddingsDeploymentName);

// Configure OpenAI for agent demo services and reasoning service
singleAgentDemo.WithReference(openai)
    .WithEnvironment("AI_ChatDeploymentName", chatDeploymentName);

multiAgentDemo.WithReference(openai)
    .WithEnvironment("AI_ChatDeploymentName", chatDeploymentName);

toolReasoningService.WithReference(openai)
    .WithEnvironment("AI_ChatDeploymentName", chatDeploymentName);

builder.Build().Run();
