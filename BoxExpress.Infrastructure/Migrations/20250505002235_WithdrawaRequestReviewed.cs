using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BoxExpress.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class WithdrawaRequestReviewed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WithdrawalRequests_Users_ApprovedByUserId",
                table: "WithdrawalRequests");

            migrationBuilder.RenameColumn(
                name: "ApprovedDescription",
                table: "WithdrawalRequests",
                newName: "Reason");

            migrationBuilder.RenameColumn(
                name: "ApprovedByUserId",
                table: "WithdrawalRequests",
                newName: "ReviewedByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_WithdrawalRequests_ApprovedByUserId",
                table: "WithdrawalRequests",
                newName: "IX_WithdrawalRequests_ReviewedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_WithdrawalRequests_Users_ReviewedByUserId",
                table: "WithdrawalRequests",
                column: "ReviewedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WithdrawalRequests_Users_ReviewedByUserId",
                table: "WithdrawalRequests");

            migrationBuilder.RenameColumn(
                name: "ReviewedByUserId",
                table: "WithdrawalRequests",
                newName: "ApprovedByUserId");

            migrationBuilder.RenameColumn(
                name: "Reason",
                table: "WithdrawalRequests",
                newName: "ApprovedDescription");

            migrationBuilder.RenameIndex(
                name: "IX_WithdrawalRequests_ReviewedByUserId",
                table: "WithdrawalRequests",
                newName: "IX_WithdrawalRequests_ApprovedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_WithdrawalRequests_Users_ApprovedByUserId",
                table: "WithdrawalRequests",
                column: "ApprovedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
