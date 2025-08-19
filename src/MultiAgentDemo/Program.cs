using Microsoft.SemanticKernel;
using MultiAgentDemo.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddControllers();

// Add Swagger for API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Semantic Kernel services
builder.Services.AddKernel();

// Register service layer implementations for multi-agent external services
builder.Services.AddHttpClient<IInventoryAgentService, InventoryAgentService>(
    client => client.BaseAddress = new Uri(builder.Configuration.GetConnectionString("MockServices") ?? "http://localhost:5010"));

builder.Services.AddHttpClient<IMatchmakingAgentService, MatchmakingAgentService>(
    client => client.BaseAddress = new Uri(builder.Configuration.GetConnectionString("MockServices") ?? "http://localhost:5010"));

builder.Services.AddHttpClient<ILocationAgentService, LocationAgentService>(
    client => client.BaseAddress = new Uri(builder.Configuration.GetConnectionString("MockServices") ?? "http://localhost:5010"));

builder.Services.AddHttpClient<INavigationAgentService, NavigationAgentService>(
    client => client.BaseAddress = new Uri(builder.Configuration.GetConnectionString("MockServices") ?? "http://localhost:5010"));

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
