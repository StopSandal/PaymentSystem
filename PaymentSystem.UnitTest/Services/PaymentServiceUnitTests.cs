using Microsoft.Extensions.Logging;
using Moq;
using PaymentSystem.DataLayer.Entities;
using PaymentSystem.DataLayer.EntitiesDTO.Transaction;
using PaymentSystem.Services.Interfaces;
using PaymentSystem.Services.Services;

namespace PaymentSystem.UnitTest.Services
{
    public class PaymentServiceUnitTests
    {
        private readonly Mock<ILogger<PaymentService>> _mockLogger;
        private readonly Mock<ICardService> _mockCardService;
        private readonly Mock<ITransactionService> _mockTransactionService;

        private readonly PaymentService _paymentService;

        public PaymentServiceUnitTests()
        {
            _mockLogger = new Mock<ILogger<PaymentService>>();
            _mockCardService = new Mock<ICardService>();
            _mockTransactionService = new Mock<ITransactionService>();

            _paymentService = new PaymentService(
                _mockLogger.Object,
                _mockCardService.Object,
                _mockTransactionService.Object);
        }

        [Fact]
        public async Task ProcessPaymentAsync_ShouldProcessPayment_WhenCardAndTransactionValid()
        {
            // Arrange
            long cardId = 1;
            decimal amount = 100;
            string currency = "USD";
            var transactionId = 1;
            var confirmationCode = "123456";
            var card = new Card { ID = cardId, CurrencyType = currency, Balance = amount * 2 };
            var addTransactionDTO = new AddTransactionDTO { CardId = cardId, TotalAmount = amount, CurrencyType = currency };
            var transaction = new Transaction { Id = transactionId, ConfirmationCode = confirmationCode };

            _mockCardService.Setup(cs => cs.GetCardAsync(cardId))
                            .ReturnsAsync(card);

            _mockTransactionService.Setup(ts => ts.CreateAsync(It.IsAny<AddTransactionDTO>()))
                                  .ReturnsAsync(transaction);

            // Act
            var result = await _paymentService.ProcessPaymentAsync(cardId, amount, currency);

            // Assert
            Assert.Equal(transactionId, result.TransactionId);
            Assert.Equal(confirmationCode, result.ConfirmationCode);
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                Times.Once);
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                Times.Never);
        }



        [Fact]
        public async Task ProcessPaymentAsync_ShouldThrowException_WhenCardNotFound()
        {
            // Arrange
            long cardId = 1;
            decimal amount = 100;
            string currency = "USD";

            _mockCardService.Setup(cs => cs.GetCardAsync(cardId))
                            .ReturnsAsync((Card)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _paymentService.ProcessPaymentAsync(cardId, amount, currency));

            Assert.Equal("Card not found.", exception.Message);
            _mockLogger.Verify(
                    x => x.Log(
                        LogLevel.Information,
                        It.IsAny<EventId>(),
                        It.IsAny<It.IsAnyType>(),
                        It.IsAny<Exception>(),
                        (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                    Times.Never);
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                Times.Once);
        }

        [Fact]
        public async Task ProcessPaymentAsync_ShouldThrowException_WhenCurrencyMismatch()
        {
            // Arrange
            long cardId = 1;
            decimal amount = 100;
            string cardCurrency = "EUR";
            string transactionCurrency = "USD";
            var card = new Card { ID = cardId, CurrencyType = cardCurrency };

            _mockCardService.Setup(cs => cs.GetCardAsync(cardId))
                            .ReturnsAsync(card);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _paymentService.ProcessPaymentAsync(cardId, amount, transactionCurrency));

            Assert.Equal("Currency mismatch.", exception.Message);
            _mockLogger.Verify(
                    x => x.Log(
                        LogLevel.Information,
                        It.IsAny<EventId>(),
                        It.IsAny<It.IsAnyType>(),
                        It.IsAny<Exception>(),
                        (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                    Times.Never);
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                Times.Once);
        }

        [Fact]
        public async Task ProcessPaymentAsync_ShouldThrowException_WhenInsufficientFunds()
        {
            // Arrange
            long cardId = 1;
            decimal amount = 100;
            string currency = "USD";
            var card = new Card { ID = cardId, CurrencyType = currency, Balance = amount - 1 };

            _mockCardService.Setup(cs => cs.GetCardAsync(cardId))
                            .ReturnsAsync(card);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _paymentService.ProcessPaymentAsync(cardId, amount, currency));

            Assert.Equal("Insufficient funds.", exception.Message);
            _mockLogger.Verify(
                    x => x.Log(
                        LogLevel.Information,
                        It.IsAny<EventId>(),
                        It.IsAny<It.IsAnyType>(),
                        It.IsAny<Exception>(),
                        (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                    Times.Never);
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                Times.Once);
        }

        [Fact]
        public async Task ConfirmPaymentAsync_ShouldConfirmTransaction_WhenConfirmationCodeIsValid()
        {
            // Arrange
            long transactionId = 1;
            string confirmationCode = "123456";
            var transaction = new Transaction { Id = transactionId };

            _mockTransactionService.Setup(ts => ts.ConfirmTransactionAsync(transactionId, confirmationCode))
                                   .ReturnsAsync(transaction);

            // Act
            await _paymentService.ConfirmPaymentAsync(transactionId, confirmationCode);

            // Assert
            _mockCardService.Verify(cs => cs.DecreaseBalanceAsync(transaction.CardId, transaction.TotalAmount), Times.Once);
        }

        [Fact]
        public async Task ConfirmPaymentAsync_ShouldThrowException_WhenTransactionNotConfirmed()
        {
            // Arrange
            long transactionId = 1;
            string confirmationCode = "123456";

            _mockTransactionService.Setup(ts => ts.ConfirmTransactionAsync(transactionId, confirmationCode))
                                   .ThrowsAsync(new InvalidOperationException("Transaction not confirmed."));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _paymentService.ConfirmPaymentAsync(transactionId, confirmationCode));

            Assert.Equal("Transaction not confirmed.", exception.Message);
            _mockLogger.Verify(
                    x => x.Log(
                        LogLevel.Information,
                        It.IsAny<EventId>(),
                        It.IsAny<It.IsAnyType>(),
                        It.IsAny<Exception>(),
                        (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                    Times.Never);
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                Times.Once);
        }

        [Fact]
        public async Task CancelPaymentAsync_ShouldCancelTransaction_WhenTransactionExists()
        {
            // Arrange
            long transactionId = 1;

            // Act
            await _paymentService.CancelPaymentAsync(transactionId);

            // Assert
            _mockTransactionService.Verify(ts => ts.CancelTransactionAsync(transactionId), Times.Once);
            _mockLogger.Verify(
                    x => x.Log(
                        LogLevel.Information,
                        It.IsAny<EventId>(),
                        It.IsAny<It.IsAnyType>(),
                        It.IsAny<Exception>(),
                        (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                    Times.Once);
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                Times.Never);
        }

        [Fact]
        public async Task CancelPaymentAsync_ShouldThrowException_WhenTransactionNotCanceled()
        {
            // Arrange
            long transactionId = 1;

            _mockTransactionService.Setup(ts => ts.CancelTransactionAsync(transactionId))
                                   .ThrowsAsync(new InvalidOperationException("Transaction not canceled."));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _paymentService.CancelPaymentAsync(transactionId));

            Assert.Equal("Transaction not canceled.", exception.Message);
            _mockLogger.Verify(
                    x => x.Log(
                        LogLevel.Information,
                        It.IsAny<EventId>(),
                        It.IsAny<It.IsAnyType>(),
                        It.IsAny<Exception>(),
                        (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                    Times.Never);
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                Times.Once);
        }

        [Fact]
        public async Task ReturnPaymentAsync_ShouldCancelTransaction_WhenTransactionExists()
        {
            // Arrange
            decimal amount = 100;
            decimal unreturnableFee = 30;
            var transactionId = 1;

            var transaction = new Transaction { Id = transactionId, TotalAmount = amount + unreturnableFee, UnreturnableFee = unreturnableFee };

            _mockTransactionService.Setup(ts => ts.ReturnTransactionAsync(transactionId))
                                  .ReturnsAsync(transaction);

            // Act
            await _paymentService.ReturnPaymentAsync(transactionId);

            // Assert
            _mockTransactionService.Verify(ts => ts.ReturnTransactionAsync(transactionId), Times.Once);
            _mockLogger.Verify(
                    x => x.Log(
                        LogLevel.Information,
                        It.IsAny<EventId>(),
                        It.IsAny<It.IsAnyType>(),
                        It.IsAny<Exception>(),
                        (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                    Times.Once);
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                Times.Never);
        }

        [Fact]
        public async Task CancelPaymentAsync_ShouldThrowException_WhenTransactionNotBeReturned()
        {
            // Arrange
            long transactionId = 1;

            _mockTransactionService.Setup(ts => ts.ReturnTransactionAsync(transactionId))
                                   .ThrowsAsync(new InvalidOperationException("Transaction not returned."));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _paymentService.ReturnPaymentAsync(transactionId));

            Assert.Equal("Transaction not returned.", exception.Message);
            _mockLogger.Verify(
                    x => x.Log(
                        LogLevel.Information,
                        It.IsAny<EventId>(),
                        It.IsAny<It.IsAnyType>(),
                        It.IsAny<Exception>(),
                        (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                    Times.Never);
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                Times.Once);
        }
    }
}
