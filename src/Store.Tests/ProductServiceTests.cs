using Store.Services;
using DataEntities;
using SearchEntities;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text.Json;

namespace Store.Tests
{
    [TestClass]
    public sealed class ProductServiceTests
    {
        private Mock<ILogger<ProductService>> _mockLogger;
        private Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private HttpClient _httpClient;
        private ProductService _productService;

        [TestInitialize]
        public void TestInit()
        {
            _mockLogger = new Mock<ILogger<ProductService>>();
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("https://test.example.com")
            };
            _productService = new ProductService(_httpClient, _mockLogger.Object);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _httpClient?.Dispose();
        }

        [TestMethod]
        public async Task GetProducts_ReturnsProducts_WhenHttpOk()
        {
            // Arrange
            var expectedProducts = new List<Product>
            {
                new Product { Id = 1, Name = "Product 1", Description = "Desc 1", Price = 10.99m, ImageUrl = "img1.jpg" },
                new Product { Id = 2, Name = "Product 2", Description = "Desc 2", Price = 20.99m, ImageUrl = "img2.jpg" }
            };

            var jsonResponse = JsonSerializer.Serialize(expectedProducts, ProductSerializerContext.Default.ListProduct);
            var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(jsonResponse, System.Text.Encoding.UTF8, "application/json")
            };

            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri.PathAndQuery == "/api/product"),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            // Act
            var result = await _productService.GetProducts();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("Product 1", result[0].Name);
            Assert.AreEqual("Product 2", result[1].Name);
        }

        [TestMethod]
        public async Task GetProducts_ReturnsEmpty_WhenHttpNotOkOrException()
        {
            // Arrange - HTTP 500 error
            var httpResponse = new HttpResponseMessage(HttpStatusCode.InternalServerError);

            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            // Act
            var result = await _productService.GetProducts();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public async Task GetProducts_ReturnsEmpty_WhenExceptionThrown()
        {
            // Arrange - Network exception
            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new HttpRequestException("Network error"));

            // Act
            var result = await _productService.GetProducts();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public async Task Search_UsesAiEndpoint_WhenSemanticSearchTrue()
        {
            // Arrange
            var expectedResponse = new SearchResponse 
            { 
                Response = "AI search results",
                Products = new List<Product> 
                { 
                    new Product { Id = 1, Name = "AI Product", Description = "AI Desc", Price = 99.99m, ImageUrl = "ai.jpg" } 
                }
            };

            var jsonResponse = JsonSerializer.Serialize(expectedResponse);
            var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(jsonResponse, System.Text.Encoding.UTF8, "application/json")
            };

            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri.PathAndQuery == "/api/aisearch/camping"),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            // Act
            var result = await _productService.Search("camping", semanticSearch: true);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("AI search results", result.Response);
            Assert.AreEqual(1, result.Products.Count);
            Assert.AreEqual("AI Product", result.Products[0].Name);
        }

        [TestMethod]
        public async Task Search_UsesStandardEndpoint_WhenSemanticSearchFalse()
        {
            // Arrange
            var expectedResponse = new SearchResponse 
            { 
                Response = "Standard search results",
                Products = new List<Product> 
                { 
                    new Product { Id = 2, Name = "Standard Product", Description = "Standard Desc", Price = 49.99m, ImageUrl = "standard.jpg" } 
                }
            };

            var jsonResponse = JsonSerializer.Serialize(expectedResponse);
            var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(jsonResponse, System.Text.Encoding.UTF8, "application/json")
            };

            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri.PathAndQuery == "/api/product/search/tent"),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            // Act
            var result = await _productService.Search("tent", semanticSearch: false);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Standard search results", result.Response);
            Assert.AreEqual(1, result.Products.Count);
            Assert.AreEqual("Standard Product", result.Products[0].Name);
        }

        [TestMethod]
        public async Task Search_ReturnsDefaultResponse_WhenHttpError()
        {
            // Arrange
            var httpResponse = new HttpResponseMessage(HttpStatusCode.BadRequest);

            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            // Act
            var result = await _productService.Search("test", semanticSearch: false);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("No response", result.Response);
            Assert.AreEqual(0, result.Products.Count);
        }

        [TestMethod]
        public async Task Search_ReturnsDefaultResponse_WhenExceptionThrown()
        {
            // Arrange
            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new HttpRequestException("Search error"));

            // Act
            var result = await _productService.Search("test", semanticSearch: true);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("No response", result.Response);
            Assert.AreEqual(0, result.Products.Count);
        }
    }
}