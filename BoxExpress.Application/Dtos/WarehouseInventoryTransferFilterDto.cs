using BoxExpress.Domain.Filters;

namespace BoxExpress.Application.Dtos;

public class WarehouseInventoryTransferFilterDto: BaseFilter
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? FromWarehouseId { get; set; }
    public int? ToWarehouseId { get; set; }
}
