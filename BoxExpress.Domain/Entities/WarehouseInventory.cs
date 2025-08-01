namespace BoxExpress.Domain.Entities;

public class WarehouseInventory : BaseEntity
{
    public int WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; } = null!;
    public int ProductVariantId { get; set; }
    public ProductVariant ProductVariant { get; set; } = null!;
    public int? StoreId { get; set; }
    public Store? Store { get; set; }
    public int Quantity { get; set; }
    public int ReservedQuantity { get; set; }
    public int PendingReturnQuantity { get; set; }
    public int? QuantityDelivered { get; set; }
    public int AvailableQuantity => Quantity - ReservedQuantity - PendingReturnQuantity;
}