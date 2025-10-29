namespace BoxExpress.Domain.Filters;

public class WithdrawalRequestFilter : BaseFilter
{
    public int? StoreId { get; set; }
}