using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BoxExpress.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialWithSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var createdAt = new DateTime();
            // Banks
            migrationBuilder.InsertData(
                table: "Banks",
                columns: new[] { "Name", "CreatedAt" },
                values: new object[,]
                {
                    { "Bancolombia", createdAt },
                    { "Banco de Bogotá", createdAt },
                    { "Davivienda", createdAt },
                    { "Banco Popular", createdAt },
                    { "Banco AV Villas", createdAt },
                    { "Banco de Occidente", createdAt },
                    { "Banco Caja Social", createdAt },
                    { "BBVA Colombia", createdAt },
                    { "Scotiabank Colpatria", createdAt },
                    { "Banco Pichincha", createdAt },
                    { "BBVA México", createdAt },
                    { "Citibanamex", createdAt },
                    { "Santander México", createdAt },
                    { "Banorte", createdAt },
                    { "HSBC México", createdAt },
                    { "Scotiabank México", createdAt },
                    { "Banco Azteca", createdAt },
                    { "Inbursa", createdAt },
                    { "Bancoppel", createdAt },
                    { "Nequi", createdAt },
                    { "DaviPlata", createdAt }
                }
            );

            // Countries
            migrationBuilder.InsertData(
                table: "Countries",
                columns: new[] { "Name", "CreatedAt" },
                values: new object[,]
                {
                    { "Mexico", createdAt }
                }
            );

            // Cities (asumiendo CountryId = 1 para el país insertado arriba)
            migrationBuilder.InsertData(
                table: "Cities",
                columns: new[] { "Name", "CountryId", "CreatedAt" },
                values: new object[,]
                {
                    { "Ciudad de Mexico", 1, createdAt }
                }
            );

            // Currency
            migrationBuilder.InsertData(
                table: "Currency",
                columns: new[] { "Code", "Name", "Symbol", "CreatedAt" },
                values: new object[,]
                {
                    { "MXN", "Pesos mexicanos", "MXN", createdAt }
                }
            );

            // DocumentTypes
            migrationBuilder.InsertData(
                table: "DocumentTypes",
                columns: new[] { "Name", "CreatedAt" },
                values: new object[,]
                {
                    { "Cédula de Ciudadanía", createdAt },
                    { "Cédula de Extranjería", createdAt },
                    { "NIT", createdAt },
                    { "Pasaporte", createdAt },
                    { "INE", createdAt },
                    { "RFC", createdAt }
                }
            );

            // OrderCategories
            migrationBuilder.InsertData(
                table: "OrderCategories",
                columns: new[] { "Name", "CreatedAt" },
                values: new object[,]
                {
                    { "Express", createdAt },
                    { "Tradicional", createdAt }
                }
            );

            // OrderStatuses
            migrationBuilder.InsertData(
                table: "OrderStatuses",
                columns: new[] { "Name", "CreatedAt" },
                values: new object[,]
                {
                    { "Sin programar", createdAt },
                    { "Programado", createdAt },
                    { "En ruta", createdAt },
                    { "Entregado", createdAt },
                    { "Cancelado", createdAt }
                }
            );

            // Roles
            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Name", "CreatedAt" },
                values: new object[,]
                {
                    { "Admin", createdAt },
                    { "Tienda", createdAt },
                    { "Bodega", createdAt }
                }
            );

            // TimeSlots (ya tiene CreatedAt, solo ajusto la variable)
            migrationBuilder.InsertData(
                table: "TimeSlots",
                columns: new[] { "StartTime", "CreatedAt", "EndTime" },
                values: new object[,]
                {
                    { "07:00:00", createdAt, "06:00:00" },
                    { "08:00:00", createdAt, "07:00:00" },
                    { "09:00:00", createdAt, "08:00:00" },
                    { "10:00:00", createdAt, "09:00:00" },
                    { "11:00:00", createdAt, "10:00:00" },
                    { "12:00:00", createdAt, "11:00:00" },
                    { "13:00:00", createdAt, "12:00:00" },
                    { "14:00:00", createdAt, "13:00:00" },
                    { "15:00:00", createdAt, "14:00:00" },
                    { "16:00:00", createdAt, "15:00:00" },
                    { "17:00:00", createdAt, "16:00:00" },
                    { "18:00:00", createdAt, "17:00:00" },
                    { "19:00:00", createdAt, "18:00:00" },
                    { "20:00:00", createdAt, "19:00:00" }
                }
            );

            // TransactionTypes
            migrationBuilder.InsertData(
                table: "TransactionTypes",
                columns: new[] { "Name", "CreatedAt" },
                values: new object[,]
                {
                    { "Entrada", createdAt },
                    { "Salida", createdAt }
                }
            );

            // Users (asumiendo RoleId=1, CountryId=1, CityId=1, puedes ajustar según tus datos)
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Email", "PasswordHash", "FirstName", "LastName", "RoleId", "CountryId", "CityId", "CreatedAt" },
                values: new object[,]
                {
                    { "jnsalinasgo@gmail.com", "$2a$12$g5EYy0MMrXN5ZUBjE6SmJ.K5J4OdrLAklMgINhR4tpEoGKsPy4uK.", "Nicolas", "Salinas", 1, 1, 1, createdAt }
                }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
