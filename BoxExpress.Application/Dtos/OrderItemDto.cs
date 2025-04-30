namespace BoxExpress.Application.Dtos;

public class OrderItemDto
{
    public int ProductVariantId { get; set; }
    public string? ProductName  { get; set; }
    public string? ProductVariantName  { get; set; }
    public int? Quantity { get; set; }
    public decimal? UnitPrice { get; set; }
}