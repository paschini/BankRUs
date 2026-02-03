using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankRUs.Intrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CorrectTypeForBalanceAfterTransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "BalanceAfterTransaction",
                table: "BankAccountTransactions",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "BalanceAfterTransaction",
                table: "BankAccountTransactions",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");
        }
    }
}
