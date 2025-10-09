using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BoxExpress.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNewStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var createdAt = DateTime.UtcNow;
            migrationBuilder.InsertData(
               table: "OrderCategories",
               columns: new[] { "Name", "CreatedAt" },
               values: new object[,]
               {
                    { "Sin cobertura", createdAt },
                    { "Orden repetida", createdAt },
               }
           );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
               table: "OrderCategories",
               keyColumn: "Name",
               keyValue: "Sin cobertura"
            );
            migrationBuilder.DeleteData(
               table: "OrderCategories",
               keyColumn: "Name",
               keyValue: "Orden repetida"
            );
        }
    }
}
