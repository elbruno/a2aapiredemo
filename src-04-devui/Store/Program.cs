using Store.Components;
using Store.Services;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Diagnostics;
using AgentServices;
using Azure.Identity;
using Microsoft.Agents.AI.DevUI;

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

// DEMO Step 4: Configure Azure OpenAI for agentic checkout
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

// DEMO Step 4: Register Agent Services using the Agent Framework approach
// This pattern is similar to how agents are registered in the AgentWebChat sample:
// https://github.com/microsoft/agent-framework/blob/main/dotnet/samples/AgentWebChat/AgentWebChat.AgentHost/Program.cs
// Benefits of this approach:
// 1. Proper lifecycle management (scoped services per request)
// 2. Centralized configuration via AgentSettings
// 3. Integration with DevUI for agent debugging
// 4. Support for OpenTelemetry tracing
builder.Services.AddAgentServices(builder.Configuration);

// DEMO Step 4: Add DevUI services for agent debugging in development
// DevUI provides a visual interface to:
// - Inspect agent reasoning and message flows
// - Debug multi-agent workflows
// - Test agent responses interactively
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDevUI();
    builder.Logging.SetMinimumLevel(LogLevel.Debug);
}

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

app.Logger.LogInformation("Store app built. Environment: {Environment}", app.Environment.EnvironmentName);
app.Logger.LogInformation("DEMO Step 4: Agentic checkout services registered with chat deployment: {ChatDeployment}", chatDeploymentName);
app.Logger.LogInformation("DEMO Step 4: Agent services registered via DI with scoped lifetime for request isolation");

// aspire map default endpoints
app.MapDefaultEndpoints();
app.Logger.LogInformation("Default endpoints mapped (health/alive in Development)");

// DEMO Step 4: Map DevUI endpoint for agent debugging in development
// Access at: https://localhost:[port]/devui when configured
if (app.Environment.IsDevelopment())
{
    app.MapDevUI();
    app.Logger.LogInformation("DEMO Step 4: DevUI endpoint mapped at /devui for agent debugging");
}

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
