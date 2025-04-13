using BoxExpress.Domain.Entities;
using BoxExpress.Domain.Interfaces;
using BoxExpress.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using BoxExpress.Domain.Filters;

namespace BoxExpress.Infrastructure.Repositories;

public class OrderStatusRepository : Repository<OrderStatus>, IOrderStatusRepository
{
    private readonly BoxExpressDbContext _context;

    public OrderStatusRepository(BoxExpressDbContext context) : base(context)
    {
        _context = context;
    }
}