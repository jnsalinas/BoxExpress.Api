namespace BoxExpress.Domain.Filters;

public class StoreFilter : BaseFilter
{
    public string? Name { get; set; }
    public int? StoreId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? ShopifyAccessToken { get; set; }
    public string? ShopifyShopDomain { get; set; }
    public Guid? PublicId { get; set; }
}