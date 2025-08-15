using BoxExpress.Application.Dtos.Common;

namespace BoxExpress.Application.Dtos;

public class UpdateWarehouseInventoryDto
{
    public int? Id { get; set; }
    public string? ProductName { get; set; }
    public string? ProductSku { get; set; }
    public string? ShopifyProductId { get; set; }
    public string? ShopifyVariantId { get; set; }
    public string? VariantName { get; set; }
    public string? VariantSku { get; set; }
    public int? StoreId { get; set; }
    public int? Quantity { get; set; }
    public int? AddQuantity { get; set; }
    public string? Notes { get; set; }
    public decimal? Price { get; set; }
}