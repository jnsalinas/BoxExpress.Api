using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BoxExpress.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProductLoanDetailHold : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductLoanDetailId",
                table: "InventoryHolds",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventoryHolds_ProductLoanDetailId",
                table: "InventoryHolds",
                column: "ProductLoanDetailId");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryHolds_ProductLoanDetails_ProductLoanDetailId",
                table: "InventoryHolds",
                column: "ProductLoanDetailId",
                principalTable: "ProductLoanDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryHolds_ProductLoanDetails_ProductLoanDetailId",
                table: "InventoryHolds");

            migrationBuilder.DropIndex(
                name: "IX_InventoryHolds_ProductLoanDetailId",
                table: "InventoryHolds");

            migrationBuilder.DropColumn(
                name: "ProductLoanDetailId",
                table: "InventoryHolds");
        }
    }
}
