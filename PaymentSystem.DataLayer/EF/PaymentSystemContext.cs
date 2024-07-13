using Microsoft.EntityFrameworkCore;
using PaymentSystem.DataLayer.Entities;

namespace PaymentSystem.DataLayer.EF
{
    public class PaymentSystemContext : DbContext
    {
        DbSet<Card> Cards { get; set; }
        DbSet<Transaction> Transactions { get; set; }
        public PaymentSystemContext(DbContextOptions options) : base(options)
        {
        }
    }
}
