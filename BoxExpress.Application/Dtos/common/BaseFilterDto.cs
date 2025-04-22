
namespace BoxExpress.Application.Dtos.Common;

public class BaseFilterDto
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public string? SortBy { get; set; }
    public string? SortDirection { get; set; }
}