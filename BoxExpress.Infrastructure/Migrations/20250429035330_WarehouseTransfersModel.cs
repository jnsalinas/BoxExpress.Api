using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BoxExpress.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class WarehouseTransfersModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WarehouseInventoryTransfers_ProductVariants_ProductVariantId",
                table: "WarehouseInventoryTransfers");

            migrationBuilder.DropIndex(
                name: "IX_WarehouseInventoryTransfers_ProductVariantId",
                table: "WarehouseInventoryTransfers");

            migrationBuilder.DropColumn(
                name: "ProductVariantId",
                table: "WarehouseInventoryTransfers");

            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "WarehouseInventoryTransfers",
                newName: "Status");

            migrationBuilder.AddColumn<int>(
                name: "AcceptedByUserId",
                table: "WarehouseInventoryTransfers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "WarehouseInventoryTransfers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "WarehouseInventoryTransferDetail",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WarehouseInventoryTransferId = table.Column<int>(type: "int", nullable: false),
                    ProductVariantId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WarehouseInventoryTransferDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WarehouseInventoryTransferDetail_ProductVariants_ProductVariantId",
                        column: x => x.ProductVariantId,
                        principalTable: "ProductVariants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WarehouseInventoryTransferDetail_WarehouseInventoryTransfers_WarehouseInventoryTransferId",
                        column: x => x.WarehouseInventoryTransferId,
                        principalTable: "WarehouseInventoryTransfers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseInventoryTransfers_AcceptedByUserId",
                table: "WarehouseInventoryTransfers",
                column: "AcceptedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseInventoryTransferDetail_ProductVariantId",
                table: "WarehouseInventoryTransferDetail",
                column: "ProductVariantId");

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseInventoryTransferDetail_WarehouseInventoryTransferId",
                table: "WarehouseInventoryTransferDetail",
                column: "WarehouseInventoryTransferId");

            migrationBuilder.AddForeignKey(
                name: "FK_WarehouseInventoryTransfers_Users_AcceptedByUserId",
                table: "WarehouseInventoryTransfers",
                column: "AcceptedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WarehouseInventoryTransfers_Users_AcceptedByUserId",
                table: "WarehouseInventoryTransfers");

            migrationBuilder.DropTable(
                name: "WarehouseInventoryTransferDetail");

            migrationBuilder.DropIndex(
                name: "IX_WarehouseInventoryTransfers_AcceptedByUserId",
                table: "WarehouseInventoryTransfers");

            migrationBuilder.DropColumn(
                name: "AcceptedByUserId",
                table: "WarehouseInventoryTransfers");

            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "WarehouseInventoryTransfers");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "WarehouseInventoryTransfers",
                newName: "Quantity");

            migrationBuilder.AddColumn<int>(
                name: "ProductVariantId",
                table: "WarehouseInventoryTransfers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseInventoryTransfers_ProductVariantId",
                table: "WarehouseInventoryTransfers",
                column: "ProductVariantId");

            migrationBuilder.AddForeignKey(
                name: "FK_WarehouseInventoryTransfers_ProductVariants_ProductVariantId",
                table: "WarehouseInventoryTransfers",
                column: "ProductVariantId",
                principalTable: "ProductVariants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
