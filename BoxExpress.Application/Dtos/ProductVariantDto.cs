namespace BoxExpress.Application.Dtos;

public class ProductVariantDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string? ShopifyId { get; internal set; }
    public decimal? Price { get; set; }
    public string? Sku { get; set; }
    public int ReservedQuantity { get; set; }
    public int AvailableQuantity { get; set; }
}
