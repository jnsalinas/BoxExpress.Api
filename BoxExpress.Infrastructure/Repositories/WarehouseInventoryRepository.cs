using BoxExpress.Domain.Entities;
using BoxExpress.Domain.Interfaces;
using BoxExpress.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using BoxExpress.Domain.Filters;

namespace BoxExpress.Infrastructure.Repositories;

public class WarehouseInventoryRepository : Repository<WarehouseInventory>, IWarehouseInventoryRepository
{
    private readonly BoxExpressDbContext _context;

    public WarehouseInventoryRepository(BoxExpressDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<WarehouseInventory?> GetByWarehouseAndProductVariant(int warehouseId, int productVariantId)
    {
        return await _context.WarehouseInventories
            .Include(wi => wi.ProductVariant)
            .ThenInclude(pv => pv.Product)
            .FirstOrDefaultAsync(wi => wi.WarehouseId == warehouseId && wi.ProductVariantId == productVariantId);
    }

    public async Task<List<WarehouseInventory>> GetByWarehouseAndProductVariants(int warehouseId, List<int> productVariantsId)
    {
        return await _context.WarehouseInventories
            .Include(wi => wi.ProductVariant)
            .ThenInclude(pv => pv.Product)
            .Where(wi => wi.WarehouseId == warehouseId && productVariantsId.Contains(wi.ProductVariantId)).ToListAsync();
    }

    public async Task<List<WarehouseInventory>> GetVariantsAutocompleteAsync(string query, int warehouseOrigonId)
    {
        List<WarehouseInventory> warehouseInventories = await _context.WarehouseInventories
            .Where
            (
                x =>
                    x.WarehouseId == warehouseOrigonId
                    &&
                    (
                        (!string.IsNullOrEmpty(x.ProductVariant.Name) && x.ProductVariant.Name.Contains(query))
                        || (!string.IsNullOrEmpty(x.ProductVariant.Sku) && x.ProductVariant.Sku.Contains(query))
                        || x.ProductVariant.Product.Name.Contains(query)
                        || (!string.IsNullOrEmpty(x.ProductVariant.Product.Sku) && x.ProductVariant.Product.Sku.Contains(query))
                    )
            )
            .Include(x => x.ProductVariant)
            .ThenInclude(x => x.Product)
            .ToListAsync();

        return warehouseInventories;
    }
}