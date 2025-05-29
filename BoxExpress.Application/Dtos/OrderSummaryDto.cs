using BoxExpress.Domain.Filters;

namespace BoxExpress.Application.Dtos;

public class OrderSummaryDto : BaseFilter
{
    public int StatusId { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public int Count { get; set; }
}
