namespace BoxExpress.Domain.Entities;

public class ProductVariant : BaseEntity
{
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public string? Name { get; set; }
    public string? ShopifyVariantId { get; set; }
}