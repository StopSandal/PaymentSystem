using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PaymentSystem.DataLayer.EF;
using PaymentSystem.DataLayer.Entities;
using PaymentSystem.IntegrationTests.Helpers;
using PaymentSystem.Services.Interfaces;

namespace PaymentSystem.IntegrationTests
{
    public class PaymentSystemApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        private readonly string dataContextName = $"myTestDB_{Guid.NewGuid()}";
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remove the existing DbContextOptions<PaymentSystemContext> registration
                var dbContextDescriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                        typeof(DbContextOptions<PaymentSystemContext>));

                if (dbContextDescriptor != null)
                {
                    services.Remove(dbContextDescriptor);
                }

                // Register the DbContext with an in-memory database for testing
                services.AddDbContext<PaymentSystemContext>(options =>
                {
                    options.UseInMemoryDatabase(dataContextName);
                });

                // Build the service provider
                var sp = services.BuildServiceProvider();

                // Create a scope to obtain a reference to the database context
                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<PaymentSystemContext>();

                    // Ensure the database is created
                    db.Database.EnsureDeleted();

                    var unitOfWork = scopedServices.GetRequiredService<IUnitOfWork>();
                    SeedDatabase(unitOfWork);
                }
            });

            builder.UseEnvironment("Development");
        }

        private async void SeedDatabase(IUnitOfWork unitOfWork)
        {
            // Card for successful test
            await unitOfWork.CardRepository.InsertAsync(new Card
            {
                ID = TestConstants.NoLimitCardId,
                CardName = "InfinityAmountCard",
                CardNumber = "1234567890",
                Balance = decimal.MaxValue / 2,
                CurrencyType = TestConstants.RightCardCurrency
            });
            // Card for not successful test
            await unitOfWork.CardRepository.InsertAsync(new Card
            {
                ID = TestConstants.NoMoneyCardId,
                CardName = "ZeroAmountCard",
                CardNumber = "1234567890",
                Balance = 0,
                CurrencyType = TestConstants.RightCardCurrency
            });

            // Fully right transaction
            await unitOfWork.TransactionRepository.InsertAsync(new Transaction
            {
                Id = TestConstants.StatusPendingTransactionId,
                ConfirmationCode = TestConstants.RightConfirmationCode,
                Status = TestConstants.TransactionStatusPending,
                ConfirmationCodeExpiresAt = DateTime.MaxValue,
                CardId = TestConstants.NoLimitCardId,
                TotalAmount = TestConstants.RightMoneyAmount,
                CurrencyType = TestConstants.RightCardCurrency

            });
            // Transaction expired
            await unitOfWork.TransactionRepository.InsertAsync(new Transaction
            {
                Id = TestConstants.ExpiredTransactionId,
                ConfirmationCode = TestConstants.RightConfirmationCode,
                Status = TestConstants.TransactionStatusPending,
                ConfirmationCodeExpiresAt = DateTime.MinValue,
                CardId = TestConstants.NoLimitCardId,
                TotalAmount = TestConstants.RightMoneyAmount,
                CurrencyType = TestConstants.RightCardCurrency
            });
            // Card with no money
            await unitOfWork.TransactionRepository.InsertAsync(new Transaction
            {
                Id = TestConstants.NotEnoughMoneyTransactionId,
                ConfirmationCode = TestConstants.RightConfirmationCode,
                Status = TestConstants.TransactionStatusPending,
                ConfirmationCodeExpiresAt = DateTime.MaxValue,
                CardId = TestConstants.NoMoneyCardId,
                TotalAmount = TestConstants.RightMoneyAmount,
                CurrencyType = TestConstants.RightCardCurrency
            });
            // Transaction already canceled
            await unitOfWork.TransactionRepository.InsertAsync(new Transaction
            {
                Id = TestConstants.StatusCanceledTransactionId,
                ConfirmationCode = TestConstants.RightConfirmationCode,
                Status = TestConstants.TransactionStatusCanceled,
                ConfirmationCodeExpiresAt = DateTime.MaxValue,
                CardId = TestConstants.NoLimitCardId,
                TotalAmount = TestConstants.RightMoneyAmount,
                CurrencyType = TestConstants.RightCardCurrency
            });
            // Transaction already confirmed
            await unitOfWork.TransactionRepository.InsertAsync(new Transaction
            {
                Id = TestConstants.StatusConfirmedTransactionId,
                ConfirmationCode = TestConstants.RightConfirmationCode,
                Status = TestConstants.TransactionStatusConfirmed,
                ConfirmationCodeExpiresAt = DateTime.MaxValue,
                CardId = TestConstants.NoLimitCardId,
                TotalAmount = TestConstants.RightMoneyAmount,
                CurrencyType = TestConstants.RightCardCurrency
            });
            // Transaction already returned
            await unitOfWork.TransactionRepository.InsertAsync(new Transaction
            {
                Id = TestConstants.StatusReturnedTransactionId,
                ConfirmationCode = TestConstants.RightConfirmationCode,
                Status = TestConstants.TransactionStatusReturned,
                ConfirmationCodeExpiresAt = DateTime.MaxValue,
                CardId = TestConstants.NoLimitCardId,
                TotalAmount = TestConstants.RightMoneyAmount,
                CurrencyType = TestConstants.RightCardCurrency
            });
            await unitOfWork.SaveAsync();
        }
    }
}
