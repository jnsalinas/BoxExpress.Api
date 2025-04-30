using BoxExpress.Domain.Entities;
using BoxExpress.Domain.Interfaces;
using BoxExpress.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using BoxExpress.Domain.Filters;

namespace BoxExpress.Infrastructure.Repositories;

public class OrderStatusHistoryRepository : Repository<OrderStatusHistory>, IOrderStatusHistoryRepository
{
    private readonly BoxExpressDbContext _context;

    public OrderStatusHistoryRepository(BoxExpressDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<List<OrderStatusHistory>> GetByOrderIdAsync(int orderId)
    {
        return await _context.OrderStatusHistories
            .Where(w => w.OrderId.Equals(orderId))
            .Include(x => x.OldStatus)
            .Include(x => x.NewStatus)
            .Include(x => x.Creator)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

}