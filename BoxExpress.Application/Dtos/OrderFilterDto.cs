namespace BoxExpress.Application.Dtos;

public class OrderFilterDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? CountryId { get; set; }
    public int? CityId { get; set; }
    public int? CategoryId { get; set; }
}
