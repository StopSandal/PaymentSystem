
namespace PaymentSystem.DataLayer.EntitiesDTO.Transaction
{
    public class AddTransactionDTO
    {
        public long CardId { get; set; }
        public decimal Amount { get; set; }
        public string CurrencyType { get; set; }
        public string Status { get; set; }
        public string ConfirmationCode { get; set; }
    }
}
