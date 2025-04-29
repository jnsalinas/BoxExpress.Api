namespace BoxExpress.Domain.Entities;

public class WarehouseInventoryTransferDetail : BaseEntity
{
    public int WarehouseInventoryTransferId { get; set; }
    public WarehouseInventoryTransfer WarehouseInventoryTransfer { get; set; } = null!;

    public int ProductVariantId { get; set; }
    public ProductVariant ProductVariant { get; set; } = null!;

    public int Quantity { get; set; }
}

