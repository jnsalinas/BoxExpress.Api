using BoxExpress.Domain.Enums;

namespace BoxExpress.Domain.Filters;

public class InventoryHoldFilter : BaseFilter
{
    public int? ProductVariantId { get; set; }
    public int? WarehouseInventoryId { get; set; }
    public InventoryHoldStatus? Status { get; set; }
    public List<InventoryHoldStatus>? Statuses { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int? OrderId { get; set; }
    public int? WarehouseId { get; set; }


}