namespace BoxExpress.Application.Dtos;

public class ProductVariantDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string? ShopifyVariantId { get; set; }
    public decimal? Price { get; set; }
    public string? Sku { get; set; }
    public int ReservedQuantity { get; set; }
    public int AvailableQuantity { get; set; }
    public int? WarehouseInventoryId { get; set; }
    public List<InventoryMovementDto> InventoryMovements { get; set; } = new();
    public int PendingReturnQuantity { get; set; }
    public int? StoreId { get; set; }
    public int? QuantityDelivered { get; set; }
    public StoreDto? Store { get; set; }
    public string? ProductName { get; set; }
    public string? ProductSku { get; set; }
    public decimal? ProductPrice { get; set; }
    public string? WarehouseName { get; set; }
}
