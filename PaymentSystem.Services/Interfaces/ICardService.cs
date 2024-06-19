using PaymentSystem.DataLayer.Entities;
using PaymentSystem.DataLayer.EntitiesDTO.Card;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PaymentSystem.Services.Interfaces
{
    /// <summary>
    /// Interface for CRUD operations with card and managing card balances.
    /// </summary>
    public interface ICardService
    {
        /// <summary>
        /// Retrieves a card by its ID.
        /// </summary>
        /// <param name="id">The ID of the card to retrieve.</param>
        /// <returns>A task that represents the asynchronous operation, containing the card with the specified ID.</returns>
        Task<Card> GetCardAsync(long id);

        /// <summary>
        /// Retrieves a card by its card number.
        /// </summary>
        /// <param name="cardNumber">The card number to search for.</param>
        /// <returns>A task that represents the asynchronous operation, containing the card with the specified card number.</returns>
        Task<Card> GetCardByCardNumberAsync(string cardNumber);

        /// <summary>
        /// Retrieves all cards.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation, containing a collection of all cards.</returns>
        Task<IEnumerable<Card>> GetCardsAsync();

        /// <summary>
        /// Creates a new card.
        /// </summary>
        /// <param name="newCard">The data transfer object containing the details of the new card.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task CreateCardAsync(AddCardDTO newCard);

        /// <summary>
        /// Updates an existing card.
        /// </summary>
        /// <param name="id">The ID of the card to update.</param>
        /// <param name="editCard">The data transfer object containing the updated details of the card.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task UpdateCardAsync(long id, EditCardDTO editCard);

        /// <summary>
        /// Deletes a card by its ID.
        /// </summary>
        /// <param name="id">The ID of the card to delete.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task DeleteCardAsync(long id);

        /// <summary>
        /// Increases the balance of a card.
        /// </summary>
        /// <param name="id">The ID of the card to increase the balance of.</param>
        /// <param name="amount">The amount to increase the balance by.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task IncreaseBalanceAsync(long id, decimal amount);

        /// <summary>
        /// Decreases the balance of a card.
        /// </summary>
        /// <param name="id">The ID of the card to decrease the balance of.</param>
        /// <param name="amount">The amount to decrease the balance by.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task DecreaseBalanceAsync(long id, decimal amount);
    }
}
