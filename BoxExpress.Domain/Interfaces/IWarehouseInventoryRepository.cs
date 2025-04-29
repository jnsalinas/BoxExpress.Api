
using BoxExpress.Domain.Entities;
using BoxExpress.Domain.Filters;

namespace BoxExpress.Domain.Interfaces;
public interface IWarehouseInventoryRepository : IRepository<WarehouseInventory>
{
    Task<WarehouseInventory?> GetByWarehouseAndProductVariant(int warehouseId, int productVariantId);
}