using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BoxExpress.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDocumentTypeIdToClient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DocumentTypeId",
                table: "Client",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Client_DocumentTypeId",
                table: "Client",
                column: "DocumentTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Client_DocumentTypes_DocumentTypeId",
                table: "Client",
                column: "DocumentTypeId",
                principalTable: "DocumentTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Client_DocumentTypes_DocumentTypeId",
                table: "Client");

            migrationBuilder.DropIndex(
                name: "IX_Client_DocumentTypeId",
                table: "Client");

            migrationBuilder.DropColumn(
                name: "DocumentTypeId",
                table: "Client");
        }
    }
}
