namespace BoxExpress.Application.Dtos;

public class CreateVariantDto
{
    public int? Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ShopifyId { get; set; }
    public int Quantity { get; set; }
    public int? StoreId { get; set; }
    public string? Sku { get; set; }
    public decimal? Price { get; set; }
}
