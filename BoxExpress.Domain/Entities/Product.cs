namespace BoxExpress.Domain.Entities;

public class Product : BaseEntity
{
    public int? StoreId { get; set; }
    public Store? Store { get; set; } = null!;
    public string Name { get; set; } = string.Empty;
    public string? ShopifyProductId { get; set; }
    public string? Sku { get; set; } = string.Empty;
    public decimal? Price { get; set; }
    public ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();

}
