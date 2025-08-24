using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using PaymentsService.Controllers;
using PaymentsService.DTOs;
using PaymentsService.Models;
using PaymentsService.Services;

namespace PaymentsService.Tests;

public class PaymentsControllerTests
{
    private readonly Mock<IPaymentRepository> _mockRepository;
    private readonly Mock<ILogger<PaymentsController>> _mockLogger;
    private readonly PaymentsController _controller;

    public PaymentsControllerTests()
    {
        _mockRepository = new Mock<IPaymentRepository>();
        _mockLogger = new Mock<ILogger<PaymentsController>>();
        _controller = new PaymentsController(_mockRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task CreatePayment_ShouldReturnCreated_WhenValidRequest()
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

        var paymentRecord = new PaymentRecord
        {
            PaymentId = Guid.NewGuid(),
            UserId = request.UserId,
            Amount = request.Amount,
            Status = "Success",
            ProcessedAt = DateTime.UtcNow
        };

        _mockRepository.Setup(r => r.CreatePaymentAsync(It.IsAny<CreatePaymentRequest>()))
                      .ReturnsAsync(paymentRecord);

        // Act
        var result = await _controller.CreatePayment(request);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var response = Assert.IsType<CreatePaymentResponse>(createdResult.Value);
        Assert.Equal(paymentRecord.PaymentId, response.PaymentId);
        Assert.Equal("Success", response.Status);
    }

    [Fact]
    public async Task CreatePayment_ShouldReturnBadRequest_WhenInvalidModel()
    {
        // Arrange
        var request = new CreatePaymentRequest(); // Invalid - missing required fields
        _controller.ModelState.AddModelError("UserId", "Required");

        // Act
        var result = await _controller.CreatePayment(request);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetPayments_ShouldReturnOk_WithPaginatedResults()
    {
        // Arrange
        var paymentRecords = new[]
        {
            new PaymentRecord { PaymentId = Guid.NewGuid(), UserId = "user1", Amount = 50.00m, Status = "Success" },
            new PaymentRecord { PaymentId = Guid.NewGuid(), UserId = "user2", Amount = 75.00m, Status = "Success" }
        };

        _mockRepository.Setup(r => r.GetPaymentsAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
                      .ReturnsAsync((paymentRecords, 2));

        // Act
        var result = await _controller.GetPayments();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<GetPaymentsResponse>(okResult.Value);
        Assert.Equal(2, response.TotalCount);
        Assert.Equal(2, response.Items.Length);
    }

    [Fact]
    public async Task GetPayment_ShouldReturnOk_WhenPaymentExists()
    {
        // Arrange
        var paymentId = Guid.NewGuid();
        var paymentRecord = new PaymentRecord
        {
            PaymentId = paymentId,
            UserId = "test-user",
            Amount = 123.45m,
            Status = "Success"
        };

        _mockRepository.Setup(r => r.GetPaymentByIdAsync(paymentId))
                      .ReturnsAsync(paymentRecord);

        // Act
        var result = await _controller.GetPayment(paymentId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<PaymentResponse>(okResult.Value);
        Assert.Equal(paymentId, response.PaymentId);
        Assert.Equal("test-user", response.UserId);
    }

    [Fact]
    public async Task GetPayment_ShouldReturnNotFound_WhenPaymentDoesNotExist()
    {
        // Arrange
        var paymentId = Guid.NewGuid();
        _mockRepository.Setup(r => r.GetPaymentByIdAsync(paymentId))
                      .ReturnsAsync((PaymentRecord?)null);

        // Act
        var result = await _controller.GetPayment(paymentId);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }
}