using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BoxExpress.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPostalCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PostalCode",
                table: "ClientAddress",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PostalCode",
                table: "ClientAddress");
        }
    }
}
