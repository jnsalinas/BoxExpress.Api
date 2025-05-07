namespace BoxExpress.Domain.Entities;

public class Wallet : BaseEntity
{
    public Store Store { get; set; } = null!;
    public decimal PendingWithdrawals { get; set; }
    public decimal Balance { get; set; }
}