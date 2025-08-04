using BoxExpress.Domain.Enums;

namespace BoxExpress.Application.Dtos;

public class UpdateProductLoanDto
{
    public DateTime LoanDate { get; set; }
    public string ResponsibleName { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public ProductLoanStatus Status { get; set; }
}

public class UpdateProductLoanDetailDto
{
    public int Id { get; set; }
    public int DeliveredQuantity { get; set; }
    public int ReturnedQuantity { get; set; }
    public string? Notes { get; set; }
} 