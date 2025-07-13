using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BoxExpress.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddStoreToWarehouseInventory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StoreId",
                table: "WarehouseInventories",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseInventories_StoreId",
                table: "WarehouseInventories",
                column: "StoreId");

            migrationBuilder.AddForeignKey(
                name: "FK_WarehouseInventories_Stores_StoreId",
                table: "WarehouseInventories",
                column: "StoreId",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WarehouseInventories_Stores_StoreId",
                table: "WarehouseInventories");

            migrationBuilder.DropIndex(
                name: "IX_WarehouseInventories_StoreId",
                table: "WarehouseInventories");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "WarehouseInventories");
        }
    }
}
