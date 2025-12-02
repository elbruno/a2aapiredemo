using Store.Components;
using Store.Services;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// add aspire service defaults
builder.AddServiceDefaults();

// extra console logging to help diagnose startup issues
builder.Logging.AddConsole();

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<ICheckoutService, CheckoutService>();
builder.Services.AddSingleton<ICustomerService, CustomerService>();

// Register protected session storage used by CartService
builder.Services.AddScoped<ProtectedSessionStorage>();

builder.Services.AddHttpClient<IProductService, ProductService>(
    static client => client.BaseAddress = new("http://products"));

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

app.Logger.LogInformation("Store app built. Environment: {Environment}", app.Environment.EnvironmentName);

// aspire map default endpoints
app.MapDefaultEndpoints();
app.Logger.LogInformation("Default endpoints mapped (health/alive in Development)");

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

// request logging middleware to understand blank page scenarios
app.Use(async (context, next) =>
{
    var sw = Stopwatch.StartNew();
    app.Logger.LogInformation("Handling request {Method} {Path}", context.Request.Method, context.Request.Path);
    try
    {
        await next();
        app.Logger.LogInformation("Completed request {Path} with {StatusCode} in {Elapsed}ms", context.Request.Path, context.Response.StatusCode, sw.ElapsedMilliseconds);
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "Error processing request {Path}", context.Request.Path);
        throw;
    }
});

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
app.Logger.LogInformation("Razor components mapped, Home page should be available at '/'");

app.Logger.LogInformation("Starting Store app...");
app.Run();
