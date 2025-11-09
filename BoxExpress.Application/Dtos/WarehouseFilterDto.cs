using BoxExpress.Application.Dtos.Common;

namespace BoxExpress.Application.Dtos;

public class WarehouseFilterDto : BaseFilterDto
{
    public int? Id { get; set; }
    public string? Name { get; set; }
}
