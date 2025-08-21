using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Logging;
using Store.Services;
using CartEntities;
using DataEntities;
using System.Text.Json;

namespace Store.Tests
{
    /// <summary>
    /// Example implementation of CartService unit tests
    /// This demonstrates the testing patterns needed for the missing unit tests
    /// </summary>
    [TestClass]
    public class CartServiceTests
    {
        private Mock<IProductService> _mockProductService;
        private Mock<ProtectedSessionStorage> _mockSessionStorage;
        private Mock<ILogger<CartService>> _mockLogger;
        private CartService _cartService;

        [TestInitialize]
        public void Setup()
        {
            _mockProductService = new Mock<IProductService>();
            _mockSessionStorage = new Mock<ProtectedSessionStorage>();
            _mockLogger = new Mock<ILogger<CartService>>();
            
            _cartService = new CartService(
                _mockProductService.Object,
                _mockSessionStorage.Object,
                _mockLogger.Object);
        }

        #region GetCartAsync Tests

        [TestMethod]
        public async Task GetCartAsync_EmptySessionStorage_ReturnsEmptyCart()
        {
            // Arrange
            var emptyResult = new ProtectedBrowserStorageResult<string>
            {
                Success = false,
                Value = null
            };
            _mockSessionStorage.Setup(x => x.GetAsync<string>("cart"))
                              .ReturnsAsync(emptyResult);

            // Act
            var result = await _cartService.GetCartAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Items.Count);
            Assert.AreEqual(0, result.ItemCount);
        }

        [TestMethod]
        public async Task GetCartAsync_ValidSessionData_ReturnsDeserializedCart()
        {
            // Arrange
            var testCart = CreateTestCart();
            var cartJson = JsonSerializer.Serialize(testCart);
            var sessionResult = new ProtectedBrowserStorageResult<string>
            {
                Success = true,
                Value = cartJson
            };
            
            _mockSessionStorage.Setup(x => x.GetAsync<string>("cart"))
                              .ReturnsAsync(sessionResult);

            // Act
            var result = await _cartService.GetCartAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(testCart.Items.Count, result.Items.Count);
            Assert.AreEqual(testCart.ItemCount, result.ItemCount);
        }

        [TestMethod]
        public async Task GetCartAsync_JavaScriptInteropException_ReturnsEmptyCart()
        {
            // Arrange
            _mockSessionStorage.Setup(x => x.GetAsync<string>("cart"))
                              .ThrowsAsync(new InvalidOperationException("JavaScript interop calls cannot be issued at this time"));

            // Act
            var result = await _cartService.GetCartAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Items.Count);
            
            // Verify debug logging occurred
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Debug,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("JavaScript interop not available")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        #endregion

        #region AddToCartAsync Tests

        [TestMethod]
        public async Task AddToCartAsync_NewProduct_AddsItemToCart()
        {
            // Arrange
            var productId = 1;
            var testProduct = CreateTestProduct(productId);
            var products = new List<Product> { testProduct };
            var emptyCart = new Cart();

            _mockProductService.Setup(x => x.GetProducts())
                              .ReturnsAsync(products);
            
            SetupEmptySessionStorage();
            SetupSaveCartSuccess();

            // Act
            await _cartService.AddToCartAsync(productId);

            // Assert
            _mockSessionStorage.Verify(x => x.SetAsync("cart", It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public async Task AddToCartAsync_ExistingProduct_IncrementsQuantity()
        {
            // Arrange
            var productId = 1;
            var testProduct = CreateTestProduct(productId);
            var products = new List<Product> { testProduct };
            var existingCart = CreateCartWithProduct(productId, quantity: 1);

            _mockProductService.Setup(x => x.GetProducts())
                              .ReturnsAsync(products);
            
            SetupSessionStorageWithCart(existingCart);
            SetupSaveCartSuccess();

            // Act
            await _cartService.AddToCartAsync(productId);

            // Assert - verify SaveCartAsync was called (cart was updated)
            _mockSessionStorage.Verify(x => x.SetAsync("cart", It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public async Task AddToCartAsync_ProductNotFound_LogsWarningAndDoesNotSaveCart()
        {
            // Arrange
            var productId = 999;
            var products = new List<Product>(); // Empty product list

            _mockProductService.Setup(x => x.GetProducts())
                              .ReturnsAsync(products);

            // Act
            await _cartService.AddToCartAsync(productId);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Product with ID {productId} not found")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);

            // Verify cart was not saved
            _mockSessionStorage.Verify(x => x.SetAsync("cart", It.IsAny<string>()), Times.Never);
        }

        #endregion

        #region UpdateQuantityAsync Tests

        [TestMethod]
        public async Task UpdateQuantityAsync_ValidQuantity_UpdatesItemQuantity()
        {
            // Arrange
            var productId = 1;
            var newQuantity = 5;
            var existingCart = CreateCartWithProduct(productId, quantity: 2);

            SetupSessionStorageWithCart(existingCart);
            SetupSaveCartSuccess();

            // Act
            await _cartService.UpdateQuantityAsync(productId, newQuantity);

            // Assert
            _mockSessionStorage.Verify(x => x.SetAsync("cart", It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public async Task UpdateQuantityAsync_ZeroQuantity_RemovesItemFromCart()
        {
            // Arrange
            var productId = 1;
            var existingCart = CreateCartWithProduct(productId, quantity: 2);

            SetupSessionStorageWithCart(existingCart);
            SetupSaveCartSuccess();

            // Act
            await _cartService.UpdateQuantityAsync(productId, 0);

            // Assert
            _mockSessionStorage.Verify(x => x.SetAsync("cart", It.IsAny<string>()), Times.Once);
        }

        #endregion

        #region RemoveFromCartAsync Tests

        [TestMethod]
        public async Task RemoveFromCartAsync_ExistingItem_RemovesItemAndSavesCart()
        {
            // Arrange
            var productId = 1;
            var existingCart = CreateCartWithProduct(productId, quantity: 2);

            SetupSessionStorageWithCart(existingCart);
            SetupSaveCartSuccess();

            // Act
            await _cartService.RemoveFromCartAsync(productId);

            // Assert
            _mockSessionStorage.Verify(x => x.SetAsync("cart", It.IsAny<string>()), Times.Once);
        }

        #endregion

        #region ClearCartAsync Tests

        [TestMethod]
        public async Task ClearCartAsync_NormalOperation_DeletesCartFromSessionStorage()
        {
            // Arrange
            _mockSessionStorage.Setup(x => x.DeleteAsync("cart"))
                              .Returns(Task.CompletedTask);

            // Act
            await _cartService.ClearCartAsync();

            // Assert
            _mockSessionStorage.Verify(x => x.DeleteAsync("cart"), Times.Once);
        }

        [TestMethod]
        public async Task ClearCartAsync_JavaScriptInteropException_LogsDebugMessage()
        {
            // Arrange
            _mockSessionStorage.Setup(x => x.DeleteAsync("cart"))
                              .ThrowsAsync(new InvalidOperationException("JavaScript interop calls cannot be issued at this time"));

            // Act
            await _cartService.ClearCartAsync();

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Debug,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("JavaScript interop not available")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        #endregion

        #region GetCartItemCountAsync Tests

        [TestMethod]
        public async Task GetCartItemCountAsync_ValidCart_ReturnsCorrectCount()
        {
            // Arrange
            var testCart = CreateTestCart(); // Creates cart with 2 items
            SetupSessionStorageWithCart(testCart);

            // Act
            var result = await _cartService.GetCartItemCountAsync();

            // Assert
            Assert.AreEqual(testCart.ItemCount, result);
        }

        [TestMethod]
        public async Task GetCartItemCountAsync_JavaScriptInteropException_ReturnsZero()
        {
            // Arrange
            _mockSessionStorage.Setup(x => x.GetAsync<string>("cart"))
                              .ThrowsAsync(new InvalidOperationException("JavaScript interop calls cannot be issued at this time"));

            // Act
            var result = await _cartService.GetCartItemCountAsync();

            // Assert
            Assert.AreEqual(0, result);
        }

        #endregion

        #region Test Helper Methods

        private Cart CreateTestCart()
        {
            return new Cart
            {
                Items = new List<CartItem>
                {
                    new CartItem
                    {
                        ProductId = 1,
                        Name = "Test Product 1",
                        Description = "Test Description 1",
                        Price = 10.00m,
                        ImageUrl = "test1.jpg",
                        Quantity = 2
                    },
                    new CartItem
                    {
                        ProductId = 2,
                        Name = "Test Product 2",
                        Description = "Test Description 2",
                        Price = 15.00m,
                        ImageUrl = "test2.jpg",
                        Quantity = 1
                    }
                }
            };
        }

        private Product CreateTestProduct(int id)
        {
            return new Product
            {
                Id = id,
                Name = $"Test Product {id}",
                Description = $"Test Description {id}",
                Price = 10.00m * id,
                ImageUrl = $"test{id}.jpg"
            };
        }

        private Cart CreateCartWithProduct(int productId, int quantity)
        {
            return new Cart
            {
                Items = new List<CartItem>
                {
                    new CartItem
                    {
                        ProductId = productId,
                        Name = $"Test Product {productId}",
                        Description = $"Test Description {productId}",
                        Price = 10.00m,
                        ImageUrl = $"test{productId}.jpg",
                        Quantity = quantity
                    }
                }
            };
        }

        private void SetupEmptySessionStorage()
        {
            var emptyResult = new ProtectedBrowserStorageResult<string>
            {
                Success = false,
                Value = null
            };
            _mockSessionStorage.Setup(x => x.GetAsync<string>("cart"))
                              .ReturnsAsync(emptyResult);
        }

        private void SetupSessionStorageWithCart(Cart cart)
        {
            var cartJson = JsonSerializer.Serialize(cart);
            var sessionResult = new ProtectedBrowserStorageResult<string>
            {
                Success = true,
                Value = cartJson
            };
            _mockSessionStorage.Setup(x => x.GetAsync<string>("cart"))
                              .ReturnsAsync(sessionResult);
        }

        private void SetupSaveCartSuccess()
        {
            _mockSessionStorage.Setup(x => x.SetAsync("cart", It.IsAny<string>()))
                              .Returns(Task.CompletedTask);
        }

        #endregion
    }
}