using Microsoft.SemanticKernel;
using System.ComponentModel;

/// <summary>
/// Plugin providing customer support functions.
/// </summary>
public sealed class SupportPlugin
{
    [KernelFunction]
    [Description("Get order status by order ID")]
    public string GetOrderStatus(string orderId)
    {
        // Mock implementation
        return $"Order {orderId}: Shipped, arriving Tuesday";
    }

    [KernelFunction]
    [Description("Process a refund for an order")]
    public string ProcessRefund(string orderId, string reason)
    {
        // Mock implementation
        return $"Refund approved for order {orderId}. Reason: {reason}";
    }

    [KernelFunction]
    [Description("Get product information")]
    public string GetProductInfo(string productId)
    {
        // Mock implementation
        return $"Product {productId}: Premium Widget, $99.99";
    }
}
