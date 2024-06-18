using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentSystem.DataLayer.EntitiesDTO.Card
{
    public class EditCardDTO
    {
        public string? CardName { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public int? CVV { get; set; }
    }
}
