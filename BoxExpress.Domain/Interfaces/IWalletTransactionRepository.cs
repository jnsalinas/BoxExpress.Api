
using BoxExpress.Domain.Entities;
using BoxExpress.Domain.Filters;

namespace BoxExpress.Domain.Interfaces;
public interface IWalletTransactionRepository : IRepository<WalletTransaction>
{
    Task<List<WalletTransaction>> GetFilteredAsync(WalletTransactionFilter filter);
}