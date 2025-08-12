namespace BoxExpress.Application.Dtos;

public class CreateProductLoanDto
{
    public int? Id { get; set; }
    public DateTime LoanDate { get; set; }
    public string ResponsibleName { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public int WarehouseId { get; set; }
    public List<CreateProductLoanDetailDto> Details { get; set; } = new List<CreateProductLoanDetailDto>();
}