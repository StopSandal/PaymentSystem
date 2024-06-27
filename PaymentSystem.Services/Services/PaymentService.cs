using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PaymentSystem.DataLayer.Entities;
using PaymentSystem.DataLayer.EntitiesDTO.Payment;
using PaymentSystem.DataLayer.EntitiesDTO.Transaction;
using PaymentSystem.Services.Interfaces;

namespace PaymentSystem.Services.Services
{
    /// <summary>
    /// Provides payment processing services.
    /// </summary>
    public class PaymentService : IPaymentService
    {
        private readonly ILogger<PaymentService> _logger;
        private readonly ICardService _cardService;
        private readonly ITransactionService _transactionService;

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentService"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="cardService">The card service instance.</param>
        /// <param name="transactionService">The transaction service instance.</param>
        public PaymentService(ILogger<PaymentService> logger, ICardService cardService, ITransactionService transactionService)
        {
            _logger = logger;
            _cardService = cardService;
            _transactionService = transactionService;
        }

        /// <inheritdoc />
        /// <exception cref="InvalidOperationException">
        /// Thrown when the card is not found, there is a currency mismatch, or there are insufficient funds.
        /// </exception>
        public async Task<ConfirmPaymentDTO> ProcessPaymentAsync(long cardId, decimal amount, string currency)
        {
            var card = await _cardService.GetCardAsync(cardId);
            if (card == null)
            {
                _logger.LogWarning("Card not found. Card ID: {CardId}", cardId);
                throw new InvalidOperationException("Card not found.");
            }

            if (card.CurrencyType != currency)
            {
                _logger.LogWarning("Currency mismatch. Card Currency: {CardCurrency}, Transaction Currency: {Currency}", card.CurrencyType, currency);
                throw new InvalidOperationException("Currency mismatch.");
            }

            if (card.Balance < amount)
            {
                _logger.LogWarning("Insufficient funds. Card ID: {CardId}, Available Balance: {Balance}, Attempted Amount: {Amount}", cardId, card.Balance, amount);
                throw new InvalidOperationException("Insufficient funds.");
            }

            var transaction = await _transactionService.CreateAsync(new AddTransactionDTO
            {
                CardId = cardId,
                Amount = amount,
                CurrencyType = currency
            });

            _logger.LogInformation("Payment initiated. Card ID: {CardId}, Transaction ID: {TransactionId}, Confirmation Code: {ConfirmationCode}", cardId, transaction.Id, transaction.ConfirmationCode);

            return new ConfirmPaymentDTO { TransactionId = transaction.Id, ConfirmationCode = transaction.ConfirmationCode };
        }

        /// <inheritdoc />
        /// <exception cref="InvalidOperationException">Thrown when the transaction is not confirmed.</exception>
        public async Task ConfirmPaymentAsync(long transactionId, string confirmationCode)
        {
            try
            {
                var transaction = await _transactionService.ConfirmTransactionAsync(transactionId, confirmationCode);
                await _cardService.DecreaseBalanceAsync(transaction.CardId, transaction.Amount);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Transaction not confirmed. Transaction ID: {TransactionId}", transactionId);
                throw;
            }
        }

        /// <inheritdoc />
        /// <exception cref="InvalidOperationException">Thrown when the transaction is not canceled.</exception>
        public async Task CancelPaymentAsync(long transactionId)
        {
            try
            {
                await _transactionService.CancelTransactionAsync(transactionId);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Transaction not canceled. Transaction ID: {TransactionId}", transactionId);
                throw;
            }

            _logger.LogInformation("Transaction cancelled. Transaction ID: {TransactionId}", transactionId);
        }
        /// <inheritdoc />
        /// <exception cref="InvalidOperationException">Thrown when the transaction is not confirmed or already returned.</exception>
        public async Task ReturnPaymentAsync(long transactionId)
        {
            try
            {
                var transaction = await _transactionService.ReturnTransactionAsync(transactionId);
                await _cardService.IncreaseBalanceAsync(transaction.CardId, transaction.Amount);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Transaction can't be returned. Transaction ID: {TransactionId}", transactionId);
                throw;
            }
        }
    }
}
