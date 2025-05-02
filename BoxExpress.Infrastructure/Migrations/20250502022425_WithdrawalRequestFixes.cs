using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BoxExpress.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class WithdrawalRequestFixes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequestedAt",
                table: "WithdrawalRequests");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "WithdrawalRequests",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "CreatorId",
                table: "WithdrawalRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_WithdrawalRequests_CreatorId",
                table: "WithdrawalRequests",
                column: "CreatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_WithdrawalRequests_Users_CreatorId",
                table: "WithdrawalRequests",
                column: "CreatorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WithdrawalRequests_Users_CreatorId",
                table: "WithdrawalRequests");

            migrationBuilder.DropIndex(
                name: "IX_WithdrawalRequests_CreatorId",
                table: "WithdrawalRequests");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "WithdrawalRequests");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "WithdrawalRequests",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<DateTime>(
                name: "RequestedAt",
                table: "WithdrawalRequests",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
