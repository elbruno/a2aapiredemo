using Store.Components;
using Store.Services;
using Store.Services.Payment;

var builder = WebApplication.CreateBuilder(args);

// add aspire service defaults
builder.AddServiceDefaults();

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<ICheckoutService, CheckoutService>();

builder.Services.AddHttpClient<IProductService, ProductService>(
    static client => client.BaseAddress = new("https+http://products"));

// Add PaymentsClient with Aspire service discovery
builder.Services.AddHttpClient<IPaymentsClient, PaymentsClient>(client =>
{
    // Use Aspire service discovery for PaymentsService
    client.BaseAddress = new Uri("https+http://payments-service");
});

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
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
