using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Store.Services;
using System.Net;
using System.Text.Json;

namespace Store.Tests;

public class PaymentsClientTests
{
    private readonly Mock<HttpMessageHandler> _mockHandler;
    private readonly HttpClient _httpClient;
    private readonly Mock<ILogger<PaymentsClient>> _mockLogger;
    private readonly PaymentsClient _paymentsClient;

    public PaymentsClientTests()
    {
        _mockHandler = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_mockHandler.Object)
        {
            BaseAddress = new Uri("https://payments-service/")
        };
        _mockLogger = new Mock<ILogger<PaymentsClient>>();
        _paymentsClient = new PaymentsClient(_httpClient, _mockLogger.Object);
    }

    [Fact]
    public async Task CreatePaymentAsync_ShouldReturnResponse_WhenSuccessful()
    {
        // Arrange
        var request = new CreatePaymentRequest
        {
            UserId = "test-user",
            Currency = "USD",
            Amount = 100.00m,
            PaymentMethod = "Visa ****1234",
            Items = new[]
            {
                new PaymentItem { ProductId = "prod-1", Quantity = 1, UnitPrice = 100.00m }
            }
        };

        var expectedResponse = new CreatePaymentResponse
        {
            PaymentId = Guid.NewGuid(),
            Status = "Success",
            ProcessedAt = DateTime.UtcNow
        };

        var responseJson = JsonSerializer.Serialize(expectedResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        _mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.Created,
                Content = new StringContent(responseJson)
            });

        // Act
        var result = await _paymentsClient.CreatePaymentAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResponse.PaymentId, result.PaymentId);
        Assert.Equal("Success", result.Status);
    }

    [Fact]
    public async Task CreatePaymentAsync_ShouldReturnNull_WhenRequestFails()
    {
        // Arrange
        var request = new CreatePaymentRequest
        {
            UserId = "test-user",
            Currency = "USD",
            Amount = 100.00m,
            PaymentMethod = "Visa ****1234",
            Items = Array.Empty<PaymentItem>()
        };

        _mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = new StringContent("Bad Request")
            });

        // Act
        var result = await _paymentsClient.CreatePaymentAsync(request);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreatePaymentAsync_ShouldReturnNull_WhenExceptionOccurs()
    {
        // Arrange
        var request = new CreatePaymentRequest
        {
            UserId = "test-user",
            Currency = "USD",
            Amount = 100.00m,
            PaymentMethod = "Visa ****1234",
            Items = Array.Empty<PaymentItem>()
        };

        _mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("Network error"));

        // Act
        var result = await _paymentsClient.CreatePaymentAsync(request);

        // Assert
        Assert.Null(result);
    }
}