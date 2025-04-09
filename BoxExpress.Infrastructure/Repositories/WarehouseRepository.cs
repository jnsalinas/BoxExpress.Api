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
            query = query.Where(w => w.CityId == filter.CityId);

        if (filter.CountryId.HasValue && filter.CountryId > 0)
            query = query.Where(w => w.CountryId == filter.CountryId);

        return await query.ToListAsync();
    }
}