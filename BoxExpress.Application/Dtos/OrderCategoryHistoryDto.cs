namespace BoxExpress.Application.Dtos;

public class OrderCategoryHistoryDto
{
    public string? Creator { get; set; }
    public string? OldCategory { get; set; }
    public string? NewCategory { get; set; }
    public DateTime CreatedAt { get; set; }    
}