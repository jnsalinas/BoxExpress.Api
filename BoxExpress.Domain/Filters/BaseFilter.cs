namespace BoxExpress.Domain.Filters;

public class BaseFilter
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SortBy { get; set; }
    public bool IsAscending { get; set; } = true;
}