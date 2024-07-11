namespace PaymentSystem.DataLayer.EntitiesDTO.Payment
{
    public class ConfirmPaymentDTO
    {
        public long TransactionId { get; set; }
        public string ConfirmationCode { get; set; }
    }
}
