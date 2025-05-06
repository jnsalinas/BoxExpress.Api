namespace BoxExpress.Domain.Entities;

public class WalletTransaction : BaseEntity
{
    public int WalletId { get; set; }
    public Wallet Wallet { get; set; } = null!;
    public int TransactionTypeId { get; set; }
    public TransactionType TransactionType { get; set; } = null!;
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public int? RelatedOrderId { get; set; }
    public Order? RelatedOrder { get; set; } = null!;
    public int CreatorId { get; set; }
    public User Creator { get; set; } = null!;
    public int? OrderStatusId { get; set; }
    public OrderStatus? OrderStatus { get; set; } = null!;
    public int? RelatedWithdrawalRequestId { get; set; }
    public WithdrawalRequest? RelatedWithdrawalRequest { get; set; } = null!;
}