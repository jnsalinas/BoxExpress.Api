namespace BoxExpress.Domain.Entities;

public class WithdrawalRequest : BaseEntity
{
    public int StoreId { get; set; }
    public Store Store { get; set; } = null!;
    public decimal Amount { get; set; }
    public string? AccountHolder { get; set; }
    public string? Document { get; set; }
    public string? Bank { get; set; }
    public string? AccountNumber { get; set; }
    public string? Description { get; set; }
    public WithdrawalRequestStatus Status { get; set; } = WithdrawalRequestStatus.Pending;
    public DateTime? ProcessedAt { get; set; }
    public int CreatorId { get; set; }
    public User Creator { get; set; } = null!;
}


public enum WithdrawalRequestStatus //todo: pasar a otra carpeta
{
    Pending = 0,
    Accepted = 1,
    Rejected = 2
}
