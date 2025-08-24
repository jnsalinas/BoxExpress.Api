using BoxExpress.Domain.Entities;
using BoxExpress.Domain.Interfaces;
using BoxExpress.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using BoxExpress.Domain.Filters;

namespace BoxExpress.Infrastructure.Repositories;

public class StoreRepository : Repository<Store>, IStoreRepository
{
    private readonly BoxExpressDbContext _context;

    public StoreRepository(BoxExpressDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<Store?> GetByIdWithDetailsAsync(int storeId)
    {
        return await _context.Stores.Include(x => x.Wallet)
        .Include(x => x.Country)
                        .FirstOrDefaultAsync(x => x.Id == storeId);

    }

    public async Task<(List<Store> Stores, int TotalCount)> GetFilteredAsync(StoreFilter filter)
    {
        var query = _context.Stores.AsQueryable();
        var totalCount = await query.CountAsync();

        var storesQuery = query
       .Include(w => w.Wallet)
       .Include(w => w.Country)
       .Include(w => w.City)
       .OrderByDescending(w => w.CreatedAt)
       .AsQueryable();

        if (filter.StoreId.HasValue)
        {
            storesQuery = storesQuery.Where(x => x.Id == filter.StoreId.Value);
        }

        if (!string.IsNullOrEmpty(filter.Name))
        {
            storesQuery = storesQuery.Where(x => x.Name.ToLower().Contains(filter.Name.ToLower()));
        }

        if (!string.IsNullOrEmpty(filter.ShopifyAccessToken))
        {
            storesQuery = storesQuery.Where(x => x.ShopifyAccessToken == filter.ShopifyAccessToken);
        }

        if (!string.IsNullOrEmpty(filter.ShopifyShopDomain))
        {
            storesQuery = storesQuery.Where(x => x.ShopifyShopDomain == filter.ShopifyShopDomain);
        }

        if (!filter.IsAll)
        {
            storesQuery = storesQuery
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize);
        }

        var stores = await storesQuery.ToListAsync();
        return (stores, totalCount);
    }

    public async Task<Store?> GetBalanceSummary()
    {
        var walletSummary = await _context.Wallets
            .GroupBy(w => 1) // Agrupar todo en un solo grupo
            .Select(g => new
            {
                TotalBalance = g.Sum(w => w.Balance),
                TotalPending = g.Sum(w => w.PendingWithdrawals),
            })
            .FirstOrDefaultAsync();

        return new Store
        {
            Id = 0,
            Name = "Resumen Global",
            Wallet = new Wallet
            {
                Balance = walletSummary?.TotalBalance ?? 0,
                PendingWithdrawals = walletSummary?.TotalPending ?? 0,
            }
        };

    }
}