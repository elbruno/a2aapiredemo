using CartEntities;

namespace Store.Tests;

public class CartCalculationTests
{
    [Fact]
    public void CartCalculation_SubtotalTaxTotalItemCount_CalculatedCorrectly()
    {
        // Arrange
        var cart = new Cart();
        cart.Items.Add(new CartItem 
        { 
            ProductId = 1, 
            Name = "Product 1", 
            Price = 10.00m, 
            Quantity = 2 
        });
        cart.Items.Add(new CartItem 
        { 
            ProductId = 2, 
            Name = "Product 2", 
            Price = 15.50m, 
            Quantity = 1 
        });

        // Act & Assert
        Assert.Equal(35.50m, cart.Subtotal); // (10.00 * 2) + (15.50 * 1)
        Assert.Equal(2.84m, cart.Tax); // 35.50 * 0.08 = 2.84
        Assert.Equal(38.34m, cart.Total); // 35.50 + 2.84 = 38.34
        Assert.Equal(3, cart.ItemCount); // 2 + 1 = 3
    }

    [Fact]
    public void CartCalculation_EmptyCart_ReturnsZeros()
    {
        // Arrange
        var cart = new Cart();

        // Act & Assert
        Assert.Equal(0m, cart.Subtotal);
        Assert.Equal(0m, cart.Tax);
        Assert.Equal(0m, cart.Total);
        Assert.Equal(0, cart.ItemCount);
    }

    [Fact]
    public void CartCalculation_SingleItem_CalculatedCorrectly()
    {
        // Arrange
        var cart = new Cart();
        cart.Items.Add(new CartItem 
        { 
            ProductId = 1, 
            Name = "Single Product", 
            Price = 100.00m, 
            Quantity = 1 
        });

        // Act & Assert
        Assert.Equal(100.00m, cart.Subtotal);
        Assert.Equal(8.00m, cart.Tax); // 100.00 * 0.08
        Assert.Equal(108.00m, cart.Total);
        Assert.Equal(1, cart.ItemCount);
    }

    [Fact]
    public void CartCalculation_MultipleQuantitiesSameItem_CalculatedCorrectly()
    {
        // Arrange
        var cart = new Cart();
        cart.Items.Add(new CartItem 
        { 
            ProductId = 1, 
            Name = "Bulk Product", 
            Price = 25.00m, 
            Quantity = 4 
        });

        // Act & Assert
        Assert.Equal(100.00m, cart.Subtotal); // 25.00 * 4
        Assert.Equal(8.00m, cart.Tax); // 100.00 * 0.08
        Assert.Equal(108.00m, cart.Total);
        Assert.Equal(4, cart.ItemCount);
    }

    [Fact]
    public void CartCalculation_DecimalPrices_CalculatedCorrectly()
    {
        // Arrange
        var cart = new Cart();
        cart.Items.Add(new CartItem 
        { 
            ProductId = 1, 
            Name = "Decimal Product", 
            Price = 12.99m, 
            Quantity = 3 
        });

        // Act & Assert
        Assert.Equal(38.97m, cart.Subtotal); // 12.99 * 3
        Assert.Equal(3.1176m, cart.Tax); // 38.97 * 0.08 = 3.1176
        Assert.Equal(42.0876m, cart.Total); // 38.97 + 3.1176
        Assert.Equal(3, cart.ItemCount);
    }
}