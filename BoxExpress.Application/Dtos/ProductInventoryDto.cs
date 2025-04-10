namespace BoxExpress.Application.Dtos;

public class ProductInventoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<VariantInventoryDto> Variants { get; set; } = new();
    public string? ShopifyId { get; set; }
}
