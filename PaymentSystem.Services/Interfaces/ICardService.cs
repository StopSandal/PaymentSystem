using PaymentSystem.DataLayer.Entities;
using PaymentSystem.DataLayer.EntitiesDTO.Card;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PaymentSystem.Services.Interfaces
{
    public interface ICardService
    {
        Task<Card> GetCardAsync(long Id);
        Task<Card> GetCardByCardNumberAsync(string cardNumber);
        Task<IEnumerable<Card>> GetCardsAsync();
        Task CreateCardAsync(AddCardDTO newCard);
        Task UpdateCardAsync(long id,EditCardDTO editCard);
        Task DeleteCardAsync(long id);
        Task IncreaseBalanceAsync(long id, decimal amount);
        Task DecreaseBalanceAsync(long id, decimal amount);
    }
}
