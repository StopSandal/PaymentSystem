using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PaymentSystem.DataLayer.Entities;
using PaymentSystem.DataLayer.EntitiesDTO.Payment;
using PaymentSystem.Services.Interfaces;


namespace PaymentSystem.Services.Services
{
    public class PaymentService : IPaymentService
    {
        const string TRANSACTION_STATUS_PENDING = "Pending";
        const string TRANSACTION_STATUS_CANCELED= "Canceled";
        const string TRANSACTION_STATUS_CONFIRMED = "Confirmed";

        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PaymentService> _logger;
        private readonly ICardService _cardService;
        private readonly IConfirmationGenerator _confirmationGenerator;

        public PaymentService(IUnitOfWork unitOfWork, ILogger<PaymentService> logger, ICardService cardService, IConfirmationGenerator confirmationGenerator)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _cardService = cardService;
            _confirmationGenerator = confirmationGenerator;
        }
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

            var confirmationCode = await _confirmationGenerator.GenerateConfirmationCodeAsync();
            var transaction = new Transaction
            {
                CardId = cardId,
                Amount = amount,
                CurrencyType = currency,
                TransactionDate = DateTime.UtcNow,
                Status = TRANSACTION_STATUS_PENDING,
                ConfirmationCode = confirmationCode,
                ConfirmationCodeExpiresAt = DateTime.UtcNow.AddMinutes(5),
                Card = card
            };

            await _unitOfWork.TransactionRepository.InsertAsync(transaction);
            await _unitOfWork.SaveAsync(); ;

            _logger.LogInformation("Payment initiated. Card ID: {CardId}, Transaction ID: {TransactionId}, Confirmation Code: {ConfirmationCode}", cardId, transaction.Id, confirmationCode);

            return new ConfirmPaymentDTO { TransactionId = transaction.Id, ConfirmationCode = confirmationCode };

        }

        public async Task<bool> ConfirmPaymentAsync(long transactionId, string confirmationCode)
        {

            var transaction = await _unitOfWork.TransactionRepository.GetByIDAsync(transactionId);
            if (transaction == null)
            {
                _logger.LogWarning("Transaction not found. Transaction ID: {TransactionId}", transactionId);
                throw new InvalidOperationException("Transaction not found.");
            }

            if (transaction.Status != TRANSACTION_STATUS_PENDING)
            {
                _logger.LogWarning("Transaction not pending. Transaction ID: {TransactionId}", transactionId);
                throw new InvalidOperationException("Transaction already canceled or confirmed.");
            }

            if (transaction.ConfirmationCodeExpiresAt < DateTime.UtcNow)
            {
                _logger.LogWarning("Confirmation code expired. Transaction ID: {TransactionId}", transactionId);
                throw new InvalidOperationException("Confirmation code expired.");
            }

            if (transaction.ConfirmationCode != confirmationCode)
            {
                _logger.LogWarning("Invalid confirmation code. Transaction ID: {TransactionId}", transactionId);
                throw new Exception("Invalid confirmation code.");
            }

            transaction.Status = TRANSACTION_STATUS_CONFIRMED;

            await _unitOfWork.SaveAsync();

            await _cardService.DecreaseBalanceAsync(transaction.CardId, transaction.Amount);

            _logger.LogInformation("Transaction completed successfully. Transaction ID: {TransactionId}", transactionId);
            return true;
        }


        public async Task CancelPaymentAsync(long transactionId)
        {
            var transaction = await _unitOfWork.TransactionRepository.GetByIDAsync(transactionId);
            if (transaction == null)
            {
                _logger.LogWarning("Transaction not found. Transaction ID: {TransactionId}", transactionId);
                throw new InvalidOperationException("Transaction not found.");
            }
            if (transaction.Status != TRANSACTION_STATUS_PENDING)
            {
                _logger.LogWarning("Transaction not pending. Transaction ID: {TransactionId}", transactionId);
                throw new InvalidOperationException("Transaction already canceled or confirmed.");
            }

            transaction.Status = TRANSACTION_STATUS_CANCELED;
            await _unitOfWork.SaveAsync();

            _logger.LogInformation("Transaction cancelled. Transaction ID: {TransactionId}", transactionId);

        }
    }
}
