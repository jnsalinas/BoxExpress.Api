namespace BoxExpress.Application.Integrations.Shopify;

public class ShopifyShippingAddressDto
{
    public string? First_Name { get; set; }
    public string? Last_Name { get; set; }
    public string? Address1 { get; set; }
    public string? Address2 { get; set; }
    public string? City { get; set; }
    public string? Province { get; set; }
    public string? Country { get; set; }
    public string? Zip { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? Phone { get; set; }
}