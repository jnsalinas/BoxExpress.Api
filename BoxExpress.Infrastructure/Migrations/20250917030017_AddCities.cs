using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BoxExpress.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "CityId",
                table: "Orders",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "CityId",
                table: "ClientAddress",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            var createdAt = DateTime.UtcNow;
            migrationBuilder.InsertData(
                table: "Cities",
                columns: new[] { "Name", "CountryId", "CreatedAt" },
                values: new object[,]
                {
                    { "San Pedro Tlaquepaque", 1, createdAt }
                }
            );

            migrationBuilder.InsertData(
                table: "Cities",
                columns: new[] { "Name", "CountryId", "CreatedAt" },
                values: new object[,]
                {
                    { "Zapopan", 1, createdAt }
                }
            );

            migrationBuilder.InsertData(
                table: "Cities",
                columns: new[] { "Name", "CountryId", "CreatedAt" },
                values: new object[,]
                {
                    { "Tlajomulco De Zúñiga", 1, createdAt }
                }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "CityId",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CityId",
                table: "ClientAddress",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
