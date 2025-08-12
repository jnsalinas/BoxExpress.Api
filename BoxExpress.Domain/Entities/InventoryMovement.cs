using BoxExpress.Domain.Entities;
using BoxExpress.Domain.Enums;
namespace BoxExpress.Domain.Entities;

public class InventoryMovement : BaseEntity
{
    public int WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; } = null!;
    public int ProductVariantId { get; set; }
    public ProductVariant ProductVariant { get; set; } = null!;
    public InventoryMovementType MovementType { get; set; }
    public int Quantity { get; set; }
    public int? OrderId { get; set; }
    public Order? Order { get; set; }
    public int? TransferId { get; set; }
    public WarehouseInventoryTransfer? Transfer { get; set; }
    public string? Reference { get; set; }
    public string? Notes { get; set; }
    public int? CreatorId { get; set; }
    public User? Creator { get; set; }
    public int? ProductLoanDetailId { get; set; }
    public ProductLoanDetail? ProductLoanDetail { get; set; }
}
