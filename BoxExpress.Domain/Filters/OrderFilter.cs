// 💡 Principios básicos:
// Domain: solo conoce entidades y lógica de negocio pura.

// Application: usa DTOs para comunicar datos entre capas (como entre API y Services).

// Infrastructure: implementa los contratos de Domain, sin introducir lógica de negocio.

// API: es la capa externa que consume Application.

// se usa para no romper la inversión de dependencias.

//Aislamiento de capas

// “Duplication is better than the wrong abstraction.”
// — Sandi Metz
namespace BoxExpress.Domain.Filters;

public class OrderFilter
{
    public string? Name { get; set; }
    public int? CityId { get; set; }
    public int? CountryId { get; set; }
    public int? CategoryId { get; set; }
}