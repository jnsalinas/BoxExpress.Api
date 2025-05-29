using System.Drawing;

namespace BoxExpress.Domain.Entities;

public class OrderSummary : BaseEntity
{
    public int StatusId { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public int Count { get; set; }
}
