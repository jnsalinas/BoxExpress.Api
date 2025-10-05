namespace BoxExpress.Domain.Entities;

public class OrderStatusCountResult
{
    public int OrderId { get; set; }
    public int NewStatusId { get; set; }
    public int Count { get; set; }
}