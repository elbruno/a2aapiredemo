using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace PaymentsService.Models;

public class PaymentRecord
{
    [Key]
    public Guid PaymentId { get; set; }
    
    [Required]
    public string UserId { get; set; } = string.Empty;
    
    public string? StoreId { get; set; }
    
    public string? CartId { get; set; }
    
    [Required]
    public string Currency { get; set; } = "USD";
    
    [Required]
    public decimal Amount { get; set; }
    
    [Required]
    public string Status { get; set; } = string.Empty;
    
    [Required]
    public string PaymentMethod { get; set; } = string.Empty;
    
    [Required]
    public string ItemsJson { get; set; } = "[]";
    
    public string? ProductEnrichmentJson { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime ProcessedAt { get; set; }
    
    // Navigation properties for convenience (not stored in DB)
    public List<DTOs.PaymentItem> Items 
    { 
        get => string.IsNullOrEmpty(ItemsJson) 
            ? new List<DTOs.PaymentItem>() 
            : JsonSerializer.Deserialize<List<DTOs.PaymentItem>>(ItemsJson) ?? new List<DTOs.PaymentItem>();
        set => ItemsJson = JsonSerializer.Serialize(value);
    }
    
    public Dictionary<string, object>? ProductEnrichment
    {
        get => string.IsNullOrEmpty(ProductEnrichmentJson) 
            ? null 
            : JsonSerializer.Deserialize<Dictionary<string, object>>(ProductEnrichmentJson);
        set => ProductEnrichmentJson = value == null ? null : JsonSerializer.Serialize(value);
    }
}