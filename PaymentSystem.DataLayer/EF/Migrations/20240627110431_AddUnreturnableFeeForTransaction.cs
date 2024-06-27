using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaymentSystem.DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddUnreturnableFeeForTransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "Transactions",
                newName: "UnreturnableFee");

            migrationBuilder.AddColumn<decimal>(
                name: "TotalAmount",
                table: "Transactions",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalAmount",
                table: "Transactions");

            migrationBuilder.RenameColumn(
                name: "UnreturnableFee",
                table: "Transactions",
                newName: "Amount");
        }
    }
}
