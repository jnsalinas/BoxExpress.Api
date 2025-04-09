namespace BoxExpress.Domain.Entities;

public class Store : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public int CountryId { get; set; }
    public Country Country { get; set; } = null!;
    public int CityId { get; set; }
    public City City { get; set; } = null!;
}