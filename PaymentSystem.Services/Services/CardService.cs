using AutoMapper;
using PaymentSystem.DataLayer.Entities;
using PaymentSystem.DataLayer.EntitiesDTO.Card;
using PaymentSystem.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentSystem.Services.Services
{
    public class CardService : ICardService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CardService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Card> GetCardAsync(long Id)
        {
            return await _unitOfWork.CardRepository.GetByIDAsync(Id);
        }

        public async Task<Card> GetCardByCardNumberAsync(string cardNumber)
        {
            return (await _unitOfWork.CardRepository.GetAsync(card => card.CardNumber == cardNumber)).FirstOrDefault();
        }

        public async Task<IEnumerable<Card>> GetCardsAsync()
        {
            return await _unitOfWork.CardRepository.GetAsync();
        }
        public async Task CreateCardAsync(AddCardDTO newCard)
        {
            await _unitOfWork.CardRepository.InsertAsync(_mapper.Map<Card>(newCard));
            await _unitOfWork.SaveAsync();
        }

        public async Task DecreaseBalanceAsync(long id, decimal amount)
        {
            var card = await GetCardAsync(id);
            if (card == null) {
                throw new InvalidDataException("Card doesn't exists");
            }
            if (amount <= 0)
            {
                throw new InvalidOperationException("Amount should be greater then 0");
            }
            if (card.Balance < amount)
            {
                throw new InvalidOperationException("Operation canceled: insufficient balance");
            }
            card.Balance -= amount;
            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteCardAsync(long id)
        {
            await _unitOfWork.CardRepository.DeleteAsync(id);
            await _unitOfWork.SaveAsync();
        }
        public async Task IncreaseBalanceAsync(long id, decimal amount)
        {
            if (amount <= 0) 
            {
                throw new InvalidOperationException("Amount should be greater then 0");
            }
            var card = await GetCardAsync(id);
            if (card == null)
            {
                throw new InvalidDataException("Card doesn't exists");
            }
            card.Balance += amount;
            await _unitOfWork.SaveAsync();
        }

        public async Task UpdateCardAsync(long id, EditCardDTO editCard)
        {
            var item = await _unitOfWork.CardRepository.GetByIDAsync(id);
            if (item == null)
                throw new NullReferenceException("No Card to update");
            _mapper.Map(editCard, item);
            _unitOfWork.CardRepository.Update(item);
            await _unitOfWork.SaveAsync();
        }
    }
}
