using Microsoft.EntityFrameworkCore;
using PaymentsService.Data;
using PaymentsService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add Aspire service defaults - this provides service discovery, health checks, logging, and telemetry
builder.AddServiceDefaults();

// Add services to the container
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddControllers();

// Add database context with SQLite for local development
// Connection string will be provided by Aspire host via configuration
builder.Services.AddDbContext<PaymentsDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("PaymentsDb") ??
                          "Data Source=Data/payments.db";
    options.UseSqlite(connectionString);
});

// Register repository services
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();

// Add Swagger for API documentation (optional)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Payments API", Version = "v1" });
});

// Add health checks for Aspire monitoring
builder.Services.AddHealthChecks()
    .AddDbContextCheck<PaymentsDbContext>();

var app = builder.Build();

// Map Aspire default endpoints for health checks and telemetry
app.MapDefaultEndpoints();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Map API controllers
app.MapControllers();

// Map Blazor pages
app.MapRazorPages();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

// Ensure database is created and apply migrations
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<PaymentsDbContext>();
    try
    {
        // For EF Core 9, prefer MigrateAsync when using migrations
        // For demo purposes with SQLite, we'll use EnsureCreated for simplicity
        await context.Database.EnsureCreatedAsync();
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while creating the database");
    }
}

app.Run();