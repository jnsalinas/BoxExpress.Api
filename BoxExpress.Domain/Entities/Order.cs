namespace BoxExpress.Domain.Entities;

public class Order : BaseEntity
{
    public int StoreId { get; set; }
    public Store Store { get; set; } = null!;
    public int CreatedBy { get; set; }
    public User Creator { get; set; } = null!;
    public int OrderStatusId { get; set; }
    public OrderStatus OrderStatus { get; set; } = null!;
    public int OrderCategoryId { get; set; }
    public OrderCategory OrderCategory { get; set; } = null!;
    public decimal DeliveryFee { get; set; }
    public string Currency { get; set; } = string.Empty;
    public int? WarehouseId { get; set; }
    public Warehouse? Warehouse { get; set; }
}