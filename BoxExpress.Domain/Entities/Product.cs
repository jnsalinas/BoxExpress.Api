namespace BoxExpress.Domain.Entities;

public class Product : BaseEntity
{
    public int StoreId { get; set; } //todo validar si el product pertenece a una tienda
    public Store Store { get; set; } = null!;
    public string Name { get; set; } = string.Empty;
    public string? ShopifyProductId { get; set; }
    public string? Sku { get; set; } = string.Empty;
    public decimal? Price { get; set; }

}
