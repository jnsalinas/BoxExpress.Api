namespace BoxExpress.Application.Dtos;

public class OrderStatusHistoryDto
{
    public string? Creator { get; set; }
    public string? OldStatus { get; set; }
    public string? NewStatus { get; set; }
    public DateTime CreatedAt { get; set; }    

}