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
    public string Status { get; set; } = "Pending"; //todo poner esto en una tabla
    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ProcessedAt { get; set; }
}