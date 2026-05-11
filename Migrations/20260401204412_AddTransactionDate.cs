using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceManager2._0.Migrations
{
    /// <inheritdoc />
    public partial class AddTransactionDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "date",
                table: "transactions",
                newName: "Date");

            migrationBuilder.RenameColumn(
                name: "start_date",
                table: "recurring_payments",
                newName: "StartDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Date",
                table: "transactions",
                newName: "date");

            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "recurring_payments",
                newName: "start_date");
        }
    }
}
