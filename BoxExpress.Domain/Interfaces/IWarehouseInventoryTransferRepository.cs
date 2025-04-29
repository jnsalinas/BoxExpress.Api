
using BoxExpress.Domain.Entities;
using BoxExpress.Domain.Filters;

namespace BoxExpress.Domain.Interfaces;
public interface IWarehouseInventoryTransferRepository : IRepository<WarehouseInventoryTransfer>
{
    Task<WarehouseInventoryTransfer?> GetByIdWithDetailsAsync(int id);
}