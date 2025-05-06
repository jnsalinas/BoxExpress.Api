
using BoxExpress.Domain.Entities;
using BoxExpress.Domain.Filters;

namespace BoxExpress.Domain.Interfaces;
public interface IWithdrawalRequestRepository : IRepository<WithdrawalRequest>
{
    Task<(List<WithdrawalRequest> Transactions, int TotalCount)> GetFilteredAsync(WithdrawalRequestFilter filter);
    Task<WithdrawalRequest?> GetByIdWithDetailsAsync(int id);
}