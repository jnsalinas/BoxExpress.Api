namespace BoxExpress.Application.Dtos;

public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ShopifyId { get; set; }
    public decimal? Price { get; set; }
    public string? Sku { get; set; }
    public List<ProductVariantDto> Variants { get; set; } = new();
}
