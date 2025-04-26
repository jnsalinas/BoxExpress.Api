using BoxExpress.Application.Dtos.Common;

namespace BoxExpress.Application.Dtos;

public class WalletTransactionFilterDto : BaseFilterDto
{
    public int? OrderId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int? StoreId { get; set; }
}