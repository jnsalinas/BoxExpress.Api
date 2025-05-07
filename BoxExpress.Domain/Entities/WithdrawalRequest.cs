using BoxExpress.Domain.Enums;

namespace BoxExpress.Domain.Entities;

public class WithdrawalRequest : BaseEntity
{
    public int StoreId { get; set; }
    public Store Store { get; set; } = null!;
    public decimal Amount { get; set; }
    public string? AccountHolder { get; set; }
    public string? Document { get; set; }
    public int? DocumentTypeId { get; set; }
    public DocumentType? DocumentType { get; set; }
    public string? Bank { get; set; }
    public string? AccountNumber { get; set; }
    public string? Description { get; set; }
    public WithdrawalRequestStatus Status { get; set; } = WithdrawalRequestStatus.Pending;
    public DateTime? ProcessedAt { get; set; }
    public int CreatorId { get; set; }
    public User Creator { get; set; } = null!;
    public int? ReviewedByUserId { get; set; }
    public User? ReviewedByUser { get; set; }
    public string? Reason { get; set; }
}