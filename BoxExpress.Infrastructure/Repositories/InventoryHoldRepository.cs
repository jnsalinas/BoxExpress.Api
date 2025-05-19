using BoxExpress.Domain.Entities;
using BoxExpress.Domain.Interfaces;
using BoxExpress.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using BoxExpress.Domain.Filters;
using BoxExpress.Domain.Enums;

namespace BoxExpress.Infrastructure.Repositories;

public class InventoryHoldRepository : Repository<InventoryHold>, IInventoryHoldRepository
{
    private readonly BoxExpressDbContext _context;

    public InventoryHoldRepository(BoxExpressDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<(List<InventoryHold> InventoryHolds, int TotalCount)> GetFilteredAsync(InventoryHoldFilter filter)
    {
        var query = _context.InventoryHolds
            .Include(w => w.Creator)
            .Include(w => w.OrderItem)
            .Where(x => x.Status == InventoryHoldStatus.Active)
            .AsQueryable();

        if (filter.WarehouseInventoryId.HasValue)
        {
            query = query.Where(w => w.WarehouseInventoryId == filter.WarehouseInventoryId.Value);
        }
        
        var total = query.Count();
        if (!filter.IsAll)
        {
            query = query
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize);
        }

        return (await query.ToListAsync(), total);
    }

    public async Task<List<InventoryHold>> GetByOrderItemIdsAndStatus(List<int> listOrderItemIds, InventoryHoldStatus? status)
    {
        return await _context.InventoryHolds
                .Where(w => w.OrderItemId.HasValue && listOrderItemIds.Contains(w.OrderItemId.Value)
                && (!status.HasValue || (w.Status == status)))
                .ToListAsync();
    }

    public async Task<List<InventoryHold>> GetByTransferIdsAndStatus(int transferId, InventoryHoldStatus? status)
    {
        return await _context.InventoryHolds
                .Where(w => w.TransferId.HasValue && (!status.HasValue || (w.Status == status)))
                .ToListAsync();
    }

}