using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentSystem.DataLayer.EntitiesDTO.Payment
{
    public class ConfirmPaymentDTO
    {
        public long TransactionId { get; set; }
        public string ConfirmationCode { get; set; }
    }
}
