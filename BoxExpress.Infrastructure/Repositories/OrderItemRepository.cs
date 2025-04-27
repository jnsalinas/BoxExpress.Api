using BoxExpress.Domain.Entities;
using BoxExpress.Domain.Interfaces;
using BoxExpress.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using BoxExpress.Domain.Filters;

namespace BoxExpress.Infrastructure.Repositories;

public class OrderItemRepository : Repository<OrderItem>, IOrderItemRepository
{
    private readonly BoxExpressDbContext _context;

    public OrderItemRepository(BoxExpressDbContext context) : base(context)
    {
        _context = context;
    }

     public async Task<List<OrderItem>> GetByOrderIdAsync(int orderId)
    {
        return await _context.OrderItems
        .Where(w => w.OrderId.Equals(orderId))
        .Include(x => x.ProductVariant)
            .ThenInclude(x => x.Product)
        .ToListAsync();
    }
}