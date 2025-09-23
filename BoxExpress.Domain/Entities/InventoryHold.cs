using BoxExpress.Domain.Enums;

namespace BoxExpress.Domain.Entities;

public class InventoryHold : BaseEntity
{
    public int WarehouseInventoryId { get; set; }
    public WarehouseInventory WarehouseInventory { get; set; } = null!;
    public int Quantity { get; set; }
    public int? OrderItemId { get; set; }
    public OrderItem? OrderItem { get; set; }
    public int? WarehouseInventoryTransferDetailId { get; set; }
    public WarehouseInventoryTransferDetail? WarehouseInventoryTransferDetail { get; set; } = null!;
    public InventoryHoldType Type { get; set; }
    public InventoryHoldStatus Status { get; set; }
    public int CreatorId { get; set; }
    public User Creator { get; set; } = null!;
    public DateTime? ResolvedAt { get; set; }
    public string? Notes { get; set; }
    public int? ProductLoanDetailId { get; set; }
    public ProductLoanDetail? ProductLoanDetail { get; set; } = null!;
    public int? OrderStatusHistoryId { get; set; }
    public OrderStatusHistory? OrderStatusHistory { get; set; }
    public string? OnRouteEvidenceUrl { get; set; }
}

