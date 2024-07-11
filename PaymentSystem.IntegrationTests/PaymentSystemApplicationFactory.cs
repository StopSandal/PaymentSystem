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
                ID = TestConstants.NO_LIMIT_CARD_ID,
                CardName = "InfinityAmountCard",
                CardNumber = "1234567890",
                Balance = decimal.MaxValue / 2,
                CurrencyType = TestConstants.RIGHT_CARD_CURRENCY
            });
            // Card for not successful test
            await unitOfWork.CardRepository.InsertAsync(new Card
            {
                ID = TestConstants.NO_MONEY_CARD_ID,
                CardName = "ZeroAmountCard",
                CardNumber = "1234567890",
                Balance = 0,
                CurrencyType = TestConstants.RIGHT_CARD_CURRENCY
            });

            // Fully right transaction
            await unitOfWork.TransactionRepository.InsertAsync(new Transaction
            {
                Id = TestConstants.STATUS_PENDING_TRANSACTION_ID,
                ConfirmationCode = TestConstants.RIGHT_CONFIRMATION_CODE,
                Status = TestConstants.TRANSACTION_STATUS_PENDING,
                ConfirmationCodeExpiresAt = DateTime.MaxValue,
                CardId = TestConstants.NO_LIMIT_CARD_ID,
                TotalAmount = TestConstants.RIGHT_MONEY_AMOUNT,
                CurrencyType = TestConstants.RIGHT_CARD_CURRENCY

            });
            // Transaction expired
            await unitOfWork.TransactionRepository.InsertAsync(new Transaction
            {
                Id = TestConstants.EXPIRED_TRANSACTION_ID,
                ConfirmationCode = TestConstants.RIGHT_CONFIRMATION_CODE,
                Status = TestConstants.TRANSACTION_STATUS_PENDING,
                ConfirmationCodeExpiresAt = DateTime.MinValue,
                CardId = TestConstants.NO_LIMIT_CARD_ID,
                TotalAmount = TestConstants.RIGHT_MONEY_AMOUNT,
                CurrencyType = TestConstants.RIGHT_CARD_CURRENCY
            });
            // Card with no money
            await unitOfWork.TransactionRepository.InsertAsync(new Transaction
            {
                Id = TestConstants.NOT_ENOUGH_MONEY_TRANSACTION_ID,
                ConfirmationCode = TestConstants.RIGHT_CONFIRMATION_CODE,
                Status = TestConstants.TRANSACTION_STATUS_PENDING,
                ConfirmationCodeExpiresAt = DateTime.MaxValue,
                CardId = TestConstants.NO_MONEY_CARD_ID,
                TotalAmount = TestConstants.RIGHT_MONEY_AMOUNT,
                CurrencyType = TestConstants.RIGHT_CARD_CURRENCY
            });
            // Transaction already canceled
            await unitOfWork.TransactionRepository.InsertAsync(new Transaction
            {
                Id = TestConstants.STATUS_CANCELED_TRANSACTION_ID,
                ConfirmationCode = TestConstants.RIGHT_CONFIRMATION_CODE,
                Status = TestConstants.TRANSACTION_STATUS_CANCELED,
                ConfirmationCodeExpiresAt = DateTime.MaxValue,
                CardId = TestConstants.NO_LIMIT_CARD_ID,
                TotalAmount = TestConstants.RIGHT_MONEY_AMOUNT,
                CurrencyType = TestConstants.RIGHT_CARD_CURRENCY
            });
            // Transaction already confirmed
            await unitOfWork.TransactionRepository.InsertAsync(new Transaction
            {
                Id = TestConstants.STATUS_CONFIRMED_TRANSACTION_ID,
                ConfirmationCode = TestConstants.RIGHT_CONFIRMATION_CODE,
                Status = TestConstants.TRANSACTION_STATUS_CONFIRMED,
                ConfirmationCodeExpiresAt = DateTime.MaxValue,
                CardId = TestConstants.NO_LIMIT_CARD_ID,
                TotalAmount = TestConstants.RIGHT_MONEY_AMOUNT,
                CurrencyType = TestConstants.RIGHT_CARD_CURRENCY
            });
            // Transaction already returned
            await unitOfWork.TransactionRepository.InsertAsync(new Transaction
            {
                Id = TestConstants.STATUS_RETURNED_TRANSACTION_ID,
                ConfirmationCode = TestConstants.RIGHT_CONFIRMATION_CODE,
                Status = TestConstants.TRANSACTION_STATUS_RETUNED,
                ConfirmationCodeExpiresAt = DateTime.MaxValue,
                CardId = TestConstants.NO_LIMIT_CARD_ID,
                TotalAmount = TestConstants.RIGHT_MONEY_AMOUNT,
                CurrencyType = TestConstants.RIGHT_CARD_CURRENCY
            });
            await unitOfWork.SaveAsync();
        }
    }
}
