namespace BoxExpress.Domain.Filters;

public class OrderStatusHistoryFilter : BaseFilter
{
    public int? OrderId { get; set; }
    public int? OldStatusId { get; set; }
    public int? NewStatusId { get; set; }
}