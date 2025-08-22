using BoxExpress.Application.Dtos.Common;

namespace BoxExpress.Application.Dtos;

public class StoreFilterDto : BaseFilterDto
{
    public string? Name { get; set; }
    public int? StoreId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? ShopifyShopDomain { get; set; }
}