namespace BoxExpress.Application.Dtos;

public class CreateProductWithVariantsDto
{
    public int? Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ShopifyProductId { get; set; }
    public string? Sku { get; set; }
    public decimal? Price { get; set; }
    public int? Quantity { get; set; }
    public List<CreateVariantDto> Variants { get; set; } = new();
}