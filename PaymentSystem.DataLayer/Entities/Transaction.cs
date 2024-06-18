namespace PaymentSystem.DataLayer.Entities
{
    public class Transaction
    {
        public long Id { get; set; }
        public long CardId { get; set; }
        public decimal Amount { get; set; }
        public string CurrencyType { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Status { get; set; }
        public string ConfirmationCode { get; set; }
        public DateTime? ConfirmationCodeExpiresAt { get; set; }
        public Card Card { get; set; }
    }
}
