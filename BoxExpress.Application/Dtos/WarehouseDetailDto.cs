namespace BoxExpress.Application.Dtos;

public class WarehouseDetailDto : WarehouseDto
{
    public List<ProductInventoryDto> Products { get; set; } = new();
}
