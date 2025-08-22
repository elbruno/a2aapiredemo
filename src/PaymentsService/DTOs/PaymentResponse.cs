namespace PaymentsService.DTOs;

public class PaymentResponse
{
    public string PaymentId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime ProcessedAt { get; set; }
    public string? Message { get; set; }
}

public class PaymentListResponse
{
    public List<PaymentRecord> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
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
    public Dictionary<string, object>? ProductEnrichment { get; set; }
}