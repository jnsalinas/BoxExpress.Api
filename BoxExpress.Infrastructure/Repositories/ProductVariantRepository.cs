using BoxExpress.Domain.Entities;
using BoxExpress.Domain.Interfaces;
using BoxExpress.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using BoxExpress.Domain.Filters;

namespace BoxExpress.Infrastructure.Repositories;

public class ProductVariantRepository : Repository<ProductVariant>, IProductVariantRepository
{
    private readonly BoxExpressDbContext _context;

    public ProductVariantRepository(BoxExpressDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<List<ProductVariant>> GetAllAsync(ProductVariantFilter filter)
    {
        var query = _context.ProductVariants.AsQueryable();

        if (filter.StoreId.HasValue)
        {
            query = query
                .Include(pv => pv.Product)
                .Include(pv => pv.WarehouseInventories
                    .Where(wi => wi.StoreId == filter.StoreId.Value))
                .ThenInclude(wi => wi.Warehouse)
                .Where(pv => pv.WarehouseInventories.Any(wi => wi.StoreId == filter.StoreId.Value));
        }

        return await query.ToListAsync();
    }

    public async Task<ProductVariant?> GetByIdWithDetailsAsync(int id)
    {
        return await _context.ProductVariants
            .Include(w => w.InventoryMovements)
                .ThenInclude(w => w.Warehouse)
            .Include(w => w.InventoryMovements)
                .ThenInclude(w => w.Order)
            .Include(w => w.InventoryMovements)
                .ThenInclude(w => w.Transfer)
            .FirstOrDefaultAsync(w => w.Id.Equals(id));
    }

    public async Task<List<ProductVariant>> GetByIdsAsync(List<int> ids)
    {
        return await _context.ProductVariants.Where(x => ids.Contains(x.Id)).ToListAsync();
    }

    public async Task<ProductVariant?> GetByProductNameVariantNameAndStoreId(string productName, string productVariantName, int storeId)
    {
        return await _context.ProductVariants
        .Where(pv => pv.Name == productVariantName && pv.Product.Name == productName && pv.WarehouseInventories.Any(wi => wi.StoreId == storeId))
        .OrderBy(x => x.CreatedAt)
            .FirstOrDefaultAsync();
    }
}