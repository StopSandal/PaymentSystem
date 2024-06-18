using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PaymentSystem.DataLayer.EntitiesDTO.Payment;
using PaymentSystem.Services.Interfaces;

namespace PaymentSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(IPaymentService paymentService, ILogger<PaymentController> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }

        [HttpPost("process")]
        public async Task<IActionResult> ProcessPayment([FromBody] ProcessPaymentDTO request)
        {
            try
            {
                var confirmationCode = await _paymentService.ProcessPaymentAsync(request.CardId, request.Amount, request.Currency);
                _logger.LogInformation("Payment request processed successfully for Card ID: {CardId}", request.CardId);
                return Ok(new { Status = "Success", ConfirmationCode = confirmationCode });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing payment request for Card ID: {CardId}", request.CardId);
                return BadRequest(new { Status = "Error", Message = ex.Message });
            }
        }

        [HttpPost("confirm")]
        public async Task<IActionResult> ConfirmPayment([FromBody] ConfirmPaymentDTO request)
        {
            try
            {
                var result = await _paymentService.ConfirmPaymentAsync(request.TransactionId, request.ConfirmationCode);
                if (result)
                {
                    return Ok(new { Status = "Success" });
                }
                else
                {
                    return BadRequest(new { Status = "Error", Message = "Failed to confirm payment." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error confirming payment for Transaction ID: {TransactionId}", request.TransactionId);
                return BadRequest(new { Status = "Error", Message = ex.Message });
            }
        }

        [HttpPost("cancel")]
        public async Task<IActionResult> CancelPayment(long transactionId)
        {
            try
            {
                await _paymentService.CancelPaymentAsync(transactionId);
                return Ok(new { Status = "Success" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling payment for Transaction ID: {TransactionId}", transactionId);
                return BadRequest(new { Status = "Error", Message = ex.Message });
            }
        }
    }
}
