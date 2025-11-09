using Microsoft.EntityFrameworkCore;
using BoxExpress.Domain.Entities;
using BoxExpress.Domain.Filters;
using BoxExpress.Domain.Interfaces;
using BoxExpress.Infrastructure.Persistence;

namespace BoxExpress.Infrastructure.Repositories;

public class ProductLoanRepository : Repository<ProductLoan>, IProductLoanRepository
{
    private readonly BoxExpressDbContext _context;

    public ProductLoanRepository(BoxExpressDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ProductLoan>> GetFilteredAsync(ProductLoanFilter filter)
    {
        var query = _context.ProductLoans
            .Include(x => x.Warehouse)
            .Include(x => x.CreatedBy)
            .Include(x => x.ProcessedBy)
            .Include(x => x.Details)
                .ThenInclude(d => d.ProductVariant)
                    .ThenInclude(pv => pv.Product)
            .AsQueryable();

        if (filter.WarehouseId.HasValue)
            query = query.Where(x => x.WarehouseId == filter.WarehouseId.Value);

        if (filter.Status.HasValue)
            query = query.Where(x => x.Status == filter.Status.Value);

        if (!string.IsNullOrWhiteSpace(filter.ResponsibleName))
            query = query.Where(x => x.ResponsibleName.Contains(filter.ResponsibleName));

        if (filter.FromDate.HasValue)
            query = query.Where(x => x.LoanDate >= filter.FromDate.Value);

        if (filter.ToDate.HasValue)
            query = query.Where(x => x.LoanDate <= filter.ToDate.Value);

        if (filter.CreatedById.HasValue)
            query = query.Where(x => x.CreatedById == filter.CreatedById.Value);

        if (filter.CountryId.HasValue)
            query = query.Where(x => x.Warehouse.CountryId == filter.CountryId.Value);

        query = query.OrderByDescending(x => x.CreatedAt);

        if (filter.Page > 0 && filter.PageSize > 0)
        {
            query = query.Skip((filter.Page - 1) * filter.PageSize)
                        .Take(filter.PageSize);
        }

        return await query.ToListAsync();
    }

    public async Task<ProductLoan?> GetByIdWithDetailsAsync(int id)
    {
        return await _context.ProductLoans
            .Include(x => x.Warehouse)
            .Include(x => x.CreatedBy)
            .Include(x => x.ProcessedBy)
            .Include(x => x.Details)
                .ThenInclude(d => d.ProductVariant)
                    .ThenInclude(pv => pv.Product)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<int> GetTotalCountAsync(ProductLoanFilter filter)
    {
        var query = _context.ProductLoans.AsQueryable();

        if (filter.WarehouseId.HasValue)
            query = query.Where(x => x.WarehouseId == filter.WarehouseId.Value);

        if (filter.Status.HasValue)
            query = query.Where(x => x.Status == filter.Status.Value);

        if (!string.IsNullOrWhiteSpace(filter.ResponsibleName))
            query = query.Where(x => x.ResponsibleName.Contains(filter.ResponsibleName));

        if (filter.FromDate.HasValue)
            query = query.Where(x => x.LoanDate >= filter.FromDate.Value);

        if (filter.ToDate.HasValue)
            query = query.Where(x => x.LoanDate <= filter.ToDate.Value);

        if (filter.CreatedById.HasValue)
            query = query.Where(x => x.CreatedById == filter.CreatedById.Value);

        return await query.CountAsync();
    }
} 