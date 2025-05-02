using BoxExpress.Application.Dtos.Common;

namespace BoxExpress.Application.Dtos;

public class WithdrawalRequestFilterDto : BaseFilterDto
{
    public int? StoreId { get; set; }
}