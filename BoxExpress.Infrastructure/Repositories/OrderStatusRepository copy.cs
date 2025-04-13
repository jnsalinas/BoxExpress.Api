using BoxExpress.Domain.Entities;
using BoxExpress.Domain.Interfaces;
using BoxExpress.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using BoxExpress.Domain.Filters;

namespace BoxExpress.Infrastructure.Repositories;

public class OrderCategoryRepository : Repository<OrderCategory>, IOrderCategoryRepository
{
    private readonly BoxExpressDbContext _context;

    public OrderCategoryRepository(BoxExpressDbContext context) : base(context)
    {
        _context = context;
    }
}