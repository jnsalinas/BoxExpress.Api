using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BoxExpress.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixHoldInventory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryHolds_WarehouseInventoryTransfers_TransferId",
                table: "InventoryHolds");

            migrationBuilder.RenameColumn(
                name: "TransferId",
                table: "InventoryHolds",
                newName: "WarehouseInventoryTransferDetailId");

            migrationBuilder.RenameIndex(
                name: "IX_InventoryHolds_TransferId",
                table: "InventoryHolds",
                newName: "IX_InventoryHolds_WarehouseInventoryTransferDetailId");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryHolds_WarehouseInventoryTransferDetails_WarehouseInventoryTransferDetailId",
                table: "InventoryHolds",
                column: "WarehouseInventoryTransferDetailId",
                principalTable: "WarehouseInventoryTransferDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryHolds_WarehouseInventoryTransferDetails_WarehouseInventoryTransferDetailId",
                table: "InventoryHolds");

            migrationBuilder.RenameColumn(
                name: "WarehouseInventoryTransferDetailId",
                table: "InventoryHolds",
                newName: "TransferId");

            migrationBuilder.RenameIndex(
                name: "IX_InventoryHolds_WarehouseInventoryTransferDetailId",
                table: "InventoryHolds",
                newName: "IX_InventoryHolds_TransferId");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryHolds_WarehouseInventoryTransfers_TransferId",
                table: "InventoryHolds",
                column: "TransferId",
                principalTable: "WarehouseInventoryTransfers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
