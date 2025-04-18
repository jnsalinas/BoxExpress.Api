namespace BoxExpress.Application.Dtos;

public class WarehouseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int CountryId { get; set; }
    public int CityId { get; set; }
    public string? CountryName { get; set; }
    public string? CityName { get; set; }
    public string? Manager { get; set; }
    public string? Address { get; set; }
}
