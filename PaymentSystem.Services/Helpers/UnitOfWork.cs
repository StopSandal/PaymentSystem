using PaymentSystem.DataLayer.EF;
using PaymentSystem.DataLayer.Entities;
using PaymentSystem.Services.Interfaces;

namespace PaymentSystem.Services.Helpers
{
    /// <summary>
    /// Represents the default implementation of the <see cref="IUnitOfWork"/>.
    /// </summary>
    /// <inheritdoc cref="IUnitOfWork"/>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly PaymentSystemContext context;
        private IRepositoryAsync<Card> cardRepository;
        private IRepositoryAsync<Transaction> transactionRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWork"/> class with the specified context.
        /// </summary>
        /// <param name="sellerContext">The database context.</param>
        public UnitOfWork(PaymentSystemContext paymentSystemContext)
        {
            context = paymentSystemContext;
        }
        /// <inheritdoc />
        public IRepositoryAsync<Card> CardRepository
        {
            get
            {
                this.cardRepository ??= new GenericRepository<Card>(context);

                return cardRepository;
            }
        }
        /// <inheritdoc />
        public IRepositoryAsync<Transaction> TransactionRepository
        {
            get
            {
                this.transactionRepository ??= new GenericRepository<Transaction>(context);

                return transactionRepository;
            }
        }

        /// <inheritdoc/>
        public Task SaveAsync()
        {
            return context.SaveChangesAsync();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
