using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PaymentSystem.DataLayer.Entities;
using PaymentSystem.DataLayer.EntitiesDTO.Transaction;
using PaymentSystem.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentSystem.Services.Services
{
    public class TransactionService : ITransactionService
    {
        const string TRANSACTION_STATUS_PENDING = "Pending";
        const string TRANSACTION_STATUS_CANCELED = "Canceled";
        const string TRANSACTION_STATUS_CONFIRMED = "Confirmed";
        const string CONFIRMATION_CODE_EXPIRATION_MINUTES = "Confirmation:ConfirmationCodeValidityInMinutes";

        private readonly IConfirmationGenerator _confirmationGenerator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<TransactionService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public TransactionService(IConfirmationGenerator confirmationGenerator, IUnitOfWork unitOfWork, IConfiguration configuration, ILogger<TransactionService> logger, IMapper mapper)
        {
            _confirmationGenerator = confirmationGenerator;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<IEnumerable<Transaction>> GetAllTransactionsAsync()
        {
            return await _unitOfWork.TransactionRepository.GetAsync();
        }

        public async Task<Transaction> GetTransactionByIdAsync(long transactionId)
        {
            return await _unitOfWork.TransactionRepository.GetByIDAsync(transactionId);
        }

        public async Task<Transaction> CreateAsync(AddTransactionDTO newTransaction)
        {
            var confirmationCode = await _confirmationGenerator.GenerateConfirmationCodeAsync();

            newTransaction.ConfirmationCode = confirmationCode;
            newTransaction.Status = TRANSACTION_STATUS_PENDING;

            var transaction = _mapper.Map<AddTransactionDTO, Transaction>(newTransaction);
            transaction.ConfirmationCodeExpiresAt = DateTime.UtcNow.AddMinutes(int.Parse(_configuration[CONFIRMATION_CODE_EXPIRATION_MINUTES]));
            await _unitOfWork.TransactionRepository.InsertAsync(transaction);
            await _unitOfWork.SaveAsync();
            return transaction;
        }

        public async Task CancelTransactionAsync(long transactionId)
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
        }

        public async Task<Transaction> ConfirmTransactionAsync(long transactionId, string confirmationCode)
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

            _logger.LogInformation("Transaction completed successfully. Transaction ID: {TransactionId}", transactionId);

            return transaction;
        }

    }
}
