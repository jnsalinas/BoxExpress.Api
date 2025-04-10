namespace BoxExpress.Application.Dtos;

public class VariantInventoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string? ShopifyId { get; internal set; }
}
