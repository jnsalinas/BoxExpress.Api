using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BoxExpress.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class WarehouseTransfersCreatorId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CreatorId",
                table: "WarehouseInventoryTransfers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseInventoryTransfers_CreatorId",
                table: "WarehouseInventoryTransfers",
                column: "CreatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_WarehouseInventoryTransfers_Users_CreatorId",
                table: "WarehouseInventoryTransfers",
                column: "CreatorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WarehouseInventoryTransfers_Users_CreatorId",
                table: "WarehouseInventoryTransfers");

            migrationBuilder.DropIndex(
                name: "IX_WarehouseInventoryTransfers_CreatorId",
                table: "WarehouseInventoryTransfers");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "WarehouseInventoryTransfers");
        }
    }
}
