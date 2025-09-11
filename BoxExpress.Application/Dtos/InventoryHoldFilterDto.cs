using BoxExpress.Application.Dtos.Common;
using BoxExpress.Domain.Enums;

namespace BoxExpress.Application.Dtos;

public class InventoryHoldFilterDto : BaseFilterDto
{
    public int? OrderId { get; set; }
    public int? ProductVariantId { get; set; }
    public int? WarehouseInventoryId { get; set; }
    public InventoryHoldStatus? Status { get; set; }
    public List<InventoryHoldStatus>? Statuses { get; set; }

    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int? WarehouseId { get; set; } 
    public int? ProductLoanDetailId { get; set; }
    public DateTime? CreatedAt { get; set; }
}