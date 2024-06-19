using PaymentSystem.DataLayer.EntitiesDTO.Payment;

namespace PaymentSystem.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<ConfirmPaymentDTO> ProcessPaymentAsync(long cardId, decimal amount, string currency);
        Task ConfirmPaymentAsync(long transactionId, string confirmationCode);
        Task CancelPaymentAsync(long transactionId);
    }
}
