
using BoxExpress.Domain.Entities;
using BoxExpress.Domain.Filters;

namespace BoxExpress.Domain.Interfaces;
public interface IWarehouseInventoryRepository : IRepository<WarehouseInventory>
{
    Task<WarehouseInventory?> GetByWarehouseAndProductVariant(int warehouseId, int productVariantId);
    Task<List<WarehouseInventory>> GetByWarehouseAndProductVariants(int warehouseId, List<int> productVariantsId);
    Task<List<WarehouseInventory>> GetVariantsAutocompleteAsync(string query, int warehouseOrigonId);
}