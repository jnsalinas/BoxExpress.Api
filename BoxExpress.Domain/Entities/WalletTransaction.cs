namespace BoxExpress.Domain.Entities;

public class WalletTransaction : BaseEntity
{
    public int WalletId { get; set; }
    public Wallet Wallet { get; set; } = null!;
    public int TransactionTypeId { get; set; }
    public TransactionType TransactionType { get; set; } = null!;
    public decimal Amount { get; set; }
    public string? Description { get; set; }
    public int? RelatedOrderId { get; set; }
    public Order? RelatedOrder { get; set; }
}