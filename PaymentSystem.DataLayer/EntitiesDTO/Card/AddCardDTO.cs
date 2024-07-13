using System.ComponentModel.DataAnnotations;

namespace PaymentSystem.DataLayer.EntitiesDTO.Card
{
    public class AddCardDTO
    {
        [Required]
        public string CardNumber { get; set; }
        [Required]
        public string CardName { get; set; }
        [Required]
        public DateTime ExpirationDate { get; set; }
        [Required]
        public int CVV { get; set; }
        [Required]
        public string CurrencyType { get; set; }
    }
}
