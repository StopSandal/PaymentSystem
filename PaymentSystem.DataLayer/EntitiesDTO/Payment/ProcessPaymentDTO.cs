using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentSystem.DataLayer.EntitiesDTO.Payment
{
    public class ProcessPaymentDTO
    {
        public long CardId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
    }
}
