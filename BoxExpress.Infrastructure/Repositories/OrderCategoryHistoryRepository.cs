using BoxExpress.Domain.Entities;
using BoxExpress.Domain.Interfaces;
using BoxExpress.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using BoxExpress.Domain.Filters;

namespace BoxExpress.Infrastructure.Repositories;

public class OrderCategoryHistoryRepository : Repository<OrderCategoryHistory>, IOrderCategoryHistoryRepository
{
    private readonly BoxExpressDbContext _context;

    public OrderCategoryHistoryRepository(BoxExpressDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<List<OrderCategoryHistory>> GetByOrderIdAsync(int orderId)
    {
        return await _context.OrderCategoryHistories
        .Where(w => w.OrderId.Equals(orderId))
        .Include(x => x.OldCategory)
        .Include(x => x.NewCategory)
        .Include(x => x.Creator)
        .OrderByDescending(x => x.CreatedAt)
        .ToListAsync();
    }
}