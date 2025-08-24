using Microsoft.EntityFrameworkCore;
using PaymentsService.Components;
using PaymentsService.Data;
using PaymentsService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add Aspire service defaults - provides service discovery, telemetry, health checks
builder.AddServiceDefaults();

// Add Entity Framework with SQLite for local development
// Connection string will be provided by Aspire host via configuration
var connectionString = builder.Configuration.GetConnectionString("PaymentsDb") 
                       ?? "Data Source=Data/payments.db";

builder.Services.AddDbContext<PaymentsDbContext>(options =>
    options.UseSqlite(connectionString));

// Add repository for payment data access
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();

// Add controllers for API endpoints
builder.Services.AddControllers();

// Add Swagger/OpenAPI support for development
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
}

// Add Blazor services for UI
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Map Aspire default endpoints (health checks, etc.)
app.MapDefaultEndpoints();

// Ensure database is created and migrated
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<PaymentsDbContext>();
    // Use EnsureCreated for local development - in production, use proper migrations
    await context.Database.EnsureCreatedAsync();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    // Enable Swagger in development
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

// Map API controllers
app.MapControllers();

// Map Blazor components
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
