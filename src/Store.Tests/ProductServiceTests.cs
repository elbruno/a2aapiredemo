using System.Net;
using System.Text;
using System.Text.Json;
using Moq;
using Moq.Protected;
using Store.Services;
using Microsoft.Extensions.Logging;
using DataEntities;
using SearchEntities;

namespace Store.Tests;

public class ProductServiceTests
{
    private readonly Mock<ILogger<ProductService>> _mockLogger;
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private readonly HttpClient _httpClient;

    public ProductServiceTests()
    {
        _mockLogger = new Mock<ILogger<ProductService>>();
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri("https://test.api/")
        };
    }

    [Fact]
    public async Task GetProducts_ReturnsProducts_WhenHttpOk()
    {
        // Arrange
        var products = new List<Product>
        {
            new Product { Id = 1, Name = "Test Product 1", Description = "Desc 1", Price = 10.99m, ImageUrl = "img1.png" },
            new Product { Id = 2, Name = "Test Product 2", Description = "Desc 2", Price = 20.99m, ImageUrl = "img2.png" }
        };

        var json = JsonSerializer.Serialize(products, ProductSerializerContext.Default.ListProduct);
        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", 
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri.ToString().EndsWith("/api/product")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponse);

        var productService = new ProductService(_httpClient, _mockLogger.Object);

        // Act
        var result = await productService.GetProducts();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("Test Product 1", result[0].Name);
        Assert.Equal("Test Product 2", result[1].Name);
    }

    [Fact]
    public async Task GetProducts_ReturnsEmpty_WhenHttpNotOk()
    {
        // Arrange
        var httpResponse = new HttpResponseMessage(HttpStatusCode.InternalServerError);

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", 
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponse);

        var productService = new ProductService(_httpClient, _mockLogger.Object);

        // Act
        var result = await productService.GetProducts();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetProducts_ReturnsEmpty_OnException()
    {
        // Arrange
        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", 
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("Network error"));

        var productService = new ProductService(_httpClient, _mockLogger.Object);

        // Act
        var result = await productService.GetProducts();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task Search_UsesAiEndpoint_WhenSemanticSearchTrue()
    {
        // Arrange
        var searchResponse = new SearchResponse 
        { 
            Response = "AI Search response",
            Products = new List<Product>()
        };

        var json = JsonSerializer.Serialize(searchResponse);
        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", 
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri.ToString().Contains("/api/aisearch/")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponse);

        var productService = new ProductService(_httpClient, _mockLogger.Object);

        // Act
        var result = await productService.Search("test search", semanticSearch: true);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("AI Search response", result.Response);
        
        // Verify the correct endpoint was called
        _mockHttpMessageHandler.Protected().Verify("SendAsync", Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req => req.RequestUri.ToString().Contains("/api/aisearch/") && req.RequestUri.ToString().Contains("test")),
            ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task Search_UsesStandardEndpoint_WhenSemanticSearchFalse()
    {
        // Arrange
        var searchResponse = new SearchResponse 
        { 
            Response = "Standard search response",
            Products = new List<Product>()
        };

        var json = JsonSerializer.Serialize(searchResponse);
        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", 
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri.ToString().Contains("/api/product/search/")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponse);

        var productService = new ProductService(_httpClient, _mockLogger.Object);

        // Act
        var result = await productService.Search("test search", semanticSearch: false);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Standard search response", result.Response);
        
        // Verify the correct endpoint was called
        _mockHttpMessageHandler.Protected().Verify("SendAsync", Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req => req.RequestUri.ToString().Contains("/api/product/search/") && req.RequestUri.ToString().Contains("test")),
            ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task Search_ReturnsDefaultResponse_OnException()
    {
        // Arrange
        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", 
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("Network error"));

        var productService = new ProductService(_httpClient, _mockLogger.Object);

        // Act
        var result = await productService.Search("test search");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("No response", result.Response);
    }
}