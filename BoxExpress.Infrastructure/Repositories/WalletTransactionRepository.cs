using BoxExpress.Domain.Entities;
using BoxExpress.Domain.Interfaces;
using BoxExpress.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using BoxExpress.Domain.Filters;

namespace BoxExpress.Infrastructure.Repositories;

public class WalletTransactionRepository : Repository<WalletTransaction>, IWalletTransactionRepository
{
    private readonly BoxExpressDbContext _context;

    public WalletTransactionRepository(BoxExpressDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<(List<WalletTransaction> Transactions, int TotalCount)> GetFilteredAsync(WalletTransactionFilter filter)
    {
        var query = _context.WalletTransactions.AsQueryable();
        if (filter.StoreId.HasValue)
        {
            query = query.Where(w => w.Wallet.Store.Id == filter.StoreId.Value);
        }
        if (filter.OrderId.HasValue)
        {
            query = query.Where(w => w.RelatedOrderId == filter.OrderId.Value);
        }
        if (filter.StartDate.HasValue)
        {
            query = query.Where(w => w.CreatedAt >= filter.StartDate.Value);
        }
        if (filter.EndDate.HasValue)
        {
            query = query.Where(w => w.CreatedAt <= filter.EndDate.Value);
        }
        if (filter.CountryId.HasValue)
        {
            query = query.Where(w => w.Wallet.Store.CountryId == filter.CountryId.Value);
        }

        var totalCount = await query.CountAsync();

        var transactionsQuery = query
            .Include(w => w.OrderStatus)
            .Include(w => w.TransactionType)
            .Include(w => w.Wallet)
                .ThenInclude(w => w.Store)
            .Include(w => w.Creator)
            .OrderByDescending(w => w.CreatedAt)
       .AsQueryable();

        if (!filter.IsAll)
        {
            transactionsQuery = transactionsQuery
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize);
        }

        var transactions = await transactionsQuery.ToListAsync();

        return (transactions, totalCount);
    }
}