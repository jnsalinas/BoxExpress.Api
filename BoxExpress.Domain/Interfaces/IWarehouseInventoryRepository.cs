
using BoxExpress.Domain.Entities;
using BoxExpress.Domain.Filters;

namespace BoxExpress.Domain.Interfaces;

public interface IWarehouseInventoryRepository : IRepository<WarehouseInventory>
{
    Task<WarehouseInventory?> GetByIdWithDetailsAsync(int id);
    Task<WarehouseInventory?> GetByWarehouseAndProductVariant(int warehouseId, int productVariantId);
    Task<List<WarehouseInventory>> GetByWarehouseAndProductVariants(int warehouseId, List<int> productVariantsId);
    Task<List<WarehouseInventory>> GetVariantsAutocompleteAsync(string query, int warehouseOrigonId);
    Task<(List<WarehouseInventory> WarehouseInventories, int TotalCount)> GetFilteredAsync(WarehouseInventoryFilter filter);
    Task<(List<Product> Products, int TotalCount)> GetFilteredGroupedByProductAsync(WarehouseInventoryFilter filter);
    Task<List<WarehouseInventory>> GetByWarehouseAndProductsId(int? warehouseId, List<int> productIds, WarehouseInventoryFilter? filter = null);
    Task<List<WarehouseInventory>> GetBySkusAsync(HashSet<string> skus, int? storeId = null);
}