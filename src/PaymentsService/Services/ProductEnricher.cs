using Microsoft.Extensions.Options;

namespace PaymentsService.Services;

public interface IProductEnricher
{
    Task<Dictionary<string, object>?> EnrichPaymentAsync(Guid paymentId, List<DTOs.PaymentItem> items);
}

public class ProductEnricher : IProductEnricher
{
    private readonly HttpClient? _httpClient;
    private readonly ILogger<ProductEnricher> _logger;
    private readonly ProductEnricherOptions _options;

    public ProductEnricher(IHttpClientFactory? httpClientFactory, ILogger<ProductEnricher> logger, IOptions<ProductEnricherOptions> options)
    {
        _httpClient = httpClientFactory?.CreateClient("ProductsService");
        _logger = logger;
        _options = options.Value;
    }

    public async Task<Dictionary<string, object>?> EnrichPaymentAsync(Guid paymentId, List<DTOs.PaymentItem> items)
    {
        // Feature flag check
        if (!_options.EnableEnrichment || _httpClient == null)
        {
            _logger.LogDebug("Product enrichment disabled or HttpClient not configured");
            return null;
        }

        try
        {
            _logger.LogInformation("Enriching payment {PaymentId} with product information", paymentId);
            
            var enrichmentData = new Dictionary<string, object>();
            var productDetails = new List<object>();

            foreach (var item in items)
            {
                try
                {
                    // Call Products service to get product details
                    var response = await _httpClient.GetAsync($"/api/products/{item.ProductId}");
                    
                    if (response.IsSuccessStatusCode)
                    {
                        var productJson = await response.Content.ReadAsStringAsync();
                        var productData = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(productJson);
                        
                        if (productData != null)
                        {
                            productDetails.Add(new
                            {
                                ProductId = item.ProductId,
                                Quantity = item.Quantity,
                                UnitPrice = item.UnitPrice,
                                ProductDetails = productData
                            });
                        }
                    }
                    else
                    {
                        _logger.LogWarning("Failed to get product details for {ProductId}: {StatusCode}", 
                            item.ProductId, response.StatusCode);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error enriching product {ProductId}", item.ProductId);
                    // Continue with other items even if one fails
                }
            }

            if (productDetails.Any())
            {
                enrichmentData["products"] = productDetails;
                enrichmentData["enrichedAt"] = DateTime.UtcNow;
                
                _logger.LogInformation("Successfully enriched payment {PaymentId} with {Count} products", 
                    paymentId, productDetails.Count);
            }

            return enrichmentData.Any() ? enrichmentData : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error enriching payment {PaymentId}", paymentId);
            // Don't throw - enrichment is optional
            return null;
        }
    }
}

public class ProductEnricherOptions
{
    public bool EnableEnrichment { get; set; } = false;
    public string ProductsServiceBaseUrl { get; set; } = string.Empty;
    public int TimeoutSeconds { get; set; } = 30;
}