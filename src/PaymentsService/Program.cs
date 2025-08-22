using Microsoft.EntityFrameworkCore;
using PaymentsService.Data;
using PaymentsService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add Aspire service defaults - registers with Aspire for service discovery, health checks, and telemetry
builder.AddServiceDefaults();

// Add services to the container
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Payments Service API", Version = "v1" });
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "PaymentsService.xml"), true);
});

// Add Entity Framework with SQLite for local development
// Connection string will be provided by Aspire host via configuration
var connectionString = builder.Configuration.GetConnectionString("PaymentsDb") 
    ?? "Data Source=Data/payments.db";

builder.Services.AddDbContext<PaymentsDbContext>(options =>
    options.UseSqlite(connectionString));

// Register repository services
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();

// Register product enrichment service (optional feature)
builder.Services.Configure<ProductEnricherOptions>(
    builder.Configuration.GetSection("ProductEnrichment"));
builder.Services.AddScoped<IProductEnricher, ProductEnricher>();

// Optional: Add HttpClient for Products service if enrichment is enabled
if (builder.Configuration.GetValue<bool>("ProductEnrichment:EnableEnrichment"))
{
    builder.Services.AddHttpClient("ProductsService", client =>
    {
        var baseUrl = builder.Configuration["ProductEnrichment:ProductsServiceBaseUrl"];
        if (!string.IsNullOrEmpty(baseUrl))
        {
            client.BaseAddress = new Uri(baseUrl);
        }
    });
}

// Add Blazor Server components for the UI
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Map Aspire default endpoints for health checks and telemetry
app.MapDefaultEndpoints();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Payments Service API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.UseAuthorization();

// Map API controllers
app.MapControllers();

// Map Blazor components for UI
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Ensure database is created and migrated
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<PaymentsDbContext>();
    try
    {
        context.Database.EnsureCreated();
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Error creating database");
    }
}

app.Run();

// Make the implicit Program class public for testing
public partial class Program { }