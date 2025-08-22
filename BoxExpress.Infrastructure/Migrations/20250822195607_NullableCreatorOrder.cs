using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BoxExpress.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NullableCreatorOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Orders");

            migrationBuilder.AlterColumn<int>(
                name: "CreatorId",
                table: "Orders",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "CreatorId",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                table: "Orders",
                type: "int",
                nullable: true);
        }
    }
}
