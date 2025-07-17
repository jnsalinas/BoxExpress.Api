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


    public async Task<List<ProductVariant>> GetBySkusAsync(HashSet<string> skus)
    {
        return await _context.ProductVariants
            .Where(x => x.Sku != null && skus.Contains(x.Sku))
            .ToListAsync();
    }
    // public async Task<List<ProductVariant>> GetVariantsAutocompleteAsync(string query, int warehouseOrigonId)
    // {
    //     List<ProductVariant> productVariants = await _context.ProductVariants
    //         .Where
    //         (
    //             x =>
    //                 (!string.IsNullOrEmpty(x.Name) && x.Name.Contains(query))
    //                 || (!string.IsNullOrEmpty(x.Sku) && x.Sku.Contains(query))
    //                 || x.Product.Name.Contains(query)
    //                 || (!string.IsNullOrEmpty(x.Product.Sku) && x.Product.Sku.Contains(query))
    //         )
    //         .Include(x => x.Product)
    //         .ToListAsync();

    //     foreach (var productVariant in productVariants)
    //     {
    //         productVariant.AvailableUnits = (await _context.WarehouseInventories
    //         .FirstOrDefaultAsync(x => x.ProductVariantId
    //         .Equals(productVariant.Id) && x.WarehouseId.Equals(warehouseOrigonId)))?.AvailableQuantity;
    //     }

    //     return productVariants;
    // }
}