using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using PaymentSystem.DataLayer.Entities;
using PaymentSystem.DataLayer.EntitiesDTO.Transaction;
using PaymentSystem.Services.Helpers.Constants;
using PaymentSystem.Services.Interfaces;
using PaymentSystem.Services.Services;
using System.Linq.Expressions;

namespace PaymentSystem.UnitTest.Services
{
    public class TransactionServiceTests
    {
        private const string ConfirmationCodeExpirationMinutes = "Confirmation:ConfirmationCodeValidityInMinutes";
        private const int ConfirmationCodeExpirationMinutesInNumber = 5;

        private readonly Mock<IConfirmationGenerator> _mockConfirmationGenerator;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<ILogger<TransactionService>> _mockLogger;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<IMapper> _mockMapper;
        private readonly TransactionService _transactionService;

        public TransactionServiceTests()
        {
            _mockConfirmationGenerator = new Mock<IConfirmationGenerator>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockLogger = new Mock<ILogger<TransactionService>>();
            _mockConfiguration = new Mock<IConfiguration>();
            _mockMapper = new Mock<IMapper>();

            _transactionService = new TransactionService(
                _mockConfirmationGenerator.Object,
                _mockUnitOfWork.Object,
                _mockConfiguration.Object,
                _mockLogger.Object,
                _mockMapper.Object
            );
        }

        [Fact]
        public async Task GetAllTransactionsAsync_ReturnsAllTransactions()
        {
            // Arrange
            var transactions = new List<Transaction>
            {
                new Transaction { Id = 1 },
                new Transaction { Id = 2 }
            };
            _mockUnitOfWork.Setup(uow => uow.TransactionRepository.GetAsync(
                It.IsAny<Expression<Func<Transaction, bool>>>(),
                It.IsAny<Func<IQueryable<Transaction>, IOrderedQueryable<Transaction>>>(),
                It.IsAny<string>()
                )).ReturnsAsync(transactions);

            // Act
            var result = await _transactionService.GetAllTransactionsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(transactions, result);
        }

        [Fact]
        public async Task GetTransactionByIdAsync_ReturnsTransactionWithMatchingId()
        {
            // Arrange
            var transactionId = 1L;
            var transaction = new Transaction { Id = transactionId };
            _mockUnitOfWork.Setup(uow => uow.TransactionRepository.GetByIDAsync(transactionId)).ReturnsAsync(transaction);

            // Act
            var result = await _transactionService.GetTransactionByIdAsync(transactionId);

            // Assert
            Assert.Equal(transaction, result);
        }

        [Fact]
        public async Task CreateAsync_CreatesNewTransaction()
        {
            var cardId = 1L;
            var amount = 1;
            var currency = "USD";
            // Arrange
            var newTransactionDTO = new AddTransactionDTO
            {
                CardId = cardId,
                TotalAmount = amount,
                CurrencyType = currency
            };
            var confirmationCode = "123456";
            var transaction = new Transaction
            {
                Id = 1,
                CardId = cardId,
                TotalAmount = amount,
                CurrencyType = currency,
                Status = TransactionsConstants.TransactionStatusPending,
                ConfirmationCode = confirmationCode,
                TransactionDate = DateTime.UtcNow,
                ConfirmationCodeExpiresAt = DateTime.UtcNow.AddMinutes(ConfirmationCodeExpirationMinutesInNumber)
            };
            _mockConfirmationGenerator.Setup(cg => cg.GenerateConfirmationCodeAsync()).ReturnsAsync(confirmationCode);
            _mockMapper.Setup(m => m.Map<AddTransactionDTO, Transaction>(newTransactionDTO)).Returns(transaction);
            _mockConfiguration.Setup(cnf => cnf[ConfirmationCodeExpirationMinutes]).Returns(ConfirmationCodeExpirationMinutesInNumber.ToString());
            _mockUnitOfWork.Setup(uow => uow.TransactionRepository.InsertAsync(transaction))
                .Returns(Task.FromResult(transaction));
            _mockUnitOfWork.Setup(uow => uow.SaveAsync())
               .Verifiable();
            // Act
            var result = await _transactionService.CreateAsync(newTransactionDTO);

            // Assert
            Assert.NotNull(result);
            _mockUnitOfWork.Verify(uow => uow.TransactionRepository.InsertAsync(transaction), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.SaveAsync(), Times.Once);
            Assert.Equal(transaction.Id, result.Id);
            Assert.Equal(transaction.ConfirmationCode, result.ConfirmationCode);
        }

        [Fact]
        public async Task CancelTransactionAsync_CancelsPendingTransaction()
        {
            // Arrange
            var transactionId = 1L;
            var transaction = new Transaction { Id = transactionId, Status = TransactionsConstants.TransactionStatusPending };
            _mockUnitOfWork.Setup(uow => uow.TransactionRepository.GetByIDAsync(transactionId)).ReturnsAsync(transaction);

            // Act
            await _transactionService.CancelTransactionAsync(transactionId);

            // Assert
            Assert.Equal(TransactionsConstants.TransactionStatusCanceled, transaction.Status);
            _mockUnitOfWork.Verify(uow => uow.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task CancelTransactionAsync_ThrowsExceptionForNonPendingTransaction()
        {
            // Arrange
            var transactionId = 1L;
            var transaction = new Transaction { Id = transactionId, Status = TransactionsConstants.TransactionStatusConfirmed };
            _mockUnitOfWork.Setup(uow => uow.TransactionRepository.GetByIDAsync(transactionId)).ReturnsAsync(transaction);

            // Act and Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _transactionService.CancelTransactionAsync(transactionId));
        }

        [Fact]
        public async Task ConfirmTransactionAsync_ConfirmsPendingTransactionWithValidCode()
        {
            // Arrange
            var transactionId = 1L;
            var confirmationCode = "123456";
            var transaction = new Transaction
            {
                Id = transactionId,
                Status = TransactionsConstants.TransactionStatusPending,
                ConfirmationCode = confirmationCode,
                ConfirmationCodeExpiresAt = DateTime.UtcNow.AddMinutes(ConfirmationCodeExpirationMinutesInNumber)
            };
            _mockUnitOfWork.Setup(uow => uow.TransactionRepository.GetByIDAsync(transactionId)).ReturnsAsync(transaction);

            // Act
            var result = await _transactionService.ConfirmTransactionAsync(transactionId, confirmationCode);

            // Assert
            Assert.Equal(TransactionsConstants.TransactionStatusConfirmed, transaction.Status);
            _mockUnitOfWork.Verify(uow => uow.SaveAsync(), Times.Once);
            Assert.Equal(transaction, result);
        }

        [Fact]
        public async Task ConfirmTransactionAsync_ThrowsExceptionForNonPendingTransaction()
        {
            // Arrange
            var transactionId = 1L;
            var confirmationCode = "123456";
            var transaction = new Transaction
            {
                Id = transactionId,
                Status = TransactionsConstants.TransactionStatusCanceled,
                ConfirmationCode = confirmationCode,
                ConfirmationCodeExpiresAt = DateTime.UtcNow.AddMinutes(ConfirmationCodeExpirationMinutesInNumber)
            };
            _mockUnitOfWork.Setup(uow => uow.TransactionRepository.GetByIDAsync(transactionId)).ReturnsAsync(transaction);

            // Act and Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _transactionService.ConfirmTransactionAsync(transactionId, confirmationCode));
        }

        [Fact]
        public async Task ConfirmTransactionAsync_ThrowsExceptionForExpiredConfirmationCode()
        {
            // Arrange
            var transactionId = 1L;
            var confirmationCode = "123456";
            var transaction = new Transaction
            {
                Id = transactionId,
                Status = TransactionsConstants.TransactionStatusPending,
                ConfirmationCode = confirmationCode,
                ConfirmationCodeExpiresAt = DateTime.UtcNow.AddMinutes(-ConfirmationCodeExpirationMinutesInNumber)
            };
            _mockUnitOfWork.Setup(uow => uow.TransactionRepository.GetByIDAsync(transactionId)).ReturnsAsync(transaction);

            // Act and Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _transactionService.ConfirmTransactionAsync(transactionId, confirmationCode));
        }

        [Fact]
        public async Task ConfirmTransactionAsync_ThrowsExceptionForInvalidConfirmationCode()
        {
            // Arrange
            var transactionId = 1L;
            var confirmationCode = "123456";
            var transaction = new Transaction
            {
                Id = transactionId,
                Status = TransactionsConstants.TransactionStatusPending,
                ConfirmationCode = "654321",
                ConfirmationCodeExpiresAt = DateTime.UtcNow.AddMinutes(ConfirmationCodeExpirationMinutesInNumber)
            };
            _mockUnitOfWork.Setup(uow => uow.TransactionRepository.GetByIDAsync(transactionId)).ReturnsAsync(transaction);

            // Act and Assert
            await Assert.ThrowsAsync<Exception>(() =>
                _transactionService.ConfirmTransactionAsync(transactionId, confirmationCode));
        }
    }
}