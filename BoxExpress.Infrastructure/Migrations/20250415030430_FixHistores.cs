using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BoxExpress.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixHistores : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderCategoryHistories_Users_UserId",
                table: "OrderCategoryHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderStatusHistories_Users_UserId",
                table: "OrderStatusHistories");

            migrationBuilder.DropColumn(
                name: "ChangedBy",
                table: "OrderStatusHistories");

            migrationBuilder.DropColumn(
                name: "ChangedBy",
                table: "OrderCategoryHistories");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "OrderStatusHistories",
                newName: "CreatorId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderStatusHistories_UserId",
                table: "OrderStatusHistories",
                newName: "IX_OrderStatusHistories_CreatorId");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "OrderCategoryHistories",
                newName: "CreatorId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderCategoryHistories_UserId",
                table: "OrderCategoryHistories",
                newName: "IX_OrderCategoryHistories_CreatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderCategoryHistories_Users_CreatorId",
                table: "OrderCategoryHistories",
                column: "CreatorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderStatusHistories_Users_CreatorId",
                table: "OrderStatusHistories",
                column: "CreatorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderCategoryHistories_Users_CreatorId",
                table: "OrderCategoryHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderStatusHistories_Users_CreatorId",
                table: "OrderStatusHistories");

            migrationBuilder.RenameColumn(
                name: "CreatorId",
                table: "OrderStatusHistories",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderStatusHistories_CreatorId",
                table: "OrderStatusHistories",
                newName: "IX_OrderStatusHistories_UserId");

            migrationBuilder.RenameColumn(
                name: "CreatorId",
                table: "OrderCategoryHistories",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderCategoryHistories_CreatorId",
                table: "OrderCategoryHistories",
                newName: "IX_OrderCategoryHistories_UserId");

            migrationBuilder.AddColumn<int>(
                name: "ChangedBy",
                table: "OrderStatusHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ChangedBy",
                table: "OrderCategoryHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderCategoryHistories_Users_UserId",
                table: "OrderCategoryHistories",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderStatusHistories_Users_UserId",
                table: "OrderStatusHistories",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
