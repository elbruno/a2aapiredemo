using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Search.Services;
using SearchEntities;
using Xunit;

namespace Search.Tests.Services;

public class NlWebNetClientTests
{
    private readonly Mock<ILogger<NlWebNetClient>> _mockLogger;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private readonly HttpClient _httpClient;

    public NlWebNetClientTests()
    {
        _mockLogger = new Mock<ILogger<NlWebNetClient>>();
        _mockConfiguration = new Mock<IConfiguration>();
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        
        // Setup configuration defaults
        _mockConfiguration.Setup(c => c["NLWeb:Endpoint"]).Returns("http://nlweb:8000");
        
        _httpClient = new HttpClient(_mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri("http://nlweb:8000")
        };
    }

    [Fact]
    public async Task QueryAsync_WithValidNLWebResponse_ReturnsSearchResults()
    {
        // Arrange
        var nlwebResponse = new
        {
            response = "Found 2 results for your query",
            results = new[]
            {
                new
                {
                    title = "Test Product 1",
                    url = "/products/test-1",
                    content = "This is a test product description",
                    score = 0.95,
                    source = "nlweb",
                    type = "product"
                },
                new
                {
                    title = "Test Product 2", 
                    url = "/products/test-2",
                    content = "Another test product description",
                    score = 0.85,
                    source = "nlweb",
                    type = "product"
                }
            }
        };

        var responseContent = JsonSerializer.Serialize(nlwebResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        });

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(responseContent, Encoding.UTF8, "application/json")
            });

        var client = new NlWebNetClient(_mockLogger.Object, _httpClient, _mockConfiguration.Object);

        // Act
        var result = await client.QueryAsync("test query", 10, 0);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("test query", result.Query);
        Assert.Equal(2, result.Count);
        Assert.Equal(2, result.Results.Count);
        
        var firstResult = result.Results[0];
        Assert.Equal("Test Product 1", firstResult.Title);
        Assert.Equal("/products/test-1", firstResult.Url);
        Assert.Equal("This is a test product description", firstResult.Snippet);
        Assert.Equal(0.95, firstResult.Score);
    }

    [Fact]
    public async Task QueryAsync_WhenNLWebUnavailable_ReturnsFallbackResults()
    {
        // Arrange
        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.ServiceUnavailable
            });

        var client = new NlWebNetClient(_mockLogger.Object, _httpClient, _mockConfiguration.Object);

        // Act
        var result = await client.QueryAsync("test query", 10, 0);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("test query", result.Query);
        Assert.True(result.Count > 0);
        Assert.Contains("Search service temporarily unavailable", result.Response);
        Assert.True(result.Results.Any(r => r.Metadata.ContainsKey("fallback") && (bool)r.Metadata["fallback"]));
    }

    [Fact]
    public async Task ReindexAsync_WithValidRequest_ReturnsOperationInfo()
    {
        // Arrange
        var nlwebResponse = new
        {
            operation_id = Guid.NewGuid().ToString(),
            status = "started",
            message = "Reindex operation initiated"
        };

        var responseContent = JsonSerializer.Serialize(nlwebResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        });

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(responseContent, Encoding.UTF8, "application/json")
            });

        var client = new NlWebNetClient(_mockLogger.Object, _httpClient, _mockConfiguration.Object);

        // Act
        var result = await client.ReindexAsync("https://store", false);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.OperationId);
        Assert.Equal("started", result.Status);
        Assert.Contains("operation", result.Message.ToLowerInvariant());
    }

    [Fact]
    public async Task ChatAsync_ReturnsBasicChatResponse()
    {
        // Arrange
        var client = new NlWebNetClient(_mockLogger.Object, _httpClient, _mockConfiguration.Object);

        // Act
        var result = await client.ChatAsync("Hello", "test-session");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("test-session", result.SessionId);
        Assert.NotNull(result.Response);
        Assert.NotNull(result.SuggestedActions);
        Assert.NotNull(result.Metadata);
    }

    [Fact]
    public async Task GetChatSessionAsync_ReturnsSessionInfo()
    {
        // Arrange
        var client = new NlWebNetClient(_mockLogger.Object, _httpClient, _mockConfiguration.Object);

        // Act
        var result = await client.GetChatSessionAsync("test-session");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("test-session", result.Id);
        Assert.NotNull(result.Messages);
    }
}