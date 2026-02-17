using BoxExpress.Application.Dtos.Common;
using BoxExpress.Domain.Enums;

namespace BoxExpress.Application.Dtos;

public class WithdrawalRequestFilterDto : BaseFilterDto
{
    public int? StoreId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public WithdrawalRequestStatus? Status { get; set; }
}