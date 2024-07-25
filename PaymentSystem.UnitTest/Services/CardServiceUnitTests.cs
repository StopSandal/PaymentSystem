using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using PaymentSystem.DataLayer.Entities;
using PaymentSystem.DataLayer.EntitiesDTO.Card;
using PaymentSystem.Services.Interfaces;
using PaymentSystem.Services.Services;
using System.Linq.Expressions;

namespace PaymentSystem.UnitTest.Services
{
    public class CardServiceUnitTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<CardService>> _mockLogger;
        private readonly CardService _cardService;

        public CardServiceUnitTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<CardService>>();
            _cardService = new CardService(_mockUnitOfWork.Object, _mockMapper.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetCardAsync_ShouldReturnCard_WhenCardExists()
        {
            // Arrange
            var cardId = 1L;
            var expectedCard = new Card { ID = cardId, CardNumber = "1234567890123456" };
            _mockUnitOfWork.Setup(uow => uow.CardRepository.GetByIDAsync(cardId))
                           .ReturnsAsync(expectedCard);

            // Act
            var result = await _cardService.GetCardAsync(cardId);

            // Assert
            Assert.Equal(cardId, result.ID);
                _mockUnitOfWork.Verify(uow => uow.CardRepository.GetByIDAsync(cardId), Times.Once);
        }

        [Fact]
        public async Task GetCardAsync_ShouldReturnNull_WhenCardDoesNotExist()
        {
            // Arrange
            var cardId = 1L;
            _mockUnitOfWork.Setup(uow => uow.CardRepository.GetByIDAsync(cardId))
                           .ReturnsAsync((Card)null);

            // Act
            var result = await _cardService.GetCardAsync(cardId);

            // Assert
            Assert.Null(result);
            _mockUnitOfWork.Verify(uow => uow.CardRepository.GetByIDAsync(cardId), Times.Once);
        }
        [Fact]
        public async Task GetCardsAsync_ShouldAllCards()
        {
            // Arrange
            var expectedCards = new List<Card>
            {
                new Card { ID = 1, CardNumber = "1234567890123456" },
                new Card { ID = 2, CardNumber = "6543210987654321" }
            };
            _mockUnitOfWork.Setup(
                uow => uow.CardRepository.GetAsync(
                    It.IsAny<System.Linq.Expressions.Expression<Func<Card, bool>>>(),
                    It.IsAny<Func<IQueryable<Card>, IOrderedQueryable<Card>>>(),
                    It.IsAny<string>()
                    )
                )
                .ReturnsAsync(expectedCards);

            // Act
            var result = await _cardService.GetCardsAsync();

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Equal("1234567890123456", result.First().CardNumber);
            _mockUnitOfWork.Verify(uow => uow.CardRepository.GetAsync(
                    It.IsAny<Expression<Func<Card, bool>>>(),
                    It.IsAny<Func<IQueryable<Card>, IOrderedQueryable<Card>>>(),
                    It.IsAny<string>())
                , Times.Once);
        }
        [Fact]
        public async Task GetCardsAsync_ShouldReturnNull()
        {
            // Arrange
            _mockUnitOfWork.Setup(
                uow => uow.CardRepository.GetAsync(
                    It.IsAny<Expression<Func<Card, bool>>>(),
                    It.IsAny<Func<IQueryable<Card>, IOrderedQueryable<Card>>>(),
                    It.IsAny<string>()
                    )
                )
                .ReturnsAsync((IEnumerable<Card>?)null);

            // Act
            var result = await _cardService.GetCardsAsync();

            // Assert
            Assert.Null(result);
            _mockUnitOfWork.Verify(uow => uow.CardRepository.GetAsync(
                    It.IsAny<System.Linq.Expressions.Expression<Func<Card, bool>>>(),
                    It.IsAny<Func<IQueryable<Card>, IOrderedQueryable<Card>>>(),
                    It.IsAny<string>())
                , Times.Once);
        }
        [Fact]
        public async Task CreateCardAsync_Success()
        {
            //Arrange
            var cardDto = new AddCardDTO
            {
                CardNumber = "1234567890",
                CurrencyType = "USD",
                CVV = 111
            };
            var card = new Card
            {
                CardNumber = "1234567890",
                CurrencyType = "USD",
                CVV = 111
            };

            _mockMapper.Setup(mapper => mapper.Map<Card>(cardDto))
               .Returns(card);
            _mockUnitOfWork.Setup(uow => uow.CardRepository.InsertAsync(card)).Verifiable();
            _mockUnitOfWork.Setup(uow => uow.SaveAsync()).Verifiable();

            //Act 
            await _cardService.CreateCardAsync(cardDto);

            //Assert
            _mockMapper.Verify(mapper => mapper.Map<Card>(cardDto), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.CardRepository.InsertAsync(card), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.SaveAsync(), Times.Once);
        }
        [Fact]
        public async Task GetCardByCardNumberAsync_ShouldReturnCard_WhenCardExists()
        {
            // Arrange
            var cardNumber = "1234567890";
            var wrongCardNumber = "BadNumber";
            var expectedCard = new Card { ID = 2L, CardNumber = cardNumber };
            var unexpectedCard = new Card { ID = 1L, CardNumber = wrongCardNumber };
            var cardList = new List<Card> { unexpectedCard, expectedCard };

            _mockUnitOfWork.Setup(uow => uow.CardRepository.GetAsync(
                  It.IsAny<Expression<Func<Card, bool>>>(),
                  It.IsAny<Func<IQueryable<Card>, IOrderedQueryable<Card>>>(),
                  It.IsAny<string>()))
                  .ReturnsAsync((Expression<Func<Card, bool>> filter, Func<IQueryable<Card>, IOrderedQueryable<Card>> orderBy, string includeProperties) =>
                  {
                      var filteredCards = cardList.Where(filter.Compile());
                      return filteredCards.ToList();
                  });

            // Act
            var result = await _cardService.GetCardByCardNumberAsync(cardNumber);

            // Assert
            Assert.Equal(cardNumber, result.CardNumber);
        }
        [Fact]
        public async Task GetCardByCardNumberAsync_ShouldReturnNull_WhenCardDoesNotExists()
        {
            //Arrange
            var cardNumber = "1234567890";

            _mockUnitOfWork.Setup(uow => uow.CardRepository.GetAsync(
                  It.IsAny<Expression<Func<Card, bool>>>(),
                  It.IsAny<Func<IQueryable<Card>, IOrderedQueryable<Card>>>(),
                  It.IsAny<string>()))
                  .ReturnsAsync(new List<Card>());

            //Act
            var result = await _cardService.GetCardByCardNumberAsync(cardNumber);

            //Assert
            Assert.Null(result);
            _mockUnitOfWork.Verify(uow => uow.CardRepository.GetAsync(
                It.IsAny<Expression<Func<Card, bool>>>(),
                  It.IsAny<Func<IQueryable<Card>, IOrderedQueryable<Card>>>(),
                  It.IsAny<string>()));
        }
        [Fact]
        public async Task UpdateCardAsync_ShouldUpdateCardSuccessfully()
        {
            // Arrange
            var cardId = 1L;
            var editCardDTO = new EditCardDTO { CardName = "Card 1" };
            var existingCard = new Card { ID = cardId, CardNumber = "Card 2" };

            _mockUnitOfWork.Setup(uow => uow.CardRepository.GetByIDAsync(cardId))
                           .ReturnsAsync(existingCard);

            // Act
            await _cardService.UpdateCardAsync(cardId, editCardDTO);

            // Assert
            _mockUnitOfWork.Verify(uow => uow.CardRepository.GetByIDAsync(cardId), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.CardRepository.Update(It.IsAny<Card>()), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateCardAsync_ShouldThrowExceptionIfCardDoesNotExist()
        {
            // Arrange
            var cardId = 1L;
            var editCardDTO = new EditCardDTO { CardName = "Card 1" };

            _mockUnitOfWork.Setup(uow => uow.CardRepository.GetByIDAsync(cardId))
                           .ReturnsAsync((Card?)null);

            // Act
            await Assert.ThrowsAsync<InvalidDataException>(() => _cardService.UpdateCardAsync(cardId, editCardDTO));

            //Assert
            _mockUnitOfWork.Verify(uow => uow.CardRepository.GetByIDAsync(cardId), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.SaveAsync(), Times.Never);
        }

        [Fact]
        public async Task DeleteCardAsync_ShouldDeleteCardSuccessfully()
        {
            // Arrange
            var cardId = 1L;

            _mockUnitOfWork.Setup(uow => uow.CardRepository.DeleteAsync(cardId))
                           .Returns(Task.CompletedTask);

            // Act
            await _cardService.DeleteCardAsync(cardId);

            // Assert
            _mockUnitOfWork.Verify(uow => uow.CardRepository.DeleteAsync(cardId), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task IncreaseBalanceAsync_ShouldIncreaseBalanceSuccessfully()
        {
            // Arrange
            var cardId = 1L;
            var amount = 100M;
            var existingCard = new Card { ID = cardId, Balance = 200M };

            _mockUnitOfWork.Setup(uow => uow.CardRepository.GetByIDAsync(cardId))
                           .ReturnsAsync(existingCard);

            // Act
            await _cardService.IncreaseBalanceAsync(cardId, amount);

            // Assert
            Assert.Equal(300M, existingCard.Balance);
            _mockUnitOfWork.Verify(uow => uow.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task IncreaseBalanceAsync_ShouldThrowExceptionIfCardDoesNotExist()
        {
            // Arrange
            var cardId = 1L;
            var amount = 100M;

            _mockUnitOfWork.Setup(uow => uow.CardRepository.GetByIDAsync(cardId))
                           .ReturnsAsync((Card?)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidDataException>(() => _cardService.IncreaseBalanceAsync(cardId, amount));
            _mockUnitOfWork.Verify(uow => uow.SaveAsync(), Times.Never);
        }

        [Fact]
        public async Task IncreaseBalanceAsync_ShouldThrowExceptionIfAmountIsInvalid()
        {
            // Arrange
            var cardId = 1L;
            var amount = -100M;
            var existingCard = new Card { ID = cardId, Balance = 200M };

            _mockUnitOfWork.Setup(uow => uow.CardRepository.GetByIDAsync(cardId))
                           .ReturnsAsync(existingCard);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _cardService.IncreaseBalanceAsync(cardId, amount));
            _mockUnitOfWork.Verify(uow => uow.SaveAsync(), Times.Never);
        }

        [Fact]
        public async Task DecreaseBalanceAsync_ShouldDecreaseBalanceSuccessfully()
        {
            // Arrange
            var cardId = 1L;
            var amount = 100M;
            var existingCard = new Card { ID = cardId, Balance = 200M };

            _mockUnitOfWork.Setup(uow => uow.CardRepository.GetByIDAsync(cardId))
                           .ReturnsAsync(existingCard);

            // Act
            await _cardService.DecreaseBalanceAsync(cardId, amount);

            // Assert
            Assert.Equal(100M, existingCard.Balance);
            _mockUnitOfWork.Verify(uow => uow.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task DecreaseBalanceAsync_ShouldThrowExceptionIfCardDoesNotExist()
        {
            // Arrange
            var cardId = 1L;
            var amount = 100M;

            _mockUnitOfWork.Setup(uow => uow.CardRepository.GetByIDAsync(cardId))
                           .ReturnsAsync((Card?)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidDataException>(() => _cardService.DecreaseBalanceAsync(cardId, amount));
            _mockUnitOfWork.Verify(uow => uow.SaveAsync(), Times.Never);
        }

        [Fact]
        public async Task DecreaseBalanceAsync_ShouldThrowExceptionIfAmountIsInvalid()
        {
            // Arrange
            var cardId = 1L;
            var amount = -100M;
            var existingCard = new Card { ID = cardId, Balance = 200M };

            _mockUnitOfWork.Setup(uow => uow.CardRepository.GetByIDAsync(cardId))
                           .ReturnsAsync(existingCard);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _cardService.DecreaseBalanceAsync(cardId, amount));
            _mockUnitOfWork.Verify(uow => uow.SaveAsync(), Times.Never);
        }

        [Fact]
        public async Task DecreaseBalanceAsync_ShouldThrowExceptionIfInsufficientBalance()
        {
            // Arrange
            var cardId = 1L;
            var amount = 300M;
            var existingCard = new Card { ID = cardId, Balance = 200M };

            _mockUnitOfWork.Setup(uow => uow.CardRepository.GetByIDAsync(cardId))
                           .ReturnsAsync(existingCard);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _cardService.DecreaseBalanceAsync(cardId, amount));
            _mockUnitOfWork.Verify(uow => uow.SaveAsync(), Times.Never);
        }

        [Fact]
        public async Task DecreaseBalanceAsync_ShouldLogErrorOnException()
        {
            // Arrange
            long cardId = 1L;
            decimal invalidAmount = -10m;
            var expectedErrorMessage = $"Error decreasing balance for card ID: {cardId}";

            // Mock setup: Return a card for the given ID
            _mockUnitOfWork.Setup(uow => uow.CardRepository.GetByIDAsync(cardId))
                           .ReturnsAsync(new Card { ID = cardId });

            // Act and Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await _cardService.DecreaseBalanceAsync(cardId, invalidAmount);
            });

            // Verify logging: Since Moq cannot verify extension methods directly, use a workaround
            _mockLogger.Verify(
                x => x.Log(
                    It.IsAny<LogLevel>(),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => string.Equals(expectedErrorMessage, o.ToString())),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

    }
}

