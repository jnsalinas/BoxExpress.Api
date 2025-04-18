
namespace BoxExpress.Domain.Entities;

public class Warehouse : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public int CountryId { get; set; }
    public Country Country { get; set; } = null!;
    public int CityId { get; set; }
    public City City { get; set; } = null!;
    public string? Manager { get; set; }
    public string? Address { get; set; }
    public ICollection<WarehouseInventory> Inventories { get; set; } = new List<WarehouseInventory>();
}