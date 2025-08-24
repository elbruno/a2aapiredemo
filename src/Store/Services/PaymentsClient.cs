using System.Text.Json;

namespace Store.Services;

// DTOs that match the PaymentsService DTOs
public class CreatePaymentRequest
{
    public string? StoreId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string? CartId { get; set; }
    public string Currency { get; set; } = "USD";
    public decimal Amount { get; set; }
    public PaymentItem[] Items { get; set; } = [];
    public string PaymentMethod { get; set; } = string.Empty;
    public object? Metadata { get; set; }
}

public class PaymentItem
{
    public string ProductId { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

public class CreatePaymentResponse
{
    public Guid PaymentId { get; set; }
    public string Status { get; set; } = "Success";
    public DateTime ProcessedAt { get; set; }
}

public interface IPaymentsClient
{
    Task<CreatePaymentResponse?> CreatePaymentAsync(CreatePaymentRequest request);
}

public class PaymentsClient : IPaymentsClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<PaymentsClient> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public PaymentsClient(HttpClient httpClient, ILogger<PaymentsClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public async Task<CreatePaymentResponse?> CreatePaymentAsync(CreatePaymentRequest request)
    {
        try
        {
            _logger.LogInformation("Creating payment for user {UserId} with amount {Amount} {Currency}",
                request.UserId, request.Amount, request.Currency);

            var json = JsonSerializer.Serialize(request, _jsonOptions);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/api/payments", content);

            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<CreatePaymentResponse>(responseJson, _jsonOptions);
                
                _logger.LogInformation("Payment created successfully with ID {PaymentId}", result?.PaymentId);
                return result;
            }
            else
            {
                _logger.LogWarning("Payment creation failed with status {StatusCode}: {Content}",
                    response.StatusCode, await response.Content.ReadAsStringAsync());
                return null;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating payment for user {UserId}", request.UserId);
            return null;
        }
    }
}