using PaymentSystem.DataLayer.Entities;
using PaymentSystem.DataLayer.EntitiesDTO.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentSystem.Services.Interfaces
{
    public interface ITransactionService
    {
        Task<Transaction> GetTransactionByIdAsync(long transactionId);
        Task<IEnumerable<Transaction>> GetAllTransactionsAsync();
        Task<Transaction> CreateAsync(AddTransactionDTO newTransaction);
        Task<Transaction> ConfirmTransactionAsync(long transactionId, string confirmationCode);
        Task CancelTransactionAsync(long transactionId);
    }
}
