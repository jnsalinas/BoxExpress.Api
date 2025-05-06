using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BoxExpress.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class WithdrawalRequestAdditionalData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Bank",
                table: "WithdrawalRequests",
                newName: "ApprovedDescription");

            migrationBuilder.AddColumn<int>(
                name: "ApprovedByUserId",
                table: "WithdrawalRequests",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BankId",
                table: "WithdrawalRequests",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DocumentTypeId",
                table: "WithdrawalRequests",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Banks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Banks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DocumentTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WithdrawalRequests_ApprovedByUserId",
                table: "WithdrawalRequests",
                column: "ApprovedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_WithdrawalRequests_BankId",
                table: "WithdrawalRequests",
                column: "BankId");

            migrationBuilder.CreateIndex(
                name: "IX_WithdrawalRequests_DocumentTypeId",
                table: "WithdrawalRequests",
                column: "DocumentTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_WithdrawalRequests_Banks_BankId",
                table: "WithdrawalRequests",
                column: "BankId",
                principalTable: "Banks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WithdrawalRequests_DocumentTypes_DocumentTypeId",
                table: "WithdrawalRequests",
                column: "DocumentTypeId",
                principalTable: "DocumentTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WithdrawalRequests_Users_ApprovedByUserId",
                table: "WithdrawalRequests",
                column: "ApprovedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WithdrawalRequests_Banks_BankId",
                table: "WithdrawalRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_WithdrawalRequests_DocumentTypes_DocumentTypeId",
                table: "WithdrawalRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_WithdrawalRequests_Users_ApprovedByUserId",
                table: "WithdrawalRequests");

            migrationBuilder.DropTable(
                name: "Banks");

            migrationBuilder.DropTable(
                name: "DocumentTypes");

            migrationBuilder.DropIndex(
                name: "IX_WithdrawalRequests_ApprovedByUserId",
                table: "WithdrawalRequests");

            migrationBuilder.DropIndex(
                name: "IX_WithdrawalRequests_BankId",
                table: "WithdrawalRequests");

            migrationBuilder.DropIndex(
                name: "IX_WithdrawalRequests_DocumentTypeId",
                table: "WithdrawalRequests");

            migrationBuilder.DropColumn(
                name: "ApprovedByUserId",
                table: "WithdrawalRequests");

            migrationBuilder.DropColumn(
                name: "BankId",
                table: "WithdrawalRequests");

            migrationBuilder.DropColumn(
                name: "DocumentTypeId",
                table: "WithdrawalRequests");

            migrationBuilder.RenameColumn(
                name: "ApprovedDescription",
                table: "WithdrawalRequests",
                newName: "Bank");
        }
    }
}
