using Microsoft.EntityFrameworkCore;
using BoxExpress.Domain.Entities;
using BoxExpress.Domain.Interfaces;
using BoxExpress.Infrastructure.Persistence;

namespace BoxExpress.Infrastructure.Repositories;

public class ProductLoanDetailRepository : Repository<ProductLoanDetail>, IProductLoanDetailRepository
{
    private readonly BoxExpressDbContext _context;

    public ProductLoanDetailRepository(BoxExpressDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ProductLoanDetail>> GetByProductLoanIdAsync(int productLoanId)
    {
        return await _context.ProductLoanDetails
            .Include(x => x.ProductVariant)
                .ThenInclude(pv => pv.Product)
            .Where(x => x.ProductLoanId == productLoanId)
            .ToListAsync();
    }
} 