namespace BoxExpress.Application.Dtos; 

public class CreateProductLoanDetailDto
{
    public int? Id { get; set; }
    public int ProductVariantId { get; set; }
    public int DeliveredQuantity { get; set; }
    public int ReturnedQuantity { get; set; }
    public int RequestedQuantity { get; set; }
    public string? Notes { get; set; }
} 