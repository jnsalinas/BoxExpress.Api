using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BoxExpress.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixWallet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Wallets_Stores_StoreId",
                table: "Wallets");

            migrationBuilder.DropIndex(
                name: "IX_Wallets_StoreId",
                table: "Wallets");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "Wallets");

            migrationBuilder.AddColumn<int>(
                name: "WalletId",
                table: "Stores",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Stores_WalletId",
                table: "Stores",
                column: "WalletId");

            migrationBuilder.AddForeignKey(
                name: "FK_Stores_Wallets_WalletId",
                table: "Stores",
                column: "WalletId",
                principalTable: "Wallets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stores_Wallets_WalletId",
                table: "Stores");

            migrationBuilder.DropIndex(
                name: "IX_Stores_WalletId",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "WalletId",
                table: "Stores");

            migrationBuilder.AddColumn<int>(
                name: "StoreId",
                table: "Wallets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Wallets_StoreId",
                table: "Wallets",
                column: "StoreId");

            migrationBuilder.AddForeignKey(
                name: "FK_Wallets_Stores_StoreId",
                table: "Wallets",
                column: "StoreId",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
