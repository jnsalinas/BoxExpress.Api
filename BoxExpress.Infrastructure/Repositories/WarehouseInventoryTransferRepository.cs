using BoxExpress.Domain.Entities;
using BoxExpress.Domain.Interfaces;
using BoxExpress.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using BoxExpress.Domain.Filters;

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
        var query = _context.WarehouseInventoryTransfers
            .AsQueryable();

        //todo: poner los filtros 
        if (filter.ToWarehouseId.HasValue)
        {
            query = query.Where(x => x.ToWarehouseId.Equals(filter.ToWarehouseId.Value));
        }
        if (filter.FromWarehouseId.HasValue)
        {
            query = query.Where(x => x.FromWarehouseId.Equals(filter.FromWarehouseId.Value));
        }

        var totalCount = await query.CountAsync();
        var warehouseInventoryTransferQuery = query
            .Include(w => w.Creator)
            .Include(w => w.TransferDetails)
                .ThenInclude(x => x.ProductVariant)
                .ThenInclude(x => x.Product)
            .Include(w => w.ToWarehouse)
            .Include(w => w.FromWarehouse)
            .OrderBy(x => x.UpdatedAt)
            .AsQueryable();

        if (!filter.IsAll)
        {
            warehouseInventoryTransferQuery = warehouseInventoryTransferQuery
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize);
        }

        return (await warehouseInventoryTransferQuery.ToListAsync(), totalCount);
    }

    public async Task<WarehouseInventoryTransfer?> GetByIdWithDetailsAsync(int id)
    {
        return
        await _context.WarehouseInventoryTransfers
            .Include(x => x.TransferDetails)
            .FirstOrDefaultAsync(x => x.Id == id);
    }
}