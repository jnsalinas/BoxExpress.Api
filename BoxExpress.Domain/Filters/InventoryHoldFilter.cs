using BoxExpress.Domain.Enums;

namespace BoxExpress.Domain.Filters;

public class InventoryHoldFilter : BaseFilter
{
    public int? ProductVariantId { get; set; }
    public int? WarehouseInventoryId { get; set; }

}