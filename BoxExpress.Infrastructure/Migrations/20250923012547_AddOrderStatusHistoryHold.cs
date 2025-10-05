using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BoxExpress.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderStatusHistoryHold : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrderStatusHistoryId",
                table: "InventoryHolds",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventoryHolds_OrderStatusHistoryId",
                table: "InventoryHolds",
                column: "OrderStatusHistoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryHolds_OrderStatusHistories_OrderStatusHistoryId",
                table: "InventoryHolds",
                column: "OrderStatusHistoryId",
                principalTable: "OrderStatusHistories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryHolds_OrderStatusHistories_OrderStatusHistoryId",
                table: "InventoryHolds");

            migrationBuilder.DropIndex(
                name: "IX_InventoryHolds_OrderStatusHistoryId",
                table: "InventoryHolds");

            migrationBuilder.DropColumn(
                name: "OrderStatusHistoryId",
                table: "InventoryHolds");
        }
    }
}
