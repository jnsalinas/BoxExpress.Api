using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BoxExpress.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixProductLoans : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "QuantityDelivered",
                table: "WarehouseInventories",
                newName: "DeliveredQuantity");

            migrationBuilder.AddColumn<int>(
                name: "ProductLoanDetailId",
                table: "InventoryMovements",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventoryMovements_ProductLoanDetailId",
                table: "InventoryMovements",
                column: "ProductLoanDetailId");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryMovements_ProductLoanDetails_ProductLoanDetailId",
                table: "InventoryMovements",
                column: "ProductLoanDetailId",
                principalTable: "ProductLoanDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryMovements_ProductLoanDetails_ProductLoanDetailId",
                table: "InventoryMovements");

            migrationBuilder.DropIndex(
                name: "IX_InventoryMovements_ProductLoanDetailId",
                table: "InventoryMovements");

            migrationBuilder.DropColumn(
                name: "ProductLoanDetailId",
                table: "InventoryMovements");

            migrationBuilder.RenameColumn(
                name: "DeliveredQuantity",
                table: "WarehouseInventories",
                newName: "QuantityDelivered");
        }
    }
}
