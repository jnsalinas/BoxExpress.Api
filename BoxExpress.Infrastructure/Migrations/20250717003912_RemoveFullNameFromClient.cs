using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BoxExpress.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveFullNameFromClient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FullName",
                table: "Client");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "Client",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
