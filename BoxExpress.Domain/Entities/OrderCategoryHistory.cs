namespace BoxExpress.Domain.Entities;

public class OrderCategoryHistory : BaseEntity
{
    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;
    public int CreatorId { get; set; }
    public User Creator { get; set; } = null!;
    public int? OldCategoryId { get; set; }
    public OrderCategory OldCategory { get; set; } = null!;
    public int NewCategoryId { get; set; }
    public OrderCategory NewCategory { get; set; } = null!;
}