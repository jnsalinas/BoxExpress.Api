using System.Drawing;

namespace BoxExpress.Domain.Entities;

public class OrderSummary : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public int Count { get; set; }
}
