using System.Diagnostics;
using AgentServices;
using AgentServices.Checkout;
using AgentServices.Discount;
using AgentServices.Stock;
using Azure.Identity;
using Microsoft.Agents.AI.DevUI;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.AI;
using Store.Components;
using Store.Services;

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

// Configure Azure OpenAI for agentic checkout
var microsoftFoundryConnectionName = "microsoftfoundry";
var chatDeploymentName = builder.Configuration["AI_ChatDeploymentName"] ?? "gpt-5-mini";

var openai = builder.AddAzureOpenAIClient(connectionName: microsoftFoundryConnectionName,
    configureSettings: settings =>
    {
        if (string.IsNullOrEmpty(settings.Key))
        {
            settings.Credential = new DefaultAzureCredential();
        }
    });
openai.AddChatClient(chatDeploymentName)
    .UseFunctionInvocation()
    .UseOpenTelemetry(configure: c =>
    c.EnableSensitiveData = builder.Environment.IsDevelopment());

// Add Agent settings and agents
builder.AddAgentSettings();
builder.AddeShopLiteAIAgents();

// Add internal services for agents and orchestrator
builder.Services.AddScoped<IStockAgentService, StockAgentService>();
builder.Services.AddScoped<IDiscountAgentService, DiscountAgentService>();
builder.Services.AddScoped<IAgentCheckoutOrchestrator, AgentCheckoutOrchestrator>();


// Register services for OpenAI responses and conversations (also required for DevUI)
builder.Services.AddOpenAIResponses();
builder.Services.AddOpenAIConversations();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// aspire map default endpoints
app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();
app.UseStaticFiles();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

if (builder.Environment.IsDevelopment())
{
    // Map DevUI endpoint to /devui
    app.MapDevUI();
}

app.Logger.LogInformation("Starting Store app...");
app.Run();
