using BoxExpress.Application.Dtos.Common;

namespace BoxExpress.Application.Dtos;

public class ProductLoanDetailDto : BaseDto
{
    public int ProductLoanId { get; set; }
    public int ProductVariantId { get; set; }
    public string ProductVariantName { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string? ProductVariantDescription { get; set; }
    public int RequestedQuantity { get; set; }
    public int DeliveredQuantity { get; set; }
    public int ReturnedQuantity { get; set; }
    public int PendingReturnQuantity { get; set; }
} 