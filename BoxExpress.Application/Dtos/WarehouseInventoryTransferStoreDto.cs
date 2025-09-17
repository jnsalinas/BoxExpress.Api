using BoxExpress.Application.Dtos.Common;

namespace BoxExpress.Application.Dtos;

public class WarehouseInventoryTransferStoreDto : BaseDto
{
    public int ProductVariantId { get; set; }
    public int StoreId { get; set; }
    public int Quantity { get; set; }
}
