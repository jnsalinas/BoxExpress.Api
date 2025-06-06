
using BoxExpress.Domain.Entities;
using BoxExpress.Domain.Filters;

namespace BoxExpress.Domain.Interfaces;
public interface IWalletRepository : IRepository<Wallet>
{
    Task<Wallet?> GetByStoreIdAsync(int storeId);
}