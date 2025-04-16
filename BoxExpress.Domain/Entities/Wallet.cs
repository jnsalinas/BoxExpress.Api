namespace BoxExpress.Domain.Entities;

public class Wallet : BaseEntity
{
    public decimal Balance { get; set; }
    public Store Store { get; set; } = null!;

}