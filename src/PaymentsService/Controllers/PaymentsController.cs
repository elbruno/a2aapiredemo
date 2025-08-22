using Microsoft.AspNetCore.Mvc;
using PaymentsService.DTOs;
using PaymentsService.Services;
using System.ComponentModel.DataAnnotations;

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
    /// Process a new payment
    /// </summary>
    /// <param name="request">Payment details</param>
    /// <returns>Payment result</returns>
    [HttpPost]
    public async Task<ActionResult<PaymentResponse>> ProcessPayment([FromBody] CreatePaymentRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Processing payment for user {UserId}, amount {Amount} {Currency}", 
                request.UserId, request.Amount, request.Currency);

            var payment = await _paymentRepository.CreatePaymentAsync(request);

            var response = new PaymentResponse
            {
                PaymentId = payment.PaymentId.ToString(),
                Status = payment.Status,
                ProcessedAt = payment.ProcessedAt,
                Message = "Payment processed successfully"
            };

            _logger.LogInformation("Payment processed successfully: {PaymentId}", payment.PaymentId);
            return CreatedAtAction(nameof(GetPayment), new { id = payment.PaymentId }, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing payment for user {UserId}", request.UserId);
            return StatusCode(500, new PaymentResponse
            {
                PaymentId = string.Empty,
                Status = "Failed",
                ProcessedAt = DateTime.UtcNow,
                Message = "Payment processing failed"
            });
        }
    }

    /// <summary>
    /// Get payments with optional filtering and pagination
    /// </summary>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10, max: 100)</param>
    /// <param name="status">Filter by payment status</param>
    /// <param name="userId">Filter by user ID</param>
    /// <returns>List of payments</returns>
    [HttpGet]
    public async Task<ActionResult<PaymentListResponse>> GetPayments(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? status = null,
        [FromQuery] string? userId = null)
    {
        try
        {
            // Validate pagination parameters
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            _logger.LogInformation("Getting payments: page {Page}, pageSize {PageSize}, status {Status}, userId {UserId}", 
                page, pageSize, status, userId);

            var result = await _paymentRepository.GetPaymentsAsync(page, pageSize, status, userId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving payments");
            return StatusCode(500, "Error retrieving payments");
        }
    }

    /// <summary>
    /// Get a specific payment by ID
    /// </summary>
    /// <param name="id">Payment ID</param>
    /// <returns>Payment details</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<DTOs.PaymentRecord>> GetPayment(Guid id)
    {
        try
        {
            var payment = await _paymentRepository.GetPaymentByIdAsync(id);
            if (payment == null)
            {
                return NotFound($"Payment {id} not found");
            }

            var response = new DTOs.PaymentRecord
            {
                PaymentId = payment.PaymentId.ToString(),
                UserId = payment.UserId,
                StoreId = payment.StoreId,
                CartId = payment.CartId,
                Currency = payment.Currency,
                Amount = payment.Amount,
                Status = payment.Status,
                PaymentMethod = payment.PaymentMethod,
                CreatedAt = payment.CreatedAt,
                ProcessedAt = payment.ProcessedAt,
                Items = payment.Items,
                ProductEnrichment = payment.ProductEnrichment
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving payment {PaymentId}", id);
            return StatusCode(500, "Error retrieving payment");
        }
    }

    /// <summary>
    /// Health check endpoint
    /// </summary>
    /// <returns>Service status</returns>
    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
    }
}