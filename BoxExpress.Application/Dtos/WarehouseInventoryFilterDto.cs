using BoxExpress.Application.Dtos.Common;

namespace BoxExpress.Application.Dtos;

public class WarehouseInventoryFilterDto : BaseFilterDto
{
    public int? Id { get; set; }
    public int? WarehouseId { get; set; }
    public int? ProductVariantId { get; set; }
    public string? ProductVariantName { get; set; }
    public string? ProductVariantSku { get; set; }
    public int? Quantity { get; set; }
    public int? Status { get; set; }
    public string? Query { get; set; }   

}
