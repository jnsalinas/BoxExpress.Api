namespace BoxExpress.Application.Dtos;

public class WarehouseDetailDto : WarehouseDto
{
    public List<ProductDto> Products { get; set; } = new();
}
