using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentSystem.DataLayer.Entities
{
    public class Transaction
    {
        public long Id { get; set; }
        public long CardId { get; set; }
        public decimal Amount { get; set; }
        public string CurrencyType { get; set; }
        public DateTime TransactionDate { get; set; }
        public Card Card { get; set; }
    }
}
