namespace BoxExpress.Application.Dtos;

public class CreateVariantDto
{
    public string Name { get; set; } = string.Empty;
    public string? ShopifyVariantId { get; set; }
    public int Quantity { get; set; }
}
