using PaymentsService.DTOs;
using PaymentsService.Models;

namespace PaymentsService.Services;

public interface IPaymentRepository
{
    Task<PaymentRecord> CreatePaymentAsync(CreatePaymentRequest request);
    Task<(IEnumerable<PaymentRecord> Items, int TotalCount)> GetPaymentsAsync(int page = 1, int pageSize = 20, string? status = null);
    Task<PaymentRecord?> GetPaymentByIdAsync(Guid paymentId);
}