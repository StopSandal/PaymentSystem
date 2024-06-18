using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentSystem.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<string> ProcessPaymentAsync(long cardId, decimal amount, string currency);
        Task<bool> ConfirmPaymentAsync(long transactionId, string confirmationCode);
        Task CancelPaymentAsync(long transactionId);
    }
}
