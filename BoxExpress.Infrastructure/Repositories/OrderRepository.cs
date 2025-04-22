using BoxExpress.Domain.Entities;
using BoxExpress.Domain.Interfaces;
using BoxExpress.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using BoxExpress.Domain.Filters;

namespace BoxExpress.Infrastructure.Repositories;

public class OrderRepository : Repository<Order>, IOrderRepository
{
    private readonly BoxExpressDbContext _context;

    public OrderRepository(BoxExpressDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<List<Order>> GetFilteredAsync(OrderFilter filter)
    {
        var query = _context.Orders
            .Include(w => w.City)
            .Include(w => w.Country)
            .Include(w => w.Store)
            .Include(w => w.Client)
            .AsQueryable();

        if (filter.CityId.HasValue && filter.CityId > 0)
            query = query.Where(w => w.CityId.Equals(filter.CityId));

        if (filter.CountryId.HasValue && filter.CountryId > 0)
            query = query.Where(w => w.CountryId.Equals(filter.CountryId));

        if (filter.CategoryId.HasValue && filter.CategoryId > 1)
            query = query.Where(w => w.OrderCategoryId.Equals(filter.CategoryId));

        return await query.Include(x => x.Client)
        .Include(x => x.Category)
        .Include(x => x.ClientAddress)
        .Include(x => x.Status)
        .Include(x => x.City)
        .Include(x => x.Country)
        .Include(x => x.Warehouse)
        .ToListAsync();
    }

    public async Task<Order?> GetByIdWithDetailsAsync(int id)
    {
        return await _context.Set<Order>()
            .Include(w => w.Client)
            .Include(w => w.City)
            .Include(w => w.Country)
            .Include(w => w.ClientAddress)
            .Include(w => w.Warehouse)
            .Include(w => w.Status)
            .Include(w => w.Category)
            .Include(w => w.Store)
                .ThenInclude(w => w.Wallet)
            .FirstOrDefaultAsync(w => w.Id.Equals(id));
    }
}