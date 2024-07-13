using PaymentSystem.DataLayer.EntitiesDTO.Payment;

namespace PaymentSystem.Services.Interfaces
{
    /// <summary>
    /// Interface for payment-related operations such as processing, confirming, and canceling payments.
    /// </summary>
    public interface IPaymentService
    {
        /// <summary>
        /// Processes a payment by initiating a transaction.
        /// </summary>
        /// <param name="cardId">The ID of the card to charge.</param>
        /// <param name="totalAmount">The Total Amount to charge.</param>
        /// <param name="currency">The currency in which the amount is specified.</param>
        /// <param name="unreturnableFee">The amount which not going to return from Total Amount. Default value is zero./></param>

        /// <returns>A task that represents the asynchronous operation, containing a DTO with payment confirmation details.</returns>
        Task<ConfirmPaymentDTO> ProcessPaymentAsync(long cardId, decimal totalAmount, string currency, decimal unreturnableFee = 0);

        /// <summary>
        /// Confirms a payment using a confirmation code.
        /// </summary>
        /// <param name="transactionId">The ID of the transaction to confirm.</param>
        /// <param name="confirmationCode">The confirmation code to validate the transaction.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task ConfirmPaymentAsync(long transactionId, string confirmationCode);

        /// <summary>
        /// Cancels a payment.
        /// </summary>
        /// <param name="transactionId">The ID of the transaction to cancel.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task CancelPaymentAsync(long transactionId);

        /// <summary>
        /// Returns a payment.
        /// </summary>
        /// <param name="transactionId">The ID of the transaction to return.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task ReturnPaymentAsync(long transactionId);
    }
}
