using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BoxExpress.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NewCity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var createdAt = new DateTime();

            migrationBuilder.InsertData(
                           table: "Cities",
                           columns: new[] { "Name", "CountryId", "CreatedAt" },
                           values: new object[,]
                           {
                    { "Estado de Mexico", 1, createdAt }
                           }
                       );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
