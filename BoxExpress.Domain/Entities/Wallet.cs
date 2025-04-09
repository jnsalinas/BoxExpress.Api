namespace BoxExpress.Domain.Entities;

public class Wallet : BaseEntity
{
    public int StoreId { get; set; }
    public Store Store { get; set; } = null!;
    public decimal Balance { get; set; }
}