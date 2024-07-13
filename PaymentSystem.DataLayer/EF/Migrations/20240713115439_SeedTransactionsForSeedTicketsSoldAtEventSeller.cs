using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaymentSystem.DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class SeedTransactionsForSeedTicketsSoldAtEventSeller : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($"Delete Transactions WHERE ID <= 150"); // if already exists

            List<object[]> transactionsList = new List<object[]>();

             for (int startTransactionId = 0; startTransactionId < 150; startTransactionId++)
             {
                 transactionsList.Add(new object[]
                 {
                 startTransactionId++, // Id
                 3, // CardId
                 10.00m, // TotalAmount
                 1.00m, // UnreturnableFee
                 "USD", // CurrencyType
                 DateTime.UtcNow, // TransactionDate
                 "Confirmed", // Status
                 $"CONFIRM{startTransactionId}", // ConfirmationCode
                 DateTime.UtcNow, // ConfirmationCodeExpiresAt
                 });
             }

            object[,] transaction = new object[transactionsList.Count, transactionsList[0].Length];
            for (int i = 0; i < transactionsList.Count; i++)
            {
                for (int j = 0; j < transactionsList[i].Length; j++)
                {
                    transaction[i, j] = transactionsList[i][j];
                }
            }

            migrationBuilder.InsertData(
                table: "Transactions",
                columns: new[] { "Id", "CardId", "TotalAmount", "UnreturnableFee", "CurrencyType", "TransactionDate", "Status", "ConfirmationCode", "ConfirmationCodeExpiresAt" },
                values: transaction
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($"Delete Transactions WHERE ID <= 150");
        }
    }
}
