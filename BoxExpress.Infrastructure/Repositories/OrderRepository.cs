using BoxExpress.Domain.Entities;
using BoxExpress.Domain.Interfaces;
using BoxExpress.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using BoxExpress.Domain.Filters;
using BoxExpress.Domain.Constants;
using Microsoft.Identity.Client;

namespace BoxExpress.Infrastructure.Repositories;

public class OrderRepository : Repository<Order>, IOrderRepository
{
    private readonly IOrderStatusRepository _orderStatusRepository;
    public OrderRepository(BoxExpressDbContext context, IOrderStatusRepository orderStatusRepository) : base(context)
    {
        _orderStatusRepository = orderStatusRepository;
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

        var orderQuery = query.Include(x => x.Client)
            .Include(x => x.Category)
            .Include(x => x.ClientAddress)
            .Include(x => x.Status)
            .Include(x => x.City)
            .Include(x => x.Country)
            .Include(x => x.Warehouse)
            .Include(x => x.TimeSlot)
            .Include(x => x.Currency)
        .OrderByDescending(x => x.Id)
        //.ThenByDescending(x => x.UpdatedAt)
        .AsQueryable();

        if (!filter.IsAll)
        {
            orderQuery = orderQuery
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize);
        }

        var totalCount = await query.CountAsync();
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
        if (filter.StartScheduledDate.HasValue)
            query = query.Where(w => w.ScheduledDate.HasValue && w.ScheduledDate.Value.Date >= filter.StartScheduledDate.Value.Date);
        if (filter.EndScheduledDate.HasValue)
            query = query.Where(w => w.ScheduledDate.HasValue && w.ScheduledDate.Value.Date <= filter.EndScheduledDate.Value.Date);
        if (filter.WarehouseId.HasValue)
            query = query.Where(w => w.WarehouseId.HasValue && w.WarehouseId.Value == filter.WarehouseId.Value);
        if (filter.StatusId.HasValue)
            query = query.Where(w => w.OrderStatusId.Equals(filter.StatusId));
        if (filter.TimeSlotId.HasValue && filter.TimeSlotId > 0)
            query = query.Where(w => w.TimeSlotId.Equals(filter.TimeSlotId));
        if (filter.TimeSlotId.HasValue && filter.TimeSlotId == 0)
            query = query.Where(w => w.TimeSlotId == null);
        if (filter.CityIds != null && filter.CityIds.Count > 0)
            query = query.Where(w => filter.CityIds.Contains(w.CityId!.Value));
        if (!string.IsNullOrEmpty(filter.Query))
        {
            query = query.Where(w =>
            w.Id.ToString().Contains(filter.Query) ||
             (
                w.Client != null
                && (w.Client.FirstName + " " + w.Client.LastName).Contains(filter.Query)
                || w.Client.Email.Contains(filter.Query)
                || w.Client.Phone.Contains(filter.Query)
                || w.Client.Document.Contains(filter.Query)
                || w.Client.Phone.Contains(filter.Query)
            )
            ||
            (
                w.ClientAddress != null && (
                w.ClientAddress.Address.Contains(filter.Query)
                || w.ClientAddress.Address2.Contains(filter.Query)
                || w.ClientAddress.Complement.Contains(filter.Query)
                || w.ClientAddress.PostalCode.Contains(filter.Query)
            )
            ));
            if (filter.Phones != null && filter.Phones.Count > 0)
                query = query.Where(w => !string.IsNullOrEmpty(w.Client.Phone) && filter.Phones.Contains(w.Client.Phone));
        }
        if (filter.ProductVariantIds != null && filter.ProductVariantIds.Count > 0)
            query = query.Where(w => w.OrderItems.Any(oi => filter.ProductVariantIds.Contains(oi.ProductVariantId)));

        if (filter.CountryId != null)
        {
            query = query.Where(w => w.CountryId == filter.CountryId);
        }

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
        query = query.Where(w => w.OrderStatusId != null);

        return await query
        .GroupBy(o => new { o.OrderStatusId, o.Status.Name })
        .Select(g => new OrderSummary
        {
            Id = g.Key.OrderStatusId!.Value,
            Name = g.Key.Name!,
            Count = g.Count()
        })
        .ToListAsync();
    }

    public async Task<List<OrderSummary>> GetSummaryCategoryAsync(OrderFilter filter)
    {
        var query = _context.Orders
           .Include(w => w.Category)
           .AsQueryable();

        filter.IsAll = true;
        query = GetQueryFiltered(filter, query);
        query = query.Where(w => w.OrderCategoryId != null && w.Category.Name == OrderCategoryConstants.WithoutCoverage || w.Category.Name == OrderCategoryConstants.RepeatedOrder); // no muestra express ni tradicional, se puede cambiar a busacr por la cosntante mejor

        return await query
        .GroupBy(o => new { o.OrderCategoryId, o.Category.Name })
        .Select(g => new OrderSummary
        {
            Id = g.Key.OrderCategoryId!.Value,
            Name = g.Key.Name!,
            Count = g.Count()
        })
        .ToListAsync();
    }

    public async Task<Order?> GetByCodeAsync(string code, int storeId)
    {
        return await _context.Orders.FirstOrDefaultAsync(w => w.Code.Equals(code) && w.StoreId.Equals(storeId));
    }

    public async Task<List<Order>> GetByPhonesAsync(List<string> phones)
    {
        return await _context
            .Orders.Where(w => w.Client != null && !string.IsNullOrEmpty(w.Client.Phone) && phones.Contains(w.Client.Phone))
        .Include(w => w.Client)
        .ToListAsync();
    }
}