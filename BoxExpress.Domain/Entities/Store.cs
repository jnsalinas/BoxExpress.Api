namespace BoxExpress.Domain.Entities;

public class Store : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public int CountryId { get; set; }
    public Country Country { get; set; } = null!;
    public int CityId { get; set; }
    public City City { get; set; } = null!;
    public int WalletId { get; set; }
    public Wallet Wallet { get; set; } = null!;
    public string? ShopifyAccessToken { get; set; }
    public string? ShopifyShopDomain { get; set; }

}