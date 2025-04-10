namespace BoxExpress.Application.Dtos;

public class CreateProductWithVariantsDto
{
    public string Name { get; set; } = string.Empty;
    public string? ShopifyProductId { get; set; }
    public List<CreateVariantDto> Variants { get; set; } = new();
}