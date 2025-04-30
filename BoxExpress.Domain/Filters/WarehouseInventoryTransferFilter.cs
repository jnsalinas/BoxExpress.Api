namespace BoxExpress.Domain.Filters;

public class WarehouseInventoryTransferFilter : BaseFilter
{
    public string? Name { get; set; }
    public int? ToWarehouseId { get; set; }
    public int? FromWarehouseId { get; set; }
}