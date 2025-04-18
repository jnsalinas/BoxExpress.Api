namespace BoxExpress.Application.Dtos;

public class WarehouseInventoryTransferDto
{
    public int FromWarehouseId { get; set; }
    public int ToWarehouseId { get; set; }
    public int ProductVariantId { get; set; }
    public int Quantity { get; set; }
}
