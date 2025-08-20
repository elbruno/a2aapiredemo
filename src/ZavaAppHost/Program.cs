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
var analyzePhotoService = builder.AddProject<Projects.AnalyzePhotoService>("analyzephotoservice")
    .WithExternalHttpEndpoints();

var customerInformationService = builder.AddProject<Projects.CustomerInformationService>("customerinformationservice")
    .WithExternalHttpEndpoints();

var toolReasoningService = builder.AddProject<Projects.ToolReasoningService>("toolreasoningservice")
    .WithExternalHttpEndpoints();

var inventoryService = builder.AddProject<Projects.InventoryService>("inventoryservice")
    .WithExternalHttpEndpoints();

// Add new multi-agent specific services
var matchmakingService = builder.AddProject<Projects.MatchmakingService>("matchmakingservice")
    .WithExternalHttpEndpoints();

var locationService = builder.AddProject<Projects.LocationService>("locationservice")
    .WithExternalHttpEndpoints();

var navigationService = builder.AddProject<Projects.NavigationService>("navigationservice")
    .WithExternalHttpEndpoints();

// Add new agent demo services
var singleAgentDemo = builder.AddProject<Projects.SingleAgentDemo>("singleagentdemo")
    .WithReference(analyzePhotoService)
    .WithReference(customerInformationService)
    .WithReference(toolReasoningService)
    .WithReference(inventoryService)
    .WithExternalHttpEndpoints();

var multiAgentDemo = builder.AddProject<Projects.MultiAgentDemo>("multiagentdemo")
    .WaitFor(analyzePhotoService)
    .WithReference(analyzePhotoService)
    .WaitFor(customerInformationService)
    .WithReference(customerInformationService)
    .WaitFor(toolReasoningService)
    .WithReference(toolReasoningService)
    .WaitFor(inventoryService)
    .WithReference(inventoryService)
    .WaitFor(matchmakingService)
    .WithReference(matchmakingService)
    .WaitFor(locationService)
    .WithReference(locationService)
    .WaitFor(navigationService)
    .WithReference(navigationService)
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
    var aoai = builder.AddAzureOpenAI("aifoundry");

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
    matchmakingService.WithReference(appInsights);
    locationService.WithReference(appInsights);
    navigationService.WithReference(appInsights);

    singleAgentDemo.WithReference(appInsights)
        .WithExternalHttpEndpoints();

    multiAgentDemo.WithReference(appInsights)
        .WithExternalHttpEndpoints();

    openai = aoai;
}
else
{
    openai = builder.AddConnectionString("aifoundry");
}

products.WithReference(openai)
    .WithEnvironment("AI_ChatDeploymentName", chatDeploymentName)
    .WithEnvironment("AI_embeddingsDeploymentName", embeddingsDeploymentName);

// Configure OpenAI for agent demo services and external services
analyzePhotoService.WithReference(openai)
    .WithEnvironment("AI_ChatDeploymentName", chatDeploymentName);
customerInformationService.WithReference(openai)
    .WithEnvironment("AI_ChatDeploymentName", chatDeploymentName);
toolReasoningService.WithReference(openai)
    .WithEnvironment("AI_ChatDeploymentName", chatDeploymentName);
inventoryService.WithReference(openai)
    .WithEnvironment("AI_ChatDeploymentName", chatDeploymentName);
matchmakingService.WithReference(openai)
    .WithEnvironment("AI_ChatDeploymentName", chatDeploymentName);
locationService.WithReference(openai)
    .WithEnvironment("AI_ChatDeploymentName", chatDeploymentName);
navigationService.WithReference(openai)
    .WithEnvironment("AI_ChatDeploymentName", chatDeploymentName);
singleAgentDemo.WithReference(openai)
    .WithEnvironment("AI_ChatDeploymentName", chatDeploymentName);
multiAgentDemo.WithReference(openai)
    .WithEnvironment("AI_ChatDeploymentName", chatDeploymentName);

builder.Build().Run();
