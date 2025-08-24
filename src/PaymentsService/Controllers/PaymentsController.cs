using Microsoft.AspNetCore.Mvc;
using PaymentsService.DTOs;
using PaymentsService.Services;
using System.Text.Json;

namespace PaymentsService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly ILogger<PaymentsController> _logger;

    public PaymentsController(IPaymentRepository paymentRepository, ILogger<PaymentsController> logger)
    {
        _paymentRepository = paymentRepository;
        _logger = logger;
    }

    /// <summary>
    /// Create a new payment
    /// </summary>
    /// <param name="request">Payment creation request</param>
    /// <returns>Payment creation response</returns>
    [HttpPost]
    public async Task<ActionResult<CreatePaymentResponse>> CreatePayment([FromBody] CreatePaymentRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Processing payment request for user {UserId} with amount {Amount} {Currency}",
                request.UserId, request.Amount, request.Currency);

            var payment = await _paymentRepository.CreatePaymentAsync(request);

            var response = new CreatePaymentResponse
            {
                PaymentId = payment.PaymentId,
                Status = payment.Status,
                ProcessedAt = payment.ProcessedAt
            };

            return CreatedAtAction(nameof(GetPayment), new { id = payment.PaymentId }, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing payment for user {UserId}", request.UserId);
            return StatusCode(500, new { Error = "An error occurred while processing the payment" });
        }
    }

    /// <summary>
    /// Get paginated list of payments
    /// </summary>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 20, max: 100)</param>
    /// <param name="status">Filter by status (optional)</param>
    /// <returns>Paginated payment list</returns>
    [HttpGet]
    public async Task<ActionResult<GetPaymentsResponse>> GetPayments(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? status = null)
    {
        try
        {
            // Validate pagination parameters
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 20;

            var (payments, totalCount) = await _paymentRepository.GetPaymentsAsync(page, pageSize, status);

            var paymentResponses = payments.Select(p => new PaymentResponse
            {
                PaymentId = p.PaymentId,
                UserId = p.UserId,
                StoreId = p.StoreId,
                CartId = p.CartId,
                Currency = p.Currency,
                Amount = p.Amount,
                Status = p.Status,
                PaymentMethod = p.PaymentMethod,
                Items = string.IsNullOrEmpty(p.ItemsJson) ? null : JsonSerializer.Deserialize<PaymentItem[]>(p.ItemsJson),
                CreatedAt = p.CreatedAt,
                ProcessedAt = p.ProcessedAt
            }).ToArray();

            return Ok(new GetPaymentsResponse
            {
                Items = paymentResponses,
                TotalCount = totalCount
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving payments");
            return StatusCode(500, new { Error = "An error occurred while retrieving payments" });
        }
    }

    /// <summary>
    /// Get a specific payment by ID
    /// </summary>
    /// <param name="id">Payment ID</param>
    /// <returns>Payment details</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<PaymentResponse>> GetPayment(Guid id)
    {
        try
        {
            var payment = await _paymentRepository.GetPaymentByIdAsync(id);

            if (payment == null)
            {
                return NotFound(new { Error = $"Payment with ID {id} not found" });
            }

            var response = new PaymentResponse
            {
                PaymentId = payment.PaymentId,
                UserId = payment.UserId,
                StoreId = payment.StoreId,
                CartId = payment.CartId,
                Currency = payment.Currency,
                Amount = payment.Amount,
                Status = payment.Status,
                PaymentMethod = payment.PaymentMethod,
                Items = string.IsNullOrEmpty(payment.ItemsJson) ? null : JsonSerializer.Deserialize<PaymentItem[]>(payment.ItemsJson),
                CreatedAt = payment.CreatedAt,
                ProcessedAt = payment.ProcessedAt
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving payment {PaymentId}", id);
            return StatusCode(500, new { Error = "An error occurred while retrieving the payment" });
        }
    }
}