using BoxExpress.Domain.Enums;

namespace BoxExpress.Domain.Filters;

public class InventoryMovementFilter : BaseFilter
{
    public int? WarehouseInventoryId { get; set; }
    public int? WarehouseId { get; set; }
    public int? OrderId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int? ProductVariantId { get; set; }
    public InventoryMovementType? MovementType { get; set; }

}