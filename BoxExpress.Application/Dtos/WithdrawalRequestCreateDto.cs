using BoxExpress.Application.Dtos.Common;
using BoxExpress.Domain.Entities;

namespace BoxExpress.Application.Dtos;

public class WithdrawalRequestCreateDto : BaseDto
{
    public int StoreId { get; set; }
    public decimal? Amount { get; set; }
    public string? AccountHolder { get; set; }
    public string? Document { get; set; }
    public string? Bank { get; set; }
    public string? AccountNumber { get; set; }
    public string? Description { get; set; }
    public int? DocumentTypeId { get; set; }
}
