namespace BoxExpress.Domain.Filters;

public class OrderStatusHistoryFilter : BaseFilter
{
    public int? OrderId { get; set; }
    public int? OldStatusId { get; set; }
    public int? NewStatusId { get; set; }
    public List<int>? NewStatusesId { get; set; }
    public List<int>? OrderIds { get; set; }
}