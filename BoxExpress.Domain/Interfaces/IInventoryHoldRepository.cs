
using BoxExpress.Domain.Entities;
using BoxExpress.Domain.Enums;
using BoxExpress.Domain.Filters;

namespace BoxExpress.Domain.Interfaces;

public interface IInventoryHoldRepository : IRepository<InventoryHold>
{
    Task<InventoryHold?> GetByIdWithDetailsAsync(int warehouseinvetoryId);
    Task<(List<InventoryHold> InventoryHolds, int TotalCount)> GetFilteredAsync(InventoryHoldFilter filter);
    Task<List<InventoryHold>> GetByOrderItemIdsAndStatus(List<int> listOrderItemIds, InventoryHoldStatus? status);
    Task<List<InventoryHold>> GetByTransferIdsAndStatus(int transferId, InventoryHoldStatus? status);
    Task<List<InventoryHold>> GetByWarehouseInventoryAndStatus(int warehouseinvetoryId, InventoryHoldStatus? status);
}