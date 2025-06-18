namespace BoxExpress.Domain.Filters;

public class WarehouseFilter
{
    public int? Id { get; set; }
    public string? Name { get; set; }
    public int? CityId { get; set; }
    public int? CountryId { get; set; }
}