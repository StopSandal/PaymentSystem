using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentSystem.DataLayer.EntitiesDTO
{
    public class BalanceUpdateDTO
    {
        public long CardId { get; set; }
        public decimal Amount { get; set; }
    }
}
