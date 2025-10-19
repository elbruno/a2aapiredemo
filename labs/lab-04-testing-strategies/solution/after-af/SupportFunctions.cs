/// <summary>
/// Support functions (no longer a plugin class!).
/// </summary>
public static class SupportFunctions
{
    public static string GetOrderStatus(string orderId)
    {
        // Mock implementation
        return $"Order {orderId}: Shipped, arriving Tuesday";
    }

    public static string ProcessRefund(string orderId, string reason)
    {
        // Mock implementation
        return $"Refund approved for order {orderId}. Reason: {reason}";
    }

    public static string GetProductInfo(string productId)
    {
        // Mock implementation
        return $"Product {productId}: Premium Widget, $99.99";
    }
}
