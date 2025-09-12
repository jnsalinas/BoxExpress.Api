namespace BoxExpress.Application.Dtos;

public class StoreDto
{
    public int Id { get; set; }
    public string? Name { get; set; } = string.Empty;
    public int? CountryId { get; set; }
    public string? Country { get; set; } = null!;
    public string? City { get; set; } = null!;
    public int? WalletId { get; set; }
    public decimal? PendingWithdrawals { get; set; }
    public decimal? Balance { get; set; }
    public decimal AvailableToWithdraw => (Balance ?? 0) - (PendingWithdrawals ?? 0);
    public string? ShopifyShopDomain { get; set; }
    public string? Username { get; set; }
    public decimal? DeliveryFee { get; set; }
    public Guid? PublicId { get; set; }
}