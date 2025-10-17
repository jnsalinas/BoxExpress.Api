using BoxExpress.Domain.Filters;

namespace BoxExpress.Application.Dtos;

public class OrderFilterDto : BaseFilter
{
    public string? Name { get; set; }
    public int? CountryId { get; set; }
    public int? CityId { get; set; }
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
    public List<int>? ProductVariantIds { get; set; }
}
