
using BoxExpress.Domain.Entities;
using BoxExpress.Domain.Filters;

namespace BoxExpress.Domain.Interfaces;
public interface IWarehouseRepository : IRepository<Warehouse>
{
    Task<List<Warehouse>> GetFilteredAsync(WarehouseFilter filter);
    Task<Warehouse?> GetByIdWithDetailsAsync(int id);
}