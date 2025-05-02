using BoxExpress.Domain.Entities;
using BoxExpress.Domain.Interfaces;
using BoxExpress.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using BoxExpress.Domain.Filters;

namespace BoxExpress.Infrastructure.Repositories;

public class WithdrawalRequestRepository : Repository<WithdrawalRequest>, IWithdrawalRequestRepository
{
    private readonly BoxExpressDbContext _context;

    public WithdrawalRequestRepository(BoxExpressDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<(List<WithdrawalRequest> Transactions, int TotalCount)> GetFilteredAsync(WithdrawalRequestFilter filter)
    {
        var query = _context.WithdrawalRequests
            .Include(x => x.Store)
            .Include(x => x.Creator)
            .AsQueryable();

        if (filter.StoreId.HasValue && filter.StoreId > 0)
            query = query.Where(w => w.StoreId.Equals(filter.StoreId));

        int totalCount = query.Count();
        return (await query.ToListAsync(), totalCount);
    }
}