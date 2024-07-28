using Microsoft.Extensions.Logging;
using Moq;
using PaymentSystem.DataLayer.Entities;
using PaymentSystem.DataLayer.EntitiesDTO.Transaction;
using PaymentSystem.Services.Interfaces;
using PaymentSystem.Services.Services;
using PaymentSystem.UnitTest.Helpers;

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
        public async Task ProcessPaymentAsync_ShouldProcessPayment_WhenCardAndTransactionValid_CheckEqualId()
        {
            // Arrange
            var cardId = UnitTestConstants.ExistingCardId;
            var amount = UnitTestConstants.OneHundredAmount;
            var cardAmount = UnitTestConstants.ThreeHundredAmount;
            var currency = UnitTestConstants.RightCurrencyType;
            var transactionId = UnitTestConstants.ExistingTransactionId;
            var confirmationCode = UnitTestConstants.RightConfirmationCode;

            var card = new Card { ID = cardId, CurrencyType = currency, Balance = cardAmount };
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
        public async Task ProcessPaymentAsync_ShouldProcessPayment_WhenCardAndTransactionValid_CheckConfirmationCode()
        {
            // Arrange
            var cardId = UnitTestConstants.ExistingCardId;
            var amount = UnitTestConstants.OneHundredAmount;
            var cardAmount = UnitTestConstants.ThreeHundredAmount;
            var currency = UnitTestConstants.RightCurrencyType;
            var transactionId = UnitTestConstants.ExistingTransactionId;
            var confirmationCode = UnitTestConstants.RightConfirmationCode;

            var card = new Card { ID = cardId, CurrencyType = currency, Balance = cardAmount };
            var addTransactionDTO = new AddTransactionDTO { CardId = cardId, TotalAmount = amount, CurrencyType = currency };
            var transaction = new Transaction { Id = transactionId, ConfirmationCode = confirmationCode };

            _mockCardService.Setup(cs => cs.GetCardAsync(cardId))
                            .ReturnsAsync(card);

            _mockTransactionService.Setup(ts => ts.CreateAsync(It.IsAny<AddTransactionDTO>()))
                                  .ReturnsAsync(transaction);

            // Act
            var result = await _paymentService.ProcessPaymentAsync(cardId, amount, currency);

            // Assert
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
            var cardId = UnitTestConstants.ExistingCardId;
            var amount = UnitTestConstants.OneHundredAmount;
            var currency = UnitTestConstants.RightCurrencyType;

            _mockCardService.Setup(cs => cs.GetCardAsync(cardId))
                            .ReturnsAsync((Card)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () =>  await
                _paymentService.ProcessPaymentAsync(cardId, amount, currency));

            Assert.Equal(UnitTestConstants.CardNotFoundExceptionMessage, exception.Message);
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
            var cardId = UnitTestConstants.ExistingCardId;
            var amount = UnitTestConstants.OneHundredAmount;
            var cardCurrency = UnitTestConstants.OtherCurrencyType;
            var transactionCurrency = UnitTestConstants.RightCurrencyType;
            var card = new Card { ID = cardId, CurrencyType = cardCurrency };

            _mockCardService.Setup(cs => cs.GetCardAsync(cardId))
                            .ReturnsAsync(card);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () =>  await
                _paymentService.ProcessPaymentAsync(cardId, amount, transactionCurrency));

            Assert.Equal(UnitTestConstants.CurrencyMismatchExceptionMessage, exception.Message);
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
            var cardId = UnitTestConstants.ExistingCardId;
            var amount = UnitTestConstants.ThreeHundredAmount;
            var cardAmount = UnitTestConstants.OneHundredAmount;
            var currency = UnitTestConstants.RightCurrencyType;
            var card = new Card { ID = cardId, CurrencyType = currency, Balance = cardAmount };

            _mockCardService.Setup(cs => cs.GetCardAsync(cardId))
                            .ReturnsAsync(card);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () =>  await
                _paymentService.ProcessPaymentAsync(cardId, amount, currency));

            Assert.Equal(UnitTestConstants.InsufficientFundsExceptionMessage, exception.Message);
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
            var transactionId = UnitTestConstants.ExistingTransactionId;
            var confirmationCode = UnitTestConstants.RightConfirmationCode;
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
            var transactionId = UnitTestConstants.ExistingTransactionId;
            var confirmationCode = UnitTestConstants.RightConfirmationCode;

            _mockTransactionService.Setup(ts => ts.ConfirmTransactionAsync(transactionId, confirmationCode))
                                   .ThrowsAsync(new InvalidOperationException(UnitTestConstants.TransactionNotConfirmedExceptionMessage));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () =>  await
                _paymentService.ConfirmPaymentAsync(transactionId, confirmationCode));

            Assert.Equal(UnitTestConstants.TransactionNotConfirmedExceptionMessage, exception.Message);
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
            var transactionId = UnitTestConstants.ExistingTransactionId;

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
            var transactionId = UnitTestConstants.ExistingTransactionId;

            _mockTransactionService.Setup(ts => ts.CancelTransactionAsync(transactionId))
                                   .ThrowsAsync(new InvalidOperationException(UnitTestConstants.TransactionNotCanceledExceptionMessage));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () =>  await
                _paymentService.CancelPaymentAsync(transactionId));

            Assert.Equal(UnitTestConstants.TransactionNotCanceledExceptionMessage, exception.Message);

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
