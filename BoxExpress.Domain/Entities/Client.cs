using System.ComponentModel.DataAnnotations.Schema;

namespace BoxExpress.Domain.Entities;

public class Client : BaseEntity
{
    public string FullName { get; set; } = string.Empty; //todo: eliminar
    public string Document { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? ExternalId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public ICollection<ClientAddress> Addresses { get; set; } = new List<ClientAddress>();
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}