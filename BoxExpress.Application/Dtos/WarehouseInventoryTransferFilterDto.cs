using BoxExpress.Domain.Filters;

namespace BoxExpress.Application.Dtos;

public class WarehouseInventoryTransferFilterDto : BaseFilter
{
    public int? FromWarehouseId { get; set; }
    public int? ToWarehouseId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int? StatusId { get; set; }
}
