using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PaymentsService.Data;
using PaymentsService.DTOs;
using PaymentsService.Services;

namespace PaymentsService.Tests;

public class PaymentRepositoryTests : IDisposable
{
    private readonly PaymentsDbContext _context;
    private readonly PaymentRepository _repository;
    private readonly ILogger<PaymentRepository> _logger;

    public PaymentRepositoryTests()
    {
        // Create in-memory database for testing
        var options = new DbContextOptionsBuilder<PaymentsDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new PaymentsDbContext(options);
        _logger = new LoggerFactory().CreateLogger<PaymentRepository>();
        _repository = new PaymentRepository(_context, _logger);
    }

    [Fact]
    public async Task CreatePaymentAsync_ShouldCreatePayment_WhenValidRequest()
    {
        // Arrange
        var request = new CreatePaymentRequest
        {
            UserId = "test-user",
            Currency = "USD",
            Amount = 99.99m,
            PaymentMethod = "Visa ****1234",
            Items = new[]
            {
                new PaymentItem { ProductId = "prod-1", Quantity = 2, UnitPrice = 49.99m }
            }
        };

        // Act
        var result = await _repository.CreatePaymentAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.PaymentId);
        Assert.Equal(request.UserId, result.UserId);
        Assert.Equal(request.Amount, result.Amount);
        Assert.Equal(request.PaymentMethod, result.PaymentMethod);
        Assert.Equal("Success", result.Status);
    }

    [Fact]
    public async Task GetPaymentsAsync_ShouldReturnPaginatedResults()
    {
        // Arrange
        var requests = new[]
        {
            new CreatePaymentRequest { UserId = "user1", Amount = 10.00m, Currency = "USD", PaymentMethod = "Visa ****1111", Items = Array.Empty<PaymentItem>() },
            new CreatePaymentRequest { UserId = "user2", Amount = 20.00m, Currency = "USD", PaymentMethod = "Visa ****2222", Items = Array.Empty<PaymentItem>() },
            new CreatePaymentRequest { UserId = "user3", Amount = 30.00m, Currency = "USD", PaymentMethod = "Visa ****3333", Items = Array.Empty<PaymentItem>() }
        };

        foreach (var request in requests)
        {
            await _repository.CreatePaymentAsync(request);
        }

        // Act
        var (payments, totalCount) = await _repository.GetPaymentsAsync(page: 1, pageSize: 2);

        // Assert
        Assert.Equal(3, totalCount);
        Assert.Equal(2, payments.Count());
    }

    [Fact]
    public async Task GetPaymentByIdAsync_ShouldReturnPayment_WhenExists()
    {
        // Arrange
        var request = new CreatePaymentRequest
        {
            UserId = "test-user",
            Currency = "USD",
            Amount = 50.00m,
            PaymentMethod = "MasterCard ****5678",
            Items = Array.Empty<PaymentItem>()
        };

        var createdPayment = await _repository.CreatePaymentAsync(request);

        // Act
        var result = await _repository.GetPaymentByIdAsync(createdPayment.PaymentId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(createdPayment.PaymentId, result.PaymentId);
        Assert.Equal(createdPayment.UserId, result.UserId);
    }

    [Fact]
    public async Task GetPaymentByIdAsync_ShouldReturnNull_WhenNotExists()
    {
        // Act
        var result = await _repository.GetPaymentByIdAsync(Guid.NewGuid());

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetPaymentsAsync_WithStatusFilter_ShouldReturnFilteredResults()
    {
        // Arrange
        var request = new CreatePaymentRequest
        {
            UserId = "test-user",
            Currency = "USD",
            Amount = 25.00m,
            PaymentMethod = "PayPal",
            Items = Array.Empty<PaymentItem>()
        };

        await _repository.CreatePaymentAsync(request);

        // Act
        var (payments, totalCount) = await _repository.GetPaymentsAsync(status: "Success");

        // Assert
        Assert.True(totalCount > 0);
        Assert.All(payments, p => Assert.Equal("Success", p.Status));
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}