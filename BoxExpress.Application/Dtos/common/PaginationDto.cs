namespace BoxExpress.Application.Dtos.Common;
public class PaginationDto
{
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }

    public PaginationDto(int totalCount, int pageSize, int currentPage)
    {
        TotalCount = totalCount;
        PageSize = pageSize;
        CurrentPage = currentPage;
        TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
    }
}
