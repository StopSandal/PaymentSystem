namespace PaymentSystem.DataLayer.EntitiesDTO.Payment
{
    public class ProcessPaymentDTO
    {
        public long CardId { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal UnreturnableFee { get; set; }
        public string Currency { get; set; }
    }
}
