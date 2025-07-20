namespace BoxExpress.Application.Dtos;

public class CreateWarehouseDto
{
    public required string Name { get; set; }
    public required int CityId { get; set; }
    public string? Manager { get; set; }
    public string? Address { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
}
