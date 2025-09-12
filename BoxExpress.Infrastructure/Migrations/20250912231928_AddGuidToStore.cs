using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BoxExpress.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGuidToStore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShopifyAccessToken",
                table: "Stores");

            migrationBuilder.AddColumn<Guid>(
                name: "PublicId",
                table: "Stores",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PublicId",
                table: "Stores");

            migrationBuilder.AddColumn<string>(
                name: "ShopifyAccessToken",
                table: "Stores",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
