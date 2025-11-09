namespace BoxExpress.Domain.Entities;

public class Currency : BaseEntity
{
    public string Code { get; set; } = string.Empty; // Ej: USD, COP, EUR
    public string Name { get; set; } = string.Empty; // Ej: Dólar estadounidense
    public string Symbol { get; set; } = string.Empty; // Ej: Dólar estadounidense
    public int? CountryId { get; set; }
    public Country? Country { get; set; }
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
