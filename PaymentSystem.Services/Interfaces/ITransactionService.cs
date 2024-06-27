using PaymentSystem.DataLayer.Entities;
using PaymentSystem.DataLayer.EntitiesDTO.Transaction;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PaymentSystem.Services.Interfaces
{
    /// <summary>
    /// Interface for transaction-related operations such as creating, confirming, and retrieving transactions.
    /// </summary>
    public interface ITransactionService
    {
        /// <summary>
        /// Retrieves a transaction by its ID.
        /// </summary>
        /// <param name="transactionId">The ID of the transaction to retrieve.</param>
        /// <returns>A task that represents the asynchronous operation, containing the transaction with the specified ID.</returns>
        Task<Transaction> GetTransactionByIdAsync(long transactionId);

        /// <summary>
        /// Retrieves all transactions.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation, containing a collection of all transactions.</returns>
        Task<IEnumerable<Transaction>> GetAllTransactionsAsync();

        /// <summary>
        /// Creates a new transaction.
        /// </summary>
        /// <param name="newTransaction">The data transfer object containing the details of the new transaction.</param>
        /// <returns>A task that represents the asynchronous operation, containing the created transaction.</returns>
        Task<Transaction> CreateAsync(AddTransactionDTO newTransaction);

        /// <summary>
        /// Confirms a transaction using a confirmation code.
        /// </summary>
        /// <param name="transactionId">The ID of the transaction to confirm.</param>
        /// <param name="confirmationCode">The confirmation code to validate the transaction.</param>
        /// <returns>A task that represents the asynchronous operation, containing the confirmed transaction.</returns>
        Task<Transaction> ConfirmTransactionAsync(long transactionId, string confirmationCode);

        /// <summary>
        /// Cancels a transaction.
        /// </summary>
        /// <param name="transactionId">The ID of the transaction to cancel.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task CancelTransactionAsync(long transactionId);

        /// <summary>
        /// Returns action of transaction.
        /// </summary>
        /// <param name="transactionId">The ID of the transaction to cancel.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task<Transaction> ReturnTransactionAsync(long transactionId);
    }
}
