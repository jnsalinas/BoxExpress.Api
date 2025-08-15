namespace BoxExpress.Application.Dtos;

public class OrderDto
{
    public int Id { get; set; }
    public string? Country { get; set; }
    public string? City { get; set; }
    public int? TimeSlotId { get; set; }
    public TimeSpan? TimeSlotStartTime { get; set; }
    public TimeSpan? TimeSlotEndTime { get; set; }
    public DateTime? DeliveryDate { get; set; }
    public DateTime? RescheduleDate { get; set; }
    public string? Contains { get; set; }

    public string? ClientAddress { get; set; }
    public string? ClientAddressComplement { get; set; }
    public string? ClientAddressPostalCode { get; set; }
    public string? ClientFullName { get; set; }
    public string? ClientDocument { get; set; }
    public string? ClientPhone { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime? ScheduledDate { get; set; }
    public string? Status { get; set; }
    public int? StatusId { get; set; }
    public string? Category { get; set; }
    public int? CategoryId { get; set; }
    public string? StoreName { get; set; }
    public string? Notes { get; set; }
    public string? Code { get; set; }
    public decimal? DeliveryFee { get; set; }
    public int? WarehouseId { get; set; }
    public string? WarehouseName { get; set; }
    public string? CurrencyCode { get; set; }
    public List<OrderItemDto>? OrderItems { get; set; }
    public int? StoreId { get; internal set; }
    public DateTime? CreatedAt { get; set; }
    public ClientDto? Client { get; set; }
}
