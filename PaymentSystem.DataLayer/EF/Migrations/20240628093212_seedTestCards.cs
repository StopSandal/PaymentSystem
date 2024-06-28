using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaymentSystem.DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class seedTestCards : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM Cards where id < 5");

            var maxBalance = 999999999999999.99m;
            migrationBuilder.InsertData(
                table: "Cards",
                columns: new[] { "ID", "CardNumber", "CardName", "ExpirationDate", "CVV", "Balance", "CurrencyType" },
                values: new object[,]
                {
                { 1L, "1111222233334444", "Max BYN", DateTime.UtcNow.AddYears(5), 123, maxBalance, "BYN" },
                { 2L, "5555666677778888", "Max RUS", DateTime.UtcNow.AddYears(5), 456, maxBalance, "RUS" },
                { 3L, "9999000011112222", "Max USD", DateTime.UtcNow.AddYears(5), 789, maxBalance, "USD" },
                { 4L, "3333444455556666", "Max EUR", DateTime.UtcNow.AddYears(5), 101, maxBalance, "EUR" }
                });
        }
        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "ID",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "ID",
                keyValue: 2L);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "ID",
                keyValue: 3L);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "ID",
                keyValue: 4L);
        }
    }
}
