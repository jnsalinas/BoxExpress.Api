
namespace BoxExpress.Domain.Entities;

public class OrderStatusHistory : BaseEntity
{
    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;
    public int ChangedBy { get; set; }
    public User User { get; set; } = null!;
    public int? OldStatusId { get; set; }
    public OrderStatus? OldStatus { get; set; }
    public int NewStatusId { get; set; }
    public OrderStatus NewStatus { get; set; } = null!;
}