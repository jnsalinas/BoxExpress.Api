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

    public async Task<WarehouseInventoryTransfer?> GetByIdWithDetailsAsync(int id)
    {
        return
        await _context.WarehouseInventoryTransfers
            .Include(x => x.TransferDetails)
            .FirstOrDefaultAsync(x => x.Id == id);
    }
}