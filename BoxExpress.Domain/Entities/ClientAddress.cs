using System.ComponentModel.DataAnnotations.Schema;

namespace BoxExpress.Domain.Entities;

public class ClientAddress : BaseEntity
{
    public int ClientId { get; set; }

    public string Address { get; set; } = string.Empty;
    public string? Address2 { get; set; } = string.Empty;
    public string? Complement { get; set; }
    public int? CityId { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public bool IsDefault { get; set; } = false;
    public Client Client { get; set; } = null!;
    public City? City { get; set; }
    public string? Zip { get; set; }
    public string? PostalCode { get; set; }
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
