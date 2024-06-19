using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentSystem.DataLayer.Entities
{
    public class Card
    {
        public long ID { get; set; }
        public string CardNumber { get; set; }
        public string CardName { get; set; }
        public DateTime ExpirationDate { get; set; }
        public int CVV { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Balance { get; set; }
        public string CurrencyType { get; set; }


    }
}
