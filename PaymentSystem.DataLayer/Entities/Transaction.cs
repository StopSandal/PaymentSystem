using System.ComponentModel.DataAnnotations.Schema;

namespace PaymentSystem.DataLayer.Entities
{
    public class Transaction
    {
        public long Id { get; set; }
        public long CardId { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalAmount { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal UnreturnableFee { get; set; } = 0;
        public string CurrencyType { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Status { get; set; }
        public string ConfirmationCode { get; set; }
        public DateTime? ConfirmationCodeExpiresAt { get; set; }
        public Card Card { get; set; }
    }
}
