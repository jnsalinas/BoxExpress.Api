using System.ComponentModel.DataAnnotations.Schema;

namespace BoxExpress.Domain.Entities;

public class ProductVariant : BaseEntity
{
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public string? Name { get; set; }
    public string? ShopifyVariantId { get; set; }
    public string? Sku { get; set; } = string.Empty;
    public decimal? Price { get; set; }

    [NotMapped]
    public int? AvailableUnits { get; set; }
    public ICollection<WarehouseInventory> WarehouseInventories { get; set; } = new List<WarehouseInventory>();
    public ICollection<InventoryMovement> InventoryMovements { get; set; } = new List<InventoryMovement>();
    
}