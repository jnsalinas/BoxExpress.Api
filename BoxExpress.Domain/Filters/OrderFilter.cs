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
    public DateTime? StartScheduledDate { get; set; }
    public DateTime? EndScheduledDate { get; set; }
    public int? WarehouseId { get; set; }
    public string? Query { get; set; }
    public int? StatusId { get; set; }
    public int? TimeSlotId { get; set; }
    public List<int>? CityIds { get; set; }
    public List<string>? Phones { get; set; }
    public List<int>? ProductVariantIds { get; set; }
}