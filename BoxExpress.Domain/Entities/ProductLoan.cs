using BoxExpress.Domain.Enums;

namespace BoxExpress.Domain.Entities;

public class ProductLoan : BaseEntity
{
    public DateTime LoanDate { get; set; }
    public string ResponsibleName { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public int WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; } = null!;
    public int CreatedById { get; set; }
    public User CreatedBy { get; set; } = null!;
    public ProductLoanStatus Status { get; set; } = ProductLoanStatus.Pending;
    public DateTime? ProcessedAt { get; set; }
    public int? ProcessedById { get; set; }
    public User? ProcessedBy { get; set; }
    
    // Navigation property para los detalles
    public ICollection<ProductLoanDetail> Details { get; set; } = new List<ProductLoanDetail>();
} 