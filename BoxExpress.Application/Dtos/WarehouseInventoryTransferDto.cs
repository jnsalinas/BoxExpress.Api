using BoxExpress.Application.Dtos.Common;

namespace BoxExpress.Application.Dtos;

public class WarehouseInventoryTransferDto : BaseDto
{
    public string? ToWarehouse { get; set; }
    public string? FromWarehouse { get; set; }
    public int FromWarehouseId { get; set; }
    public int ToWarehouseId { get; set; }
    public string? Creator { get; set; }
    public int? Status { get; set; }
    public string? StatusName { get; set; }
    public string? AcceptedBy { get; set; }
    public string? RejectionReason { get; set; }


    public List<WarehouseInventoryTransferDetailDto>? TransferDetails { get; set; }
}
