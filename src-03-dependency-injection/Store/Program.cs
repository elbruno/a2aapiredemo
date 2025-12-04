using Store.Components;
using Store.Services;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Diagnostics;
using AgentServices;
using Azure.Identity;

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

// DEMO Step 3: Configure Azure OpenAI for agentic checkout
var microsoftFoundryConnectionName = "microsoftfoundry";
var chatDeploymentName = builder.Configuration["AI_ChatDeploymentName"] ?? "gpt-5-mini";

builder.AddAzureOpenAIClient(connectionName: microsoftFoundryConnectionName,
    configureSettings: settings =>
    {
        if (string.IsNullOrEmpty(settings.Key))
        {
            settings.Credential = new DefaultAzureCredential();
        }
    }).AddChatClient(chatDeploymentName);

// DEMO Step 3: Register Agent Services using DI with proper abstractions
// This approach provides several benefits:
// 1. Easier testing through interface injection
// 2. Proper lifecycle management (scoped services)
// 3. Centralized configuration through AgentSettings
// 4. Support for OpenTelemetry and DevUI debugging
builder.Services.AddAgentServices(builder.Configuration);

// DEMO Step 3: Add DevUI for agent debugging in development
// DevUI provides a visual interface to:
// - Inspect agent reasoning and message flows
// - Debug multi-agent workflows
// - Test agent responses interactively
// Note: DevUI requires Microsoft.Agents.AI.DevUI package and additional setup
// Access at: https://localhost:[port]/devui when configured
if (builder.Environment.IsDevelopment())
{
    // DevUI is available via builder.AddDevUI() when using Microsoft.Agents.AI.Hosting patterns
    // For this demo, we use the existing AgentServices pattern with enhanced logging
    builder.Logging.SetMinimumLevel(LogLevel.Debug);
}

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

app.Logger.LogInformation("Store app built. Environment: {Environment}", app.Environment.EnvironmentName);
app.Logger.LogInformation("DEMO Step 3: Agentic checkout services registered with chat deployment: {ChatDeployment}", chatDeploymentName);
app.Logger.LogInformation("DEMO Step 3: Agent services registered via DI with scoped lifetime for request isolation");

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
