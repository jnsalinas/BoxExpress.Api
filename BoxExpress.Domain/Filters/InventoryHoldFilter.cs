using BoxExpress.Domain.Enums;

namespace BoxExpress.Domain.Filters;

public class InventoryHoldFilter : BaseFilter
{
    public string? Query { get; set; }
    public int? ProductVariantId { get; set; }
    public int? WarehouseInventoryId { get; set; }
    public InventoryHoldStatus? Status { get; set; }
    public List<InventoryHoldStatus>? Statuses { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int? OrderId { get; set; }
    public int? WarehouseId { get; set; }
    public int? ProductLoanDetailId { get; set; }
    public int? ProductLoanId { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? EndCreatedAt { get; set; }
    public int? DeliveryProviderId { get; set; }
    public string? CourierName { get; set; }
}