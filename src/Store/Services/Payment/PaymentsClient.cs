using System.Text.Json;

namespace Store.Services.Payment;

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
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<PaymentResponse> ProcessPaymentAsync(CreatePaymentRequest request)
    {
        try
        {
            _logger.LogInformation("Processing payment for user {UserId}, amount {Amount} {Currency}", 
                request.UserId, request.Amount, request.Currency);

            var response = await _httpClient.PostAsJsonAsync("/api/payments", request, _jsonOptions);
            
            if (response.IsSuccessStatusCode)
            {
                var paymentResponse = await response.Content.ReadFromJsonAsync<PaymentResponse>(_jsonOptions);
                if (paymentResponse != null)
                {
                    _logger.LogInformation("Payment processed successfully: {PaymentId}", paymentResponse.PaymentId);
                    return paymentResponse;
                }
            }
            
            _logger.LogWarning("Payment processing failed with status {StatusCode}", response.StatusCode);
            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogWarning("Payment error response: {ErrorContent}", errorContent);
            
            return new PaymentResponse
            {
                PaymentId = string.Empty,
                Status = "Failed",
                ProcessedAt = DateTime.UtcNow,
                Message = $"Payment processing failed: {response.StatusCode}"
            };
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error processing payment for user {UserId}", request.UserId);
            return new PaymentResponse
            {
                PaymentId = string.Empty,
                Status = "Failed",
                ProcessedAt = DateTime.UtcNow,
                Message = "Network error: Unable to connect to payment service"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error processing payment for user {UserId}", request.UserId);
            return new PaymentResponse
            {
                PaymentId = string.Empty,
                Status = "Failed",
                ProcessedAt = DateTime.UtcNow,
                Message = "An unexpected error occurred"
            };
        }
    }

    public async Task<PaymentRecord?> GetPaymentAsync(string paymentId)
    {
        try
        {
            _logger.LogInformation("Retrieving payment {PaymentId}", paymentId);
            
            var response = await _httpClient.GetAsync($"/api/payments/{paymentId}");
            
            if (response.IsSuccessStatusCode)
            {
                var payment = await response.Content.ReadFromJsonAsync<PaymentRecord>(_jsonOptions);
                return payment;
            }
            
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogWarning("Payment {PaymentId} not found", paymentId);
                return null;
            }
            
            _logger.LogWarning("Failed to retrieve payment {PaymentId}: {StatusCode}", paymentId, response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving payment {PaymentId}", paymentId);
            return null;
        }
    }
}