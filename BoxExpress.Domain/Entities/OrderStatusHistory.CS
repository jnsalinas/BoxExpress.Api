
namespace BoxExpress.Domain.Entities;

public class OrderStatusHistory : BaseEntity
{
    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;
    public int? CreatorId { get; set; }
    public User? Creator { get; set; } = null!;
    public int? OldStatusId { get; set; }
    public OrderStatus OldStatus { get; set; } = null!;
    public int NewStatusId { get; set; }
    public OrderStatus NewStatus { get; set; } = null!;
}