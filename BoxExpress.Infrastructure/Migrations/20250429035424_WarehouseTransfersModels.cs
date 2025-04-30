using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BoxExpress.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class WarehouseTransfersModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WarehouseInventoryTransferDetail_ProductVariants_ProductVariantId",
                table: "WarehouseInventoryTransferDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_WarehouseInventoryTransferDetail_WarehouseInventoryTransfers_WarehouseInventoryTransferId",
                table: "WarehouseInventoryTransferDetail");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WarehouseInventoryTransferDetail",
                table: "WarehouseInventoryTransferDetail");

            migrationBuilder.RenameTable(
                name: "WarehouseInventoryTransferDetail",
                newName: "WarehouseInventoryTransferDetails");

            migrationBuilder.RenameIndex(
                name: "IX_WarehouseInventoryTransferDetail_WarehouseInventoryTransferId",
                table: "WarehouseInventoryTransferDetails",
                newName: "IX_WarehouseInventoryTransferDetails_WarehouseInventoryTransferId");

            migrationBuilder.RenameIndex(
                name: "IX_WarehouseInventoryTransferDetail_ProductVariantId",
                table: "WarehouseInventoryTransferDetails",
                newName: "IX_WarehouseInventoryTransferDetails_ProductVariantId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WarehouseInventoryTransferDetails",
                table: "WarehouseInventoryTransferDetails",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WarehouseInventoryTransferDetails_ProductVariants_ProductVariantId",
                table: "WarehouseInventoryTransferDetails",
                column: "ProductVariantId",
                principalTable: "ProductVariants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WarehouseInventoryTransferDetails_WarehouseInventoryTransfers_WarehouseInventoryTransferId",
                table: "WarehouseInventoryTransferDetails",
                column: "WarehouseInventoryTransferId",
                principalTable: "WarehouseInventoryTransfers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WarehouseInventoryTransferDetails_ProductVariants_ProductVariantId",
                table: "WarehouseInventoryTransferDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_WarehouseInventoryTransferDetails_WarehouseInventoryTransfers_WarehouseInventoryTransferId",
                table: "WarehouseInventoryTransferDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WarehouseInventoryTransferDetails",
                table: "WarehouseInventoryTransferDetails");

            migrationBuilder.RenameTable(
                name: "WarehouseInventoryTransferDetails",
                newName: "WarehouseInventoryTransferDetail");

            migrationBuilder.RenameIndex(
                name: "IX_WarehouseInventoryTransferDetails_WarehouseInventoryTransferId",
                table: "WarehouseInventoryTransferDetail",
                newName: "IX_WarehouseInventoryTransferDetail_WarehouseInventoryTransferId");

            migrationBuilder.RenameIndex(
                name: "IX_WarehouseInventoryTransferDetails_ProductVariantId",
                table: "WarehouseInventoryTransferDetail",
                newName: "IX_WarehouseInventoryTransferDetail_ProductVariantId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WarehouseInventoryTransferDetail",
                table: "WarehouseInventoryTransferDetail",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WarehouseInventoryTransferDetail_ProductVariants_ProductVariantId",
                table: "WarehouseInventoryTransferDetail",
                column: "ProductVariantId",
                principalTable: "ProductVariants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WarehouseInventoryTransferDetail_WarehouseInventoryTransfers_WarehouseInventoryTransferId",
                table: "WarehouseInventoryTransferDetail",
                column: "WarehouseInventoryTransferId",
                principalTable: "WarehouseInventoryTransfers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
