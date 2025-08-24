using Microsoft.EntityFrameworkCore;
using PaymentsService.Data;
using PaymentsService.DTOs;
using PaymentsService.Models;
using System.Text.Json;

namespace PaymentsService.Services;

public class PaymentRepository : IPaymentRepository
{
    private readonly PaymentsDbContext _context;
    private readonly ILogger<PaymentRepository> _logger;

    public PaymentRepository(PaymentsDbContext context, ILogger<PaymentRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PaymentRecord> CreatePaymentAsync(CreatePaymentRequest request)
    {
        // Create payment record
        var payment = new PaymentRecord
        {
            PaymentId = Guid.NewGuid(),
            UserId = request.UserId,
            StoreId = request.StoreId,
            CartId = request.CartId,
            Currency = request.Currency,
            Amount = request.Amount,
            PaymentMethod = request.PaymentMethod,
            Status = "Success", // Mock always succeeds
            ItemsJson = JsonSerializer.Serialize(request.Items),
            CreatedAt = DateTime.UtcNow,
            ProcessedAt = DateTime.UtcNow
        };

        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Payment {PaymentId} created for user {UserId} with amount {Amount} {Currency}",
            payment.PaymentId, payment.UserId, payment.Amount, payment.Currency);

        return payment;
    }

    public async Task<(IEnumerable<PaymentRecord> Items, int TotalCount)> GetPaymentsAsync(int page = 1, int pageSize = 20, string? status = null)
    {
        var query = _context.Payments.AsQueryable();

        // Apply status filter if provided
        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(p => p.Status == status);
        }

        // Get total count for pagination
        var totalCount = await query.CountAsync();

        // Apply pagination and ordering
        var items = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<PaymentRecord?> GetPaymentByIdAsync(Guid paymentId)
    {
        return await _context.Payments
            .FirstOrDefaultAsync(p => p.PaymentId == paymentId);
    }
}