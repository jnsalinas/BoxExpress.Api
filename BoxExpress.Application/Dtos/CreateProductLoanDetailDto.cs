namespace BoxExpress.Application.Dtos; 

public class CreateProductLoanDetailDto
{
    public int ProductVariantId { get; set; }
    public int RequestedQuantity { get; set; }
    public string? Notes { get; set; }
} 