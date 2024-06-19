using AutoMapper;
using Microsoft.Extensions.Logging;
using PaymentSystem.DataLayer.Entities;
using PaymentSystem.DataLayer.EntitiesDTO.Card;
using PaymentSystem.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentSystem.Services.Services
{
    public class CardService : ICardService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CardService> _logger;

        public CardService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<CardService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Card> GetCardAsync(long id)
        {
            try
            {
                _logger.LogInformation("Fetching card with ID: {CardId}", id);
                var card = await _unitOfWork.CardRepository.GetByIDAsync(id);
                if (card == null)
                {
                    _logger.LogWarning("Card with ID: {CardId} not found", id);
                }
                return card;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching card with ID: {CardId}", id);
                throw;
            }
        }

        public async Task<Card> GetCardByCardNumberAsync(string cardNumber)
        {
            try
            {
                _logger.LogInformation("Fetching card with card number: {CardNumber}", cardNumber);
                var card = (await _unitOfWork.CardRepository.GetAsync(c => c.CardNumber == cardNumber)).FirstOrDefault();
                if (card == null)
                {
                    _logger.LogWarning("Card with card number: {CardNumber} not found", cardNumber);
                }
                return card;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching card with card number: {CardNumber}", cardNumber);
                throw;
            }
        }

        public async Task<IEnumerable<Card>> GetCardsAsync()
        {
            try
            {
                _logger.LogInformation("Fetching all cards");
                return await _unitOfWork.CardRepository.GetAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all cards");
                throw;
            }
        }

        public async Task CreateCardAsync(AddCardDTO newCard)
        {
            try
            {
                _logger.LogInformation("Creating a new card");
                await _unitOfWork.CardRepository.InsertAsync(_mapper.Map<Card>(newCard));
                await _unitOfWork.SaveAsync();
                _logger.LogInformation("Card created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating new card");
                throw;
            }
        }

        public async Task DecreaseBalanceAsync(long id, decimal amount)
        {
            try
            {
                _logger.LogInformation("Decreasing balance for card ID: {CardId} by amount: {Amount}", id, amount);
                var card = await GetCardAsync(id);
                if (card == null)
                {
                    _logger.LogWarning("Card with ID: {CardId} not found", id);
                    throw new InvalidDataException("Card doesn't exist");
                }
                if (amount <= 0)
                {
                    _logger.LogWarning("Invalid amount: {Amount} for decreasing balance", amount);
                    throw new InvalidOperationException("Amount should be greater than 0");
                }
                if (card.Balance < amount)
                {
                    _logger.LogWarning("Insufficient balance for card ID: {CardId}. Available: {Balance}, Required: {Amount}", id, card.Balance, amount);
                    throw new InvalidOperationException("Operation canceled: insufficient balance");
                }
                card.Balance -= amount;
                await _unitOfWork.SaveAsync();
                _logger.LogInformation("Balance decreased successfully for card ID: {CardId}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error decreasing balance for card ID: {CardId}", id);
                throw;
            }
        }

        public async Task DeleteCardAsync(long id)
        {
            try
            {
                _logger.LogInformation("Deleting card with ID: {CardId}", id);
                await _unitOfWork.CardRepository.DeleteAsync(id);
                await _unitOfWork.SaveAsync();
                _logger.LogInformation("Card deleted successfully with ID: {CardId}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting card with ID: {CardId}", id);
                throw;
            }
        }

        public async Task IncreaseBalanceAsync(long id, decimal amount)
        {
            try
            {
                _logger.LogInformation("Increasing balance for card ID: {CardId} by amount: {Amount}", id, amount);
                if (amount <= 0)
                {
                    _logger.LogWarning("Invalid amount: {Amount} for increasing balance", amount);
                    throw new InvalidOperationException("Amount should be greater than 0");
                }
                var card = await GetCardAsync(id);
                if (card == null)
                {
                    _logger.LogWarning("Card with ID: {CardId} not found", id);
                    throw new InvalidDataException("Card doesn't exist");
                }
                card.Balance += amount;
                await _unitOfWork.SaveAsync();
                _logger.LogInformation("Balance increased successfully for card ID: {CardId}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error increasing balance for card ID: {CardId}", id);
                throw;
            }
        }

        public async Task UpdateCardAsync(long id, EditCardDTO editCard)
        {
            try
            {
                _logger.LogInformation("Updating card with ID: {CardId}", id);
                var item = await _unitOfWork.CardRepository.GetByIDAsync(id);
                if (item == null)
                {
                    _logger.LogWarning("No card found to update with ID: {CardId}", id);
                    throw new InvalidDataException("No card to update");
                }
                _mapper.Map(editCard, item);
                _unitOfWork.CardRepository.Update(item);
                await _unitOfWork.SaveAsync();
                _logger.LogInformation("Card updated successfully with ID: {CardId}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating card with ID: {CardId}", id);
                throw;
            }
        }
    }
}
