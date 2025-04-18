namespace BoxExpress.Domain.Entities;

public class WarehouseInventoryTransfer : BaseEntity
{
    public int FromWarehouseId { get; set; }
    public Warehouse FromWarehouse { get; set; } = null!;

    public int ToWarehouseId { get; set; }
    public Warehouse ToWarehouse { get; set; } = null!;

    public int ProductVariantId { get; set; }
    public ProductVariant ProductVariant { get; set; } = null!;

    public int Quantity { get; set; }
}
