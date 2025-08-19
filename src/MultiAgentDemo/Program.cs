using Microsoft.SemanticKernel;
using MultiAgentDemo.Services;

// KernelAzureOpenAIConfigurator moved to its own file under Services to avoid mixing
// type declarations with top-level statements in Program.cs.

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddControllers();

// Add Swagger for API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Semantic Kernel services and configure Azure OpenAI connector when a connection string named 'openai' exists.
builder.Services.AddKernel();

// If an 'openai' connection string is configured, register a hosted configurator that will
// attempt to attach the Azure OpenAI connector to the kernel at runtime using reflection.
// This avoids compile-time references to IKernel or KernelOptions symbols which may vary
// between Semantic Kernel versions.
var openAiConnection = builder.Configuration.GetValue<string>("ConnectionStrings:openai");
if (!string.IsNullOrEmpty(openAiConnection))
{
    builder.Services.AddSingleton<Microsoft.Extensions.Hosting.IHostedService>(sp =>
        new KernelAzureOpenAIConfigurator(sp, openAiConnection));
}

// Register the Semantic Kernel provider adapter so controllers can request a kernel via
// ISemanticKernelProvider without depending on the compile-time IKernel symbol.
builder.Services.AddSingleton<MultiAgentDemo.Services.ISemanticKernelProvider, MultiAgentDemo.Services.SemanticKernelProvider>();

// Register service layer implementations for multi-agent external services
builder.Services.AddHttpClient<IInventoryAgentService, InventoryAgentService>(
    client => client.BaseAddress = new("https+http://inventoryservice"));

builder.Services.AddHttpClient<IMatchmakingAgentService, MatchmakingAgentService>(
    client => client.BaseAddress = new Uri("https+http://matchmakingservice"));

builder.Services.AddHttpClient<ILocationAgentService, LocationAgentService>(
    client => client.BaseAddress = new Uri("https+http://locationservice"));

builder.Services.AddHttpClient<INavigationAgentService, NavigationAgentService>(
    client => client.BaseAddress = new Uri("https+http://navigationservice"));

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
