
using BoxExpress.Domain.Enums;

namespace BoxExpress.Domain.Entities;

public class WarehouseInventoryTransfer : BaseEntity
{
    public int FromWarehouseId { get; set; }
    public Warehouse FromWarehouse { get; set; } = null!;
    public int ToWarehouseId { get; set; }
    public Warehouse ToWarehouse { get; set; } = null!;
    public ICollection<WarehouseInventoryTransferDetail> TransferDetails { get; set; } = new List<WarehouseInventoryTransferDetail>();
    public InventoryTransferStatus Status { get; set; } = InventoryTransferStatus.Pending;
    public int? AcceptedByUserId { get; set; }
    public User? AcceptedByUser { get; set; }
    public string? RejectionReason { get; set; }
    public int CreatorId { get; set; }
    public User Creator { get; set; } = null!;
}