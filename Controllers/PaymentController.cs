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
        public async Task<IActionResult> ProcessPaymentAsync([FromBody] ProcessPaymentDTO request)
        {
            try
            {
                var processPaymentResponse = await _paymentService.ProcessPaymentAsync(request.CardId, request.TotalAmount, request.Currency, request.UnreturnableFee);
                _logger.LogInformation("Payment request processed successfully for Card ID: {CardId}", request.CardId);
                return Ok(new { ProcessPaymentResponse = processPaymentResponse });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing payment request for Card ID: {CardId}", request.CardId);
                return BadRequest(new { Status = "Error", Message = ex.Message });
            }
        }

        [HttpPost("confirm")]
        public async Task<IActionResult> ConfirmPaymentAsync([FromBody] ConfirmPaymentDTO request)
        {
            try
            {
                await _paymentService.ConfirmPaymentAsync(request.TransactionId, request.ConfirmationCode);
                return Ok(new { Status = "Success" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error confirming payment for Transaction ID: {TransactionId}", request.TransactionId);
                return BadRequest(new { Status = "Error", Message = ex.Message });
            }
        }

        [HttpPost("cancel/{transactionId}")]
        public async Task<IActionResult> CancelPaymentAsync([FromRoute] long transactionId)
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

        [HttpPost("return/{transactionId}")]
        public async Task<IActionResult> ReturnPaymentAsync([FromRoute] long transactionId)
        {
            try
            {
                await _paymentService.ReturnPaymentAsync(transactionId);
                return Ok(new { Status = "Success" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error returning payment for Transaction ID: {TransactionId}", transactionId);
                return BadRequest(new { Status = "Error", Message = ex.Message });
            }
        }
    }
}
