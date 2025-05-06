using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BoxExpress.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class WithdrawalwalletRecord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "RelatedOrderId",
                table: "WalletTransactions",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "OrderStatusId",
                table: "WalletTransactions",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "RelatedWithdrawalRequestId",
                table: "WalletTransactions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WalletTransactions_RelatedWithdrawalRequestId",
                table: "WalletTransactions",
                column: "RelatedWithdrawalRequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_WalletTransactions_WithdrawalRequests_RelatedWithdrawalRequestId",
                table: "WalletTransactions",
                column: "RelatedWithdrawalRequestId",
                principalTable: "WithdrawalRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WalletTransactions_WithdrawalRequests_RelatedWithdrawalRequestId",
                table: "WalletTransactions");

            migrationBuilder.DropIndex(
                name: "IX_WalletTransactions_RelatedWithdrawalRequestId",
                table: "WalletTransactions");

            migrationBuilder.DropColumn(
                name: "RelatedWithdrawalRequestId",
                table: "WalletTransactions");

            migrationBuilder.AlterColumn<int>(
                name: "RelatedOrderId",
                table: "WalletTransactions",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "OrderStatusId",
                table: "WalletTransactions",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
