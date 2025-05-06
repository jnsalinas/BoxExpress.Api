using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BoxExpress.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class WithdrawalRequestBank : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WithdrawalRequests_Banks_BankId",
                table: "WithdrawalRequests");

            migrationBuilder.DropIndex(
                name: "IX_WithdrawalRequests_BankId",
                table: "WithdrawalRequests");

            migrationBuilder.DropColumn(
                name: "BankId",
                table: "WithdrawalRequests");

            migrationBuilder.AddColumn<string>(
                name: "Bank",
                table: "WithdrawalRequests",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Bank",
                table: "WithdrawalRequests");

            migrationBuilder.AddColumn<int>(
                name: "BankId",
                table: "WithdrawalRequests",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WithdrawalRequests_BankId",
                table: "WithdrawalRequests",
                column: "BankId");

            migrationBuilder.AddForeignKey(
                name: "FK_WithdrawalRequests_Banks_BankId",
                table: "WithdrawalRequests",
                column: "BankId",
                principalTable: "Banks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
