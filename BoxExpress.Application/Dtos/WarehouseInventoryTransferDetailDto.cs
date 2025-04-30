using BoxExpress.Application.Dtos.Common;

namespace BoxExpress.Application.Dtos;

public class WarehouseInventoryTransferDetailDto : BaseDto
{
    public string? Product { get; set; }
    public string? ProductVariant { get; set; }
    public int ProductVariantId { get; set; }
    public int Quantity { get; set; }
}
