namespace PaymentsService.DTOs;

public class CreatePaymentResponse
{
    public Guid PaymentId { get; set; }
    
    public string Status { get; set; } = "Success";
    
    public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
}

public class PaymentResponse
{
    public Guid PaymentId { get; set; }
    
    public string UserId { get; set; } = string.Empty;
    
    public string? StoreId { get; set; }
    
    public string? CartId { get; set; }
    
    public string Currency { get; set; } = "USD";
    
    public decimal Amount { get; set; }
    
    public string Status { get; set; } = "Success";
    
    public string PaymentMethod { get; set; } = string.Empty;
    
    public PaymentItem[]? Items { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime ProcessedAt { get; set; }
}

public class GetPaymentsResponse
{
    public PaymentResponse[] Items { get; set; } = [];
    
    public int TotalCount { get; set; }
}