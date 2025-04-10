namespace BoxExpress.Application.Dtos;

public class WarehouseDetailDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? CountryName { get; set; }
    public string? CityName { get; set; }
    public List<ProductInventoryDto> Products { get; set; } = new();
}
