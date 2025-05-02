using BoxExpress.Application.Dtos.Common;
using BoxExpress.Domain.Entities;

namespace BoxExpress.Application.Dtos;

public class WithdrawalRequestDto : BaseDto
{
    public int StoreId { get; set; }
    public string? Store { get; set; } = null!;
    public decimal? Amount { get; set; }
    public string? AccountHolder { get; set; }
    public string? Document { get; set; }
    public string? Bank { get; set; }
    public string? AccountNumber { get; set; }
    public string? Description { get; set; }
    public int? Status { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public string? Creator { get; set; }
}
