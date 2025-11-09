using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BoxExpress.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixCurrencyAndAddCountries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CountryId",
                table: "Currency",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Currency_CountryId",
                table: "Currency",
                column: "CountryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Currency_Countries_CountryId",
                table: "Currency",
                column: "CountryId",
                principalTable: "Countries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);


            var createdAt = DateTime.UtcNow;
            migrationBuilder.InsertData(
                table: "Countries",
                columns: new[] { "Name", "CreatedAt" },
                values: new object[,]
                {
                    { "Colombia", createdAt }
                }
            );

            migrationBuilder.InsertData(
                table: "Countries",
                columns: new[] { "Name", "CreatedAt" },
                values: new object[,]
                {
                    { "Paraguay", createdAt }
                }
            );

            // Actualizar moneda existente MXN (Id = 1) para enlazarla a México (CountryId = 1)
            migrationBuilder.UpdateData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CountryId", "CreatedAt" },
                values: new object[] { 1, createdAt }
            );

            // Insertar monedas para Colombia (2) y Paraguay (3)
            migrationBuilder.InsertData(
                table: "Currency",
                columns: new[] { "Code", "Name", "Symbol", "CountryId", "CreatedAt" },
                values: new object[,]
                {
                    { "COP", "Pesos colombianos", "COP", 2, createdAt },
                    { "PYG", "Guaraníes paraguayos", "PYG", 3, createdAt }
                }
            );
            // Ciudades MX (CountryId = 1), CO (2), PY (3)
            migrationBuilder.InsertData(
                table: "Cities",
                columns: new[] { "Name", "CountryId", "CreatedAt" },
                values: new object[,]
                {
                    // México
                    // omitimos Ciudad de México para evitar duplicado con dato existente
                    { "Ecatepec", 1, createdAt },
                    { "Guadalajara", 1, createdAt },
                    { "Puebla", 1, createdAt },
                    { "Juárez", 1, createdAt },
                    { "Tijuana", 1, createdAt },
                    { "León", 1, createdAt },
                    // omitimos Zapopan para evitar duplicado con dato existente
                    { "Monterrey", 1, createdAt },
                    { "Nezahualcóyotl", 1, createdAt },
                    { "Mérida", 1, createdAt },
                    { "San Luis Potosí", 1, createdAt },
                    { "Querétaro", 1, createdAt },
                    { "Aguascalientes", 1, createdAt },
                    { "Hermosillo", 1, createdAt },
                    { "Saltillo", 1, createdAt },
                    { "Mexicali", 1, createdAt },
                    { "Culiacán", 1, createdAt },
                    { "Chihuahua", 1, createdAt },
                    { "Morelia", 1, createdAt },
                    { "Cancún", 1, createdAt },
                    { "Toluca", 1, createdAt },
                    { "Reynosa", 1, createdAt },
                    { "Torreón", 1, createdAt },
                    { "Naucalpan", 1, createdAt },
                    { "Acapulco", 1, createdAt },
                    { "Irapuato", 1, createdAt },
                    { "San Pedro Tlaquepaque", 1, createdAt },
                    { "San Nicolás de los Garza", 1, createdAt },
                    { "Durango", 1, createdAt },
                    { "Xalapa", 1, createdAt },
                    { "Celaya", 1, createdAt },
                    { "Ciudad Victoria", 1, createdAt },
                    { "Villahermosa", 1, createdAt },
                    { "Matamoros", 1, createdAt },
                    { "Veracruz", 1, createdAt },
                    { "Córdoba", 1, createdAt },
                    { "Orizaba", 1, createdAt },
                    { "Coatzacoalcos", 1, createdAt },
                    { "Tampico", 1, createdAt },
                    { "Ensenada", 1, createdAt },
                    { "Tepic", 1, createdAt },
                    { "La Paz", 1, createdAt },
                    { "Puerto Vallarta", 1, createdAt },
                    { "Playa del Carmen", 1, createdAt },
                    { "Campeche", 1, createdAt },
                    { "Tuxtla Gutiérrez", 1, createdAt },
                    { "Oaxaca", 1, createdAt },
                    { "Pachuca", 1, createdAt },
                    { "Cuernavaca", 1, createdAt },
                    { "Tlajomulco de Zúñiga", 1, createdAt },

                    // Colombia
                    { "Bogotá", 2, createdAt },
                    { "Medellín", 2, createdAt },
                    { "Cali", 2, createdAt },
                    { "Barranquilla", 2, createdAt },
                    { "Cartagena", 2, createdAt },
                    { "Cúcuta", 2, createdAt },
                    { "Bucaramanga", 2, createdAt },
                    { "Soledad", 2, createdAt },
                    { "Ibagué", 2, createdAt },
                    { "Soacha", 2, createdAt },
                    { "Santa Marta", 2, createdAt },
                    { "Villavicencio", 2, createdAt },
                    { "Bello", 2, createdAt },
                    { "Valledupar", 2, createdAt },
                    { "Montería", 2, createdAt },
                    { "Pasto", 2, createdAt },
                    { "Manizales", 2, createdAt },
                    { "Pereira", 2, createdAt },
                    { "Neiva", 2, createdAt },
                    { "Armenia", 2, createdAt },
                    { "Popayán", 2, createdAt },
                    { "Sincelejo", 2, createdAt },
                    { "Floridablanca", 2, createdAt },
                    { "Itagüí", 2, createdAt },
                    { "Envigado", 2, createdAt },
                    { "Dosquebradas", 2, createdAt },
                    { "Tuluá", 2, createdAt },
                    { "Tunja", 2, createdAt },
                    { "Riohacha", 2, createdAt },
                    { "Quibdó", 2, createdAt },
                    { "Yopal", 2, createdAt },
                    { "Barrancabermeja", 2, createdAt },
                    { "Girardot", 2, createdAt },
                    { "Zipaquirá", 2, createdAt },
                    { "Chía", 2, createdAt },
                    { "Funza", 2, createdAt },
                    { "Mosquera", 2, createdAt },

                    // Paraguay
                    { "Asunción", 3, createdAt },
                    { "Ciudad del Este", 3, createdAt },
                    { "San Lorenzo", 3, createdAt },
                    { "Luque", 3, createdAt },
                    { "Capiatá", 3, createdAt },
                    { "Lambaré", 3, createdAt },
                    { "Fernando de la Mora", 3, createdAt },
                    { "Limpio", 3, createdAt },
                    { "Ñemby", 3, createdAt },
                    { "Encarnación", 3, createdAt },
                    { "Pedro Juan Caballero", 3, createdAt },
                    { "Coronel Oviedo", 3, createdAt },
                    { "Villarrica", 3, createdAt },
                    { "Caacupé", 3, createdAt },
                    { "Itauguá", 3, createdAt },
                    { "Mariano Roque Alonso", 3, createdAt },
                    { "Caaguazú", 3, createdAt },
                    { "Concepción", 3, createdAt }
                }
            );

            
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Currency_Countries_CountryId",
                table: "Currency");

            migrationBuilder.DropIndex(
                name: "IX_Currency_CountryId",
                table: "Currency");

            migrationBuilder.DropColumn(
                name: "CountryId",
                table: "Currency");
        }
    }
}
