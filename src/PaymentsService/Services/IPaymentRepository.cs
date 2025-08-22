using PaymentsService.Models;
using PaymentsService.DTOs;

namespace PaymentsService.Services;

public interface IPaymentRepository
{
    Task<PaymentRecord> CreatePaymentAsync(CreatePaymentRequest request);
    Task<PaymentRecord?> GetPaymentByIdAsync(Guid paymentId);
    Task<PaymentListResponse> GetPaymentsAsync(int page = 1, int pageSize = 10, string? status = null, string? userId = null);
    Task<bool> UpdatePaymentStatusAsync(Guid paymentId, string status);
}