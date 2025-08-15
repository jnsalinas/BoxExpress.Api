using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BoxExpress.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InsertNewProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var createdAt = DateTime.UtcNow;

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Name", "CreatedAt" },
                values: new object[,]
                {
                    { "Supervisor", createdAt },
                }
            );

            // Users (asumiendo RoleId=1, CountryId=1, CityId=1, puedes ajustar según tus datos)
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Email", "PasswordHash", "FirstName", "LastName", "RoleId", "CountryId", "CityId", "CreatedAt" },
                values: new object[,]
                {
                    { "supervisor@boxexpress.com", "$2a$12$g5EYy0MMrXN5ZUBjE6SmJ.K5J4OdrLAklMgINhR4tpEoGKsPy4uK.", "Nicolas", "Salinas", 4, 1, 1, createdAt }
                }
            );

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
