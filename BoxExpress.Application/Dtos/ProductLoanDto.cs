using BoxExpress.Domain.Enums;
using BoxExpress.Application.Dtos.Common;

namespace BoxExpress.Application.Dtos;

public class ProductLoanDto : BaseDto
{
    public DateTime LoanDate { get; set; }
    public string ResponsibleName { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public int WarehouseId { get; set; }
    public string WarehouseName { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public int CreatedById { get; set; }
    public string CreatedByName { get; set; } = string.Empty;
    public ProductLoanStatus Status { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public int? ProcessedById { get; set; }
    public string? ProcessedByName { get; set; }
    public int TotalRequestedQuantity { get; set; }
    public int TotalDeliveredQuantity { get; set; }
    public int TotalReturnedQuantity { get; set; }
    public int TotalPendingReturnQuantity { get; set; }
    public List<ProductLoanDetailDto> Details { get; set; } = new List<ProductLoanDetailDto>();
} 