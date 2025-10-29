namespace BoxExpress.Domain.Entities;

public class Country : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public ICollection<City> Cities { get; set; } = null!;
}