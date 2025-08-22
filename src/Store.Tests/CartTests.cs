using CartEntities;

namespace Store.Tests
{
    [TestClass]
    public sealed class CartTests
    {
        [TestMethod]
        public void CartCalculation_SubtotalTaxTotalItemCount_CalculatedCorrectly()
        {
            // Arrange
            var cart = new Cart();
            
            // Add first item: 2 units at $10.99 each = $21.98
            cart.Items.Add(new CartItem
            {
                ProductId = 1,
                Name = "Product 1",
                Description = "Description 1",
                Price = 10.99m,
                ImageUrl = "image1.jpg",
                Quantity = 2
            });

            // Add second item: 3 units at $15.50 each = $46.50
            cart.Items.Add(new CartItem
            {
                ProductId = 2,
                Name = "Product 2",
                Description = "Description 2",
                Price = 15.50m,
                ImageUrl = "image2.jpg",
                Quantity = 3
            });

            // Add third item: 1 unit at $5.25 = $5.25
            cart.Items.Add(new CartItem
            {
                ProductId = 3,
                Name = "Product 3",
                Description = "Description 3",
                Price = 5.25m,
                ImageUrl = "image3.jpg",
                Quantity = 1
            });

            // Act & Assert
            // Subtotal should be: $21.98 + $46.50 + $5.25 = $73.73
            Assert.AreEqual(73.73m, cart.Subtotal);

            // Tax should be: $73.73 * 0.08 = $5.8984
            Assert.AreEqual(5.8984m, cart.Tax);

            // Total should be: $73.73 + $5.8984 = $79.6284
            Assert.AreEqual(79.6284m, cart.Total);

            // Item count should be: 2 + 3 + 1 = 6
            Assert.AreEqual(6, cart.ItemCount);
        }

        [TestMethod]
        public void CartCalculation_EmptyCart_ReturnsZeroValues()
        {
            // Arrange
            var cart = new Cart();

            // Act & Assert
            Assert.AreEqual(0m, cart.Subtotal);
            Assert.AreEqual(0m, cart.Tax);
            Assert.AreEqual(0m, cart.Total);
            Assert.AreEqual(0, cart.ItemCount);
        }

        [TestMethod]
        public void CartCalculation_SingleItem_CalculatedCorrectly()
        {
            // Arrange
            var cart = new Cart();
            cart.Items.Add(new CartItem
            {
                ProductId = 1,
                Name = "Single Product",
                Description = "Single Description",
                Price = 100.00m,
                ImageUrl = "single.jpg",
                Quantity = 1
            });

            // Act & Assert
            Assert.AreEqual(100.00m, cart.Subtotal);
            Assert.AreEqual(8.00m, cart.Tax); // 100.00 * 0.08
            Assert.AreEqual(108.00m, cart.Total); // 100.00 + 8.00
            Assert.AreEqual(1, cart.ItemCount);
        }

        [TestMethod]
        public void CartCalculation_ZeroQuantityItems_NotIncludedInCalculations()
        {
            // Arrange
            var cart = new Cart();
            
            // Add item with positive quantity
            cart.Items.Add(new CartItem
            {
                ProductId = 1,
                Name = "Valid Product",
                Price = 20.00m,
                Quantity = 2
            });

            // Add item with zero quantity (should not affect calculations)
            cart.Items.Add(new CartItem
            {
                ProductId = 2,
                Name = "Zero Quantity Product",
                Price = 50.00m,
                Quantity = 0
            });

            // Act & Assert
            // Only the first item should be counted: 2 * $20.00 = $40.00
            Assert.AreEqual(40.00m, cart.Subtotal);
            Assert.AreEqual(3.20m, cart.Tax); // 40.00 * 0.08
            Assert.AreEqual(43.20m, cart.Total); // 40.00 + 3.20
            Assert.AreEqual(2, cart.ItemCount); // Only valid quantity items
        }

        [TestMethod]
        public void CartItem_Total_CalculatedCorrectly()
        {
            // Arrange
            var cartItem = new CartItem
            {
                ProductId = 1,
                Name = "Test Product",
                Price = 12.50m,
                Quantity = 4
            };

            // Act & Assert
            Assert.AreEqual(50.00m, cartItem.Total); // 12.50 * 4 = 50.00
        }

        [TestMethod]
        public void CartItem_Total_WithZeroQuantity_ReturnsZero()
        {
            // Arrange
            var cartItem = new CartItem
            {
                ProductId = 1,
                Name = "Test Product",
                Price = 12.50m,
                Quantity = 0
            };

            // Act & Assert
            Assert.AreEqual(0m, cartItem.Total);
        }

        [TestMethod]
        public void CartItem_Total_WithZeroPrice_ReturnsZero()
        {
            // Arrange
            var cartItem = new CartItem
            {
                ProductId = 1,
                Name = "Free Product",
                Price = 0m,
                Quantity = 5
            };

            // Act & Assert
            Assert.AreEqual(0m, cartItem.Total);
        }
    }
}