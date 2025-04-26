namespace BoxExpress.Domain.Filters;

public class WalletTransactionFilter : BaseFilter
{
     public int? OrderId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int? StoreId { get; set; }
}