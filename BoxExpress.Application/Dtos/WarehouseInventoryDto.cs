using BoxExpress.Application.Dtos.Common;

namespace BoxExpress.Application.Dtos;

public class WarehouseInventoryDto : BaseFilterDto
{
    public int? Id { get; set; }
    public int? WarehouseId { get; set; }
    public int? Quantity { get; set; }
    public int? Status { get; set; }
    public string? Query { get; set; }
    public int ReservedQuantity { get; set; }
    public int PendingReturnQuantity { get; set; }
    public int AvailableQuantity { get; set; }
    public ProductVariantDto? ProductVariant { get; set; }
    public WarehouseDto? Warehouse { get; set; }

}
