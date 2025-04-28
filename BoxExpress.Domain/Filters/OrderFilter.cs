namespace BoxExpress.Domain.Filters;

public class OrderFilter : BaseFilter
{
    public string? Name { get; set; }
    public int? CityId { get; set; }
    public int? CountryId { get; set; }
    public int? CategoryId { get; set; }
    public int? OrderId { get; set; }
    public int? StoreId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}