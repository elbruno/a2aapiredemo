using Microsoft.EntityFrameworkCore;
using PaymentsService.Data;
using PaymentsService.Models;
using PaymentsService.DTOs;

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
        try
        {
            var payment = new PaymentRecord
            {
                PaymentId = Guid.NewGuid(),
                UserId = request.UserId,
                StoreId = request.StoreId,
                CartId = request.CartId,
                Currency = request.Currency,
                Amount = request.Amount,
                Status = "Success", // Mock success for demo
                PaymentMethod = request.PaymentMethod,
                Items = request.Items,
                CreatedAt = DateTime.UtcNow,
                ProcessedAt = DateTime.UtcNow
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Payment created successfully: {PaymentId}", payment.PaymentId);
            return payment;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating payment for user {UserId}", request.UserId);
            throw;
        }
    }

    public async Task<PaymentRecord?> GetPaymentByIdAsync(Guid paymentId)
    {
        try
        {
            return await _context.Payments
                .FirstOrDefaultAsync(p => p.PaymentId == paymentId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving payment {PaymentId}", paymentId);
            throw;
        }
    }

    public async Task<PaymentListResponse> GetPaymentsAsync(int page = 1, int pageSize = 10, string? status = null, string? userId = null)
    {
        try
        {
            var query = _context.Payments.AsQueryable();

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(p => p.Status == status);
            }

            if (!string.IsNullOrEmpty(userId))
            {
                query = query.Where(p => p.UserId == userId);
            }

            var totalCount = await query.CountAsync();
            
            var payments = await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new DTOs.PaymentRecord
                {
                    PaymentId = p.PaymentId.ToString(),
                    UserId = p.UserId,
                    StoreId = p.StoreId,
                    CartId = p.CartId,
                    Currency = p.Currency,
                    Amount = p.Amount,
                    Status = p.Status,
                    PaymentMethod = p.PaymentMethod,
                    CreatedAt = p.CreatedAt,
                    ProcessedAt = p.ProcessedAt,
                    Items = p.Items,
                    ProductEnrichment = p.ProductEnrichment
                })
                .ToListAsync();

            return new PaymentListResponse
            {
                Items = payments,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving payments");
            throw;
        }
    }

    public async Task<bool> UpdatePaymentStatusAsync(Guid paymentId, string status)
    {
        try
        {
            var payment = await _context.Payments.FindAsync(paymentId);
            if (payment == null)
            {
                return false;
            }

            payment.Status = status;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Payment status updated: {PaymentId} -> {Status}", paymentId, status);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating payment status for {PaymentId}", paymentId);
            throw;
        }
    }
}