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
}