using Microsoft.EntityFrameworkCore;
using PaymentSystem.DataLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentSystem.DataLayer.EF
{
    public class PaymentSystemContext : DbContext
    {
        DbSet<Card> Cards { get; set; }
        DbSet<Transaction> Transactions { get; set;}
        public PaymentSystemContext(DbContextOptions options) : base(options)
        {
        }
    }
}
