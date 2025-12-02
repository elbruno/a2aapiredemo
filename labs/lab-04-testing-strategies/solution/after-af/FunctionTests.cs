/// <summary>
/// Tests for individual functions (unit tests without AI).
/// </summary>
public class FunctionTests
{
    [Fact]
    public void GetOrderStatus_ReturnsCorrectFormat()
    {
        // Arrange
        var orderId = "12345";

        // Act
        var result = SupportFunctions.GetOrderStatus(orderId);

        // Assert
        Assert.Contains(orderId, result);
        Assert.Contains("Order", result);
    }

    [Theory]
    [InlineData("12345")]
    [InlineData("67890")]
    [InlineData("ABC123")]
    public void GetOrderStatus_HandlesVariousOrderIds(string orderId)
    {
        // Arrange & Act
        var result = SupportFunctions.GetOrderStatus(orderId);

        // Assert
        Assert.NotNull(result);
        Assert.Contains(orderId, result);
    }

    [Fact]
    public void ProcessRefund_ReturnsConfirmation()
    {
        // Arrange
        var orderId = "12345";
        var reason = "Defective item";

        // Act
        var result = SupportFunctions.ProcessRefund(orderId, reason);

        // Assert
        Assert.Contains("approved", result, StringComparison.OrdinalIgnoreCase);
        Assert.Contains(orderId, result);
        Assert.Contains(reason, result);
    }

    [Fact]
    public void GetProductInfo_ReturnsProductDetails()
    {
        // Arrange
        var productId = "WIDGET-001";

        // Act
        var result = SupportFunctions.GetProductInfo(productId);

        // Assert
        Assert.Contains(productId, result);
        Assert.Contains("Product", result);
    }
}
