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
}