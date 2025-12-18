using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BoxExpress.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddChileanRegion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var createdAt = DateTime.UtcNow;

            migrationBuilder.InsertData(
                table: "Countries",
                columns: new[] { "Name", "CreatedAt", "Code" },
                values: new object[,]
                {
                    { "Chile", createdAt, "CL" }
                }
            );

            migrationBuilder.InsertData(
                table: "Currency",
                columns: new[] { "Code", "Name", "Symbol", "CountryId", "CreatedAt" },
                values: new object[,]
                {
                    { "CLP", "Peso chileno", "CLP", 4, createdAt }
                }
            );

            migrationBuilder.InsertData(
            table: "Cities",
            columns: new[] { "Name", "CountryId", "CreatedAt" },
            values: new object[,]
            {
                // Región Metropolitana
                { "Santiago", 4, createdAt },
                { "Puente Alto", 4, createdAt },
                { "Maipú", 4, createdAt },
                { "La Florida", 4, createdAt },
                { "Las Condes", 4, createdAt },
                { "San Bernardo", 4, createdAt },

                // Valparaíso
                { "Valparaíso", 4, createdAt },
                { "Viña del Mar", 4, createdAt },
                { "Quilpué", 4, createdAt },
                { "Villa Alemana", 4, createdAt },
                { "San Antonio", 4, createdAt },

                // Biobío
                { "Concepción", 4, createdAt },
                { "Talcahuano", 4, createdAt },
                { "Chiguayante", 4, createdAt },
                { "Los Ángeles", 4, createdAt },
                { "Coronel", 4, createdAt },

                // Antofagasta
                { "Antofagasta", 4, createdAt },
                { "Calama", 4, createdAt },
                { "Tocopilla", 4, createdAt },

                // La Araucanía
                { "Temuco", 4, createdAt },
                { "Padre Las Casas", 4, createdAt },
                { "Angol", 4, createdAt },

                // Coquimbo
                { "La Serena", 4, createdAt },
                { "Coquimbo", 4, createdAt },
                { "Ovalle", 4, createdAt },

                // Maule
                { "Talca", 4, createdAt },
                { "Curicó", 4, createdAt },
                { "Linares", 4, createdAt },

                // Ñuble
                { "Chillán", 4, createdAt },    
                { "Chillán Viejo", 4, createdAt },

                // Los Ríos
                { "Valdivia", 4, createdAt },

                // Los Lagos
                { "Puerto Montt", 4, createdAt },
                { "Osorno", 4, createdAt },
                { "Castro", 4, createdAt },
                { "Ancud", 4, createdAt },

                // Tarapacá
                { "Iquique", 4, createdAt },
                { "Alto Hospicio", 4, createdAt },

                // Arica y Parinacota
                { "Arica", 4, createdAt },

                // O’Higgins
                { "Rancagua", 4, createdAt },
                { "San Fernando", 4, createdAt },

                // Atacama
                { "Copiapó", 4, createdAt },
                { "Vallenar", 4, createdAt },

                // Magallanes
                { "Punta Arenas", 4, createdAt },

                // Aysén
                { "Coyhaique", 4, createdAt }
                }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
