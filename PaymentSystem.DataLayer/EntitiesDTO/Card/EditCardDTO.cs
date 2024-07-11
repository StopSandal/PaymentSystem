namespace PaymentSystem.DataLayer.EntitiesDTO.Card
{
    public class EditCardDTO
    {
        public string? CardName { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public int? CVV { get; set; }
    }
}
