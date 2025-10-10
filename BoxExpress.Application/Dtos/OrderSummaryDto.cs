using BoxExpress.Domain.Filters;

namespace BoxExpress.Application.Dtos;

public class OrderSummaryDto : BaseFilter
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Count { get; set; }
}
