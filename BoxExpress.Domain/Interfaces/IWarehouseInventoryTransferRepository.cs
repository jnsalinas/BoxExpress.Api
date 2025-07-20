
using BoxExpress.Domain.Entities;
using BoxExpress.Domain.Filters;

namespace BoxExpress.Domain.Interfaces;
public interface IWarehouseInventoryTransferRepository : IRepository<WarehouseInventoryTransfer>
{
    Task<(List<WarehouseInventoryTransfer> Transactions, int TotalCount)> GetFilteredAsync(WarehouseInventoryTransferFilter filter);
    Task<WarehouseInventoryTransfer?> GetByIdWithDetailsAsync(int id);
    Task<int> GetPendingTransfersAsync(WarehouseInventoryTransferFilter warehouseInventoryTransferFilter);
}