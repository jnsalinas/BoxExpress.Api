namespace BoxExpress.Application.Dtos;

public class OrderStatusHistoryDto
{
    public int? Id { get; set; }
    public string? Creator { get; set; } //todo mirar como le ponemos a todos los creadores
    public string? OldStatus { get; set; }
    public string? NewStatus { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? DeliveryProviderName { get; set; }
    public string? PhotoUrl { get; set; }
    public string? CourierName { get; set; }
}