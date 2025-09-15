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
            .Include(wi => wi.Store)
            .FirstOrDefaultAsync(wi => wi.WarehouseId == warehouseId && wi.ProductVariantId == productVariantId);
    }

    public async Task<List<WarehouseInventory>> GetByWarehouseAndProductVariants(int warehouseId, List<int> productVariantsId)
    {
        return await _context.WarehouseInventories
            .Include(wi => wi.ProductVariant)
            .ThenInclude(pv => pv.Product)
            .Include(wi => wi.Store)
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
            .Include(x => x.Store)
            .ToListAsync();

        return warehouseInventories;
    }

    public async Task<(List<WarehouseInventory> WarehouseInventories, int TotalCount)> GetFilteredAsync(WarehouseInventoryFilter filter)
    {
        var query = _context.WarehouseInventories
            .Where
            (
                x =>
                    (filter.WarehouseId == null || x.WarehouseId == filter.WarehouseId)
                    &&
                    (filter.StoreId == null || x.StoreId == filter.StoreId)
                    &&
                    (
                        filter.Query == null ||
                        (
                            !string.IsNullOrEmpty(filter.Query) &&
                            !string.IsNullOrEmpty(x.ProductVariant.Name) && x.ProductVariant.Name.Contains(filter.Query)
                            || (!string.IsNullOrEmpty(x.ProductVariant.Sku) && x.ProductVariant.Sku.Contains(filter.Query))
                            || x.ProductVariant.Product.Name.Contains(filter.Query)
                            || (!string.IsNullOrEmpty(x.ProductVariant.Product.Sku) && x.ProductVariant.Product.Sku.Contains(filter.Query))
                        )
                    )
            )
            .Include(x => x.ProductVariant)
            .ThenInclude(x => x.Product)
            .Include(x => x.Store)
            .AsQueryable();

        if (!filter.IsAll)
        {
            query = query
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize);
        }

        var totalCount = await query.CountAsync();
        return (await query.ToListAsync(), totalCount);
    }

    public async Task<(List<Product> Products, int TotalCount)> GetFilteredGroupedByProductAsync(WarehouseInventoryFilter filter)
    {
        var query = _context.WarehouseInventories
            .Where(x =>
                (filter.WarehouseId == null || x.WarehouseId == filter.WarehouseId) &&
                (filter.StoreId == null || x.StoreId == filter.StoreId) &&
                (
                    filter.Query == null ||
                    (
                        !string.IsNullOrEmpty(x.ProductVariant.Name) && x.ProductVariant.Name.Contains(filter.Query)
                        || (!string.IsNullOrEmpty(x.ProductVariant.Sku) && x.ProductVariant.Sku.Contains(filter.Query))
                        || x.ProductVariant.Product.Name.Contains(filter.Query)
                        || (!string.IsNullOrEmpty(x.ProductVariant.Product.Sku) && x.ProductVariant.Product.Sku.Contains(filter.Query))
                    )
                )
            )
            .Include(x => x.ProductVariant)
            .ThenInclude(x => x.Product);

        var totalCount = await query
            .Select(x => x.ProductVariant.Product.Id)
            .Distinct()
            .CountAsync();

        var productIds = await query
                      .Select(x => x.ProductVariant.Product)
                      .Distinct().ToListAsync();

        if (!filter.IsAll)
        {
            // OBTENER SOLO LOS PRODUCTOS DEL RANGO
            productIds = await query
                .Select(x => x.ProductVariant.Product)
                .Distinct()
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();
        }

        return (productIds, totalCount);
    }


    public async Task<List<WarehouseInventory>> GetByWarehouseAndProductsId(int? warehouseId, List<int> productIds, WarehouseInventoryFilter? filter)
    {
        var query = _context.WarehouseInventories
            .Where(x => (warehouseId == null || x.WarehouseId == warehouseId) && productIds.Contains(x.ProductVariant.ProductId));

        if (filter != null)
        {
            query = query.Where(x =>
                filter.Query == null ||
                    (
                        !string.IsNullOrEmpty(x.ProductVariant.Name) && x.ProductVariant.Name.Contains(filter.Query)
                        || (!string.IsNullOrEmpty(x.ProductVariant.Sku) && x.ProductVariant.Sku.Contains(filter.Query))
                        | (!string.IsNullOrEmpty(x.ProductVariant.ShopifyVariantId) && x.ProductVariant.ShopifyVariantId.Contains(filter.Query))
                    )
            ).AsQueryable();
        }

        return await query.Include(x => x.ProductVariant).Include(x => x.Warehouse).Include(x => x.Store).ToListAsync();
    }

    public async Task<WarehouseInventory?> GetByIdWithDetailsAsync(int id)
    {
        return await _context.WarehouseInventories
            .Include(wi => wi.ProductVariant)
            .ThenInclude(pv => pv.Product)
            .Include(wi => wi.Warehouse)
            .Include(wi => wi.Store)
            .FirstOrDefaultAsync(wi => wi.Id == id);
    }

    public async Task<List<WarehouseInventory>> GetBySkusAsync(HashSet<string> skus, int? storeId = null)
    {
        return await _context.WarehouseInventories
            .Include(x => x.ProductVariant)
            .Where(x => x.ProductVariant != null && x.ProductVariant.Sku != null && x.ProductVariant.Sku != string.Empty
             && skus.Contains(x.ProductVariant.Sku) && (storeId == null || x.StoreId == storeId))
            .ToListAsync();
    }

    public async Task<List<WarehouseInventory>> GetByWarehouseIdAndProductVariantsIdAndStoresId(int warehouseId, List<int> productVariantsId, List<int>? storesId)
    {
        return await _context.WarehouseInventories
            .Include(wi => wi.ProductVariant)
            .ThenInclude(pv => pv.Product)
            .Include(wi => wi.Store)
            .Where(wi => wi.WarehouseId == warehouseId &&
                (productVariantsId.Contains(wi.ProductVariantId) || (storesId != null && storesId.Contains(wi.StoreId ?? 0))))
            .ToListAsync();
    }
}