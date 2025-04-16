namespace BoxExpress.Application.Dtos;

public class WalletTransactionDto
{
    public string? StoreName { get; set; }
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public string? Description { get; set; }
    public int? RelatedOrderId { get; set; }
    public string? UserName { get; set; }
}
