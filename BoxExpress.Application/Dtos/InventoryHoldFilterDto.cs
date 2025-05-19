using BoxExpress.Application.Dtos.Common;

namespace BoxExpress.Application.Dtos;

public class InventoryHoldFilterDto : BaseFilterDto
{
    public int? ProductVariantId { get; set; }
    public int? WarehouseInventoryId { get; set; }
}