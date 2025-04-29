namespace BoxExpress.Application.Dtos;

public class WarehouseInventoryTransferDto
{
    public int FromWarehouseId { get; set; }
    public int ToWarehouseId { get; set; }
    public List<WarehouseInventoryTransferDetailDto>? TransferDetails { get; set; }
}
