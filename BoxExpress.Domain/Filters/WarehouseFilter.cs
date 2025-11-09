namespace BoxExpress.Domain.Filters;

public class WarehouseFilter : BaseFilter
{
    public int? Id { get; set; }
    public string? Name { get; set; }
    public int? CityId { get; set; }
}