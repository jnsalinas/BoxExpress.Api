using BoxExpress.Domain.Entities;
using BoxExpress.Domain.Interfaces;
using BoxExpress.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using BoxExpress.Domain.Filters;

namespace BoxExpress.Infrastructure.Repositories;

public class ProductRepository : Repository<Product>, IProductRepository
{
    private readonly BoxExpressDbContext _context;

    public ProductRepository(BoxExpressDbContext context) : base(context)
    {
        _context = context;
    }

    public new async Task<List<Product>> GetAllAsync()
    {
        return await _context.Set<Product>()
            .Include(p => p.Variants)
            .ToListAsync();
    }
}