using BoxExpress.Domain.Entities;
using BoxExpress.Domain.Interfaces;
using BoxExpress.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using BoxExpress.Domain.Filters;

namespace BoxExpress.Infrastructure.Repositories;

public class WarehouseRepository : Repository<Warehouse>, IWarehouseRepository
{
    private readonly BoxExpressDbContext _context;

    public WarehouseRepository(BoxExpressDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<List<Warehouse>> GetFilteredAsync(WarehouseFilter filter)
    {
        var query = _context.Warehouses
            .Include(w => w.City)
            .Include(w => w.Country)
            .AsQueryable();

        if (!string.IsNullOrEmpty(filter.Name))
            query = query.Where(w => w.Name.Contains(filter.Name));

        if (filter.CityId.HasValue && filter.CityId > 0)
            query = query.Where(w => w.CityId.Equals(filter.CityId));

        if (filter.CountryId.HasValue && filter.CountryId > 0)
            query = query.Where(w => w.CountryId.Equals(filter.CountryId));

        return await query.ToListAsync();
    }

    //toodo mejor pasar que devuuelva los productvariants en otro lado para poder ordenar, filtrar y paginar 
    public async Task<Warehouse?> GetByIdWithDetailsAsync(int id)
    {
        return await _context.Set<Warehouse>()
            // .Include(w => w.Inventories)
            //     .ThenInclude(p => p.ProductVariant)
            //         .ThenInclude(p => p.Product)
            .FirstOrDefaultAsync(w => w.Id.Equals(id));
    }
}