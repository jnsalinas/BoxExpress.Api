using BoxExpress.Domain.Enums;
using BoxExpress.Application.Dtos.Common;

namespace BoxExpress.Application.Dtos;

public class ProductLoanFilterDto : BaseFilterDto
{
    public int? WarehouseId { get; set; }
    public ProductLoanStatus? Status { get; set; }
    public string? ResponsibleName { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int? CreatedById { get; set; }
} 