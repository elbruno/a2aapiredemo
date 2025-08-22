using CartEntities;

namespace Store.Services.Payment;

public interface IPaymentsClient
{
    Task<PaymentResponse> ProcessPaymentAsync(CreatePaymentRequest request);
    Task<PaymentRecord?> GetPaymentAsync(string paymentId);
}

public class CreatePaymentRequest
{
    public string? StoreId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string? CartId { get; set; }
    public string Currency { get; set; } = "USD";
    public decimal Amount { get; set; }
    public List<PaymentItem> Items { get; set; } = new();
    public string PaymentMethod { get; set; } = string.Empty;
    public object? Metadata { get; set; }
}

public class PaymentItem
{
    public string ProductId { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

public class PaymentResponse
{
    public string PaymentId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime ProcessedAt { get; set; }
    public string? Message { get; set; }
}

public class PaymentRecord
{
    public string PaymentId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string? StoreId { get; set; }
    public string? CartId { get; set; }
    public string Currency { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime ProcessedAt { get; set; }
    public List<PaymentItem> Items { get; set; } = new();
}