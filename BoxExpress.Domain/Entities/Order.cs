namespace BoxExpress.Domain.Entities;

public class Order : BaseEntity
{
    public string Customer { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public string Description { get; set; }
}
