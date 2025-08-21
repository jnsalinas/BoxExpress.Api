using BoxExpress.Domain.Entities;
using BoxExpress.Domain.Interfaces;
using BoxExpress.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using BoxExpress.Domain.Filters;
using BoxExpress.Domain.Enums;

namespace BoxExpress.Infrastructure.Repositories;

public class WarehouseInventoryTransferRepository : Repository<WarehouseInventoryTransfer>, IWarehouseInventoryTransferRepository
{
    private readonly BoxExpressDbContext _context;

    public WarehouseInventoryTransferRepository(BoxExpressDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<(List<WarehouseInventoryTransfer> Transactions, int TotalCount)> GetFilteredAsync(WarehouseInventoryTransferFilter filter)
    {
        var query = _context.WarehouseInventoryTransfers.AsQueryable();
        query = BuildQueryFilter(filter, query);

        var totalCount = await query.CountAsync();
        var warehouseInventoryTransferQuery = query
            .Include(w => w.Creator)
            .Include(w => w.TransferDetails)
                .ThenInclude(x => x.ProductVariant)
                .ThenInclude(x => x.Product)
            .Include(w => w.ToWarehouse)
            .Include(w => w.FromWarehouse)
            .OrderByDescending(x => x.Id)
            .AsQueryable();

        if (!filter.IsAll)
        {
            warehouseInventoryTransferQuery = warehouseInventoryTransferQuery
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize);
        }

        return (await warehouseInventoryTransferQuery.ToListAsync(), totalCount);
    }

    private static IQueryable<WarehouseInventoryTransfer> BuildQueryFilter(WarehouseInventoryTransferFilter filter, IQueryable<WarehouseInventoryTransfer> query)
    {
        if (filter.ToWarehouseId.HasValue)
        {
            query = query.Where(x => x.ToWarehouseId.Equals(filter.ToWarehouseId.Value));
        }
        if (filter.FromWarehouseId.HasValue)
        {
            query = query.Where(x => x.FromWarehouseId.Equals(filter.FromWarehouseId.Value));
        }
        if (filter.StartDate.HasValue)
        {
            query = query.Where(x => x.CreatedAt >= filter.StartDate.Value);
        }
        if (filter.EndDate.HasValue)
        {
            query = query.Where(x => x.CreatedAt <= filter.EndDate.Value);
        }
        if (filter.StatusId.HasValue)
        {
            query = query.Where(x => x.Status == filter.StatusId.Value);
        }

        return query;
    }

    public async Task<WarehouseInventoryTransfer?> GetByIdWithDetailsAsync(int id)
    {
        return
        await _context.WarehouseInventoryTransfers
            .Include(x => x.TransferDetails)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public Task<int> GetPendingTransfersAsync(WarehouseInventoryTransferFilter filter)
    {
        var query = _context.WarehouseInventoryTransfers.AsQueryable();
        query = query.Where(x => x.Status == InventoryTransferStatus.Pending);
        query = BuildQueryFilter(filter, query);
        return query.CountAsync();

    }
}