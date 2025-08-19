using Microsoft.SemanticKernel;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add Swagger for API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Semantic Kernel services
builder.Services.AddKernel();

// Add HTTP clients for external services (multi-agent system)
builder.Services.AddHttpClient("InventoryAgent", client =>
{
    client.BaseAddress = new Uri(builder.Configuration.GetConnectionString("MockServices") ?? "http://localhost:5010");
});

builder.Services.AddHttpClient("MatchmakingAgent", client =>
{
    client.BaseAddress = new Uri(builder.Configuration.GetConnectionString("MockServices") ?? "http://localhost:5010");
});

builder.Services.AddHttpClient("LocationAgent", client =>
{
    client.BaseAddress = new Uri(builder.Configuration.GetConnectionString("MockServices") ?? "http://localhost:5010");
});

builder.Services.AddHttpClient("NavigationAgent", client =>
{
    client.BaseAddress = new Uri(builder.Configuration.GetConnectionString("MockServices") ?? "http://localhost:5010");
});

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
