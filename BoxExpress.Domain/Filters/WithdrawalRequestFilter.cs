using BoxExpress.Domain.Enums;

namespace BoxExpress.Domain.Filters;

public class WithdrawalRequestFilter : BaseFilter
{
    public int? StoreId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public WithdrawalRequestStatus? Status { get; set; }
}