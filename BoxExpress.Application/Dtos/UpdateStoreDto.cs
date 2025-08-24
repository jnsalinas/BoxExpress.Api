namespace BoxExpress.Application.Dtos;

public class UpdateStoreDto
{
    public int Id { get; set; }
    public string? Name { get; set; } = string.Empty;
    public string? ShopifyShopDomain { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
}