namespace BoxExpress.Domain.Entities;

public class Product : BaseEntity
{
    public int StoreId { get; set; }
    public Store Store { get; set; } = null!;
    public string Name { get; set; } = string.Empty;
    public string? ShopifyProductId { get; set; }
}
