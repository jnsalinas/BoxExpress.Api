using BoxExpress.Domain.Enums;

namespace BoxExpress.Domain.Filters;

public class ProductLoanFilter : BaseFilter
{
    public int? WarehouseId { get; set; }
    public ProductLoanStatus? Status { get; set; }
    public string? ResponsibleName { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int? CreatedById { get; set; }
} 