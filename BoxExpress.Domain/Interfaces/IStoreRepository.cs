
using BoxExpress.Domain.Entities;
using BoxExpress.Domain.Filters;

namespace BoxExpress.Domain.Interfaces;
public interface IStoreRepository : IRepository<Store>
{
    Task<(List<Store> Stores, int TotalCount)> GetFilteredAsync(StoreFilter filter);
    Task<Store?> GetByIdWithDetailsAsync(int storeId);
}