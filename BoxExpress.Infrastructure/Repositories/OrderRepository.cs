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

    public async Task<(List<Order> Orders, int TotalCount)> GetFilteredAsync(OrderFilter filter)
    {
        var query = _context.Orders
            .Include(w => w.City)
            .Include(w => w.Country)
            .Include(w => w.Store)
            .Include(w => w.Client)
            .Include(w => w.OrderItems)
                .ThenInclude(w => w.ProductVariant)
                .ThenInclude(w => w.Product)
            .AsQueryable();

        query = GetQueryFiltered(filter, query);

        var totalCount = await query.CountAsync();

        var orderQuery = query.Include(x => x.Client)
        .Include(x => x.Category)
        .Include(x => x.ClientAddress)
        .Include(x => x.Status)
        .Include(x => x.City)
        .Include(x => x.Country)
        .Include(x => x.Warehouse)
        .Include(x => x.TimeSlot)
        .Include(x => x.Currency)
        .OrderByDescending(x => x.CreatedAt)
        .ThenByDescending(x => x.UpdatedAt).AsQueryable();

        if (!filter.IsAll)
        {
            orderQuery = orderQuery
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize);
        }

        return (await orderQuery.ToListAsync(), totalCount);
    }

    private static IQueryable<Order> GetQueryFiltered(OrderFilter filter, IQueryable<Order> query)
    {
        if (filter.CityId.HasValue && filter.CityId > 0)
            query = query.Where(w => w.CityId.Equals(filter.CityId));
        if (filter.CountryId.HasValue && filter.CountryId > 0)
            query = query.Where(w => w.CountryId.Equals(filter.CountryId));
        if (filter.CategoryId.HasValue && filter.CategoryId > 0)
            query = query.Where(w => w.OrderCategoryId.Equals(filter.CategoryId));
        if (filter.EndDate.HasValue)
            query = query.Where(w => w.CreatedAt <= filter.EndDate.Value);
        if (filter.StartDate.HasValue)
            query = query.Where(w => w.CreatedAt >= filter.StartDate.Value);
        if (filter.StoreId.HasValue && filter.StoreId > 0)
            query = query.Where(w => w.StoreId.Equals(filter.StoreId));
        if (filter.OrderId.HasValue && filter.OrderId > 0)
            query = query.Where(w => w.Id.Equals(filter.OrderId));
        if (filter.ScheduledDate.HasValue)
            query = query.Where(w => w.ScheduledDate.HasValue && w.ScheduledDate.Value.Date == filter.ScheduledDate.Value);
        if (filter.WarehouseId.HasValue)
            query = query.Where(w => w.WarehouseId.HasValue && w.WarehouseId.Value == filter.WarehouseId.Value);

        //todo quitar, es por pruebas
        query = query.Where(w => w.IsEnabled.HasValue && w.IsEnabled.Value);

        return query;
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
            .Include(w => w.OrderItems)
            .Include(w => w.Store)
                .ThenInclude(w => w.Wallet)
            .Include(w => w.Currency)
            .FirstOrDefaultAsync(w => w.Id.Equals(id));
    }

    public async Task<Order?> GetByIdAsync(int id)
    {
        return await _context.Set<Order>()
            .Include(w => w.Client)
            .Include(w => w.City)
            .Include(w => w.Warehouse)
            .Include(w => w.Category)
            .Include(w => w.Status)
            .Include(w => w.Currency)
            .Include(w => w.TimeSlot)
            .Include(w => w.ClientAddress)
            .Include(w => w.OrderItems)
            .FirstOrDefaultAsync(w => w.Id.Equals(id));
    }

    public async Task<List<OrderSummary>> GetSummaryAsync(OrderFilter filter)
    {
        var query = _context.Orders
           .Include(w => w.Status)
           .AsQueryable();

        filter.IsAll = true;
        query = GetQueryFiltered(filter, query);

        return await query
        .GroupBy(o => new { o.OrderStatusId, o.Status.Name })
        .Select(g => new OrderSummary
        {
            StatusId = g.Key.OrderStatusId,
            StatusName = g.Key.Name,
            Count = g.Count()
        })
        .ToListAsync();
    }

    public async Task<Order?> GetByCodeAsync(string code, int storeId)
    {
        return await _context.Orders.FirstOrDefaultAsync(w => w.Code.Equals(code) && w.StoreId.Equals(storeId));
    }
}