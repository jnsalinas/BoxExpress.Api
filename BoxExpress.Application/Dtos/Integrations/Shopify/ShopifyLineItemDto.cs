namespace BoxExpress.Application.Integrations.Shopify;

public class ShopifyLineItemDto
{
    public long Id { get; set; }
    public string? Title { get; set; }
    public int Quantity { get; set; }
    public string? Price { get; set; }
    public string? Sku { get; set; }
    public string? Variant_Title { get; set; }
    public string? Total_Discount { get; set; }
    public long? Product_Id { get; set; }
    public int? Variant_Id { get; set; }
}
