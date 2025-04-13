namespace BoxExpress.Domain.Entities;

public class Client : BaseEntity
{
   
    public string FullName { get; set; } = string.Empty;
    public string Document { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public ICollection<ClientAddress> Addresses { get; set; } = new List<ClientAddress>();
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}