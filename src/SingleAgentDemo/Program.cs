using Microsoft.SemanticKernel;
using SingleAgentDemo.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddControllers();

// Add Swagger for API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Semantic Kernel services
builder.Services.AddKernel();

// Register service layer implementations for external services
builder.Services.AddHttpClient<IAnalyzePhotoService, AnalyzePhotoService>(
    client => client.BaseAddress = new Uri("https+http://analyzephotoservice"));

builder.Services.AddHttpClient<ICustomerInformationService, CustomerInformationService>(
    client => client.BaseAddress = new Uri("https+http://customerinformationservice"));

builder.Services.AddHttpClient<IToolReasoningService, ToolReasoningService>(
    client => client.BaseAddress = new Uri("https+http://toolreasoningservice"));

builder.Services.AddHttpClient<IInventoryService, InventoryService>(
    client => client.BaseAddress = new Uri("https+http://inventoryservice"));

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
