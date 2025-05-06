using BoxExpress.Domain.Entities;

namespace BoxExpress.Domain.Filters;

public class WarehouseInventoryTransferFilter : BaseFilter
{
    public int? ToWarehouseId { get; set; }
    public int? FromWarehouseId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public TransferStatus? StatusId { get; set; }

}