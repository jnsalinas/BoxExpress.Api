using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BoxExpress.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatorInventoryMovement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CreatorId",
                table: "InventoryMovements",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventoryMovements_CreatorId",
                table: "InventoryMovements",
                column: "CreatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryMovements_Users_CreatorId",
                table: "InventoryMovements",
                column: "CreatorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryMovements_Users_CreatorId",
                table: "InventoryMovements");

            migrationBuilder.DropIndex(
                name: "IX_InventoryMovements_CreatorId",
                table: "InventoryMovements");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "InventoryMovements");
        }
    }
}
