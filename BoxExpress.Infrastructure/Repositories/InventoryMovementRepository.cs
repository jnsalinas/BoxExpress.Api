using BoxExpress.Domain.Entities;
using BoxExpress.Domain.Interfaces;
using BoxExpress.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using BoxExpress.Domain.Filters;

namespace BoxExpress.Infrastructure.Repositories;

public class InventoryMovementRepository : Repository<InventoryMovement>, IInventoryMovementRepository
{
    private readonly BoxExpressDbContext _context;

    public InventoryMovementRepository(BoxExpressDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<(List<InventoryMovement> InventoryMovements, int TotalCount)> GetFilteredAsync(InventoryMovementFilter filter)
    {
        var query = _context.InventoryMovements
            .Include(w => w.Warehouse)
            .Include(w => w.ProductVariant)
            // .Include(w => w.Order)
            // .Include(w => w.Transfer)
            .AsQueryable();

        if (filter.WarehouseId.HasValue)
        {
            query = query.Where(w => w.WarehouseId == filter.WarehouseId.Value);
        }

        if (filter.ProductVariantId.HasValue)
        {
            query = query.Where(w => w.ProductVariantId == filter.ProductVariantId.Value);
        }

        if (filter.MovementType.HasValue)
        {
            query = query.Where(w => w.MovementType == filter.MovementType.Value);
        }

        query = query.Include(w => w.Warehouse)
            .Include(w => w.ProductVariant)
                .ThenInclude(x => x.Product).OrderByDescending(x => x.CreatedAt);

        var total = query.Count();
        if (!filter.IsAll)
        {
            query = query
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize);
        }

        return (await query.ToListAsync(), total);
    }
}