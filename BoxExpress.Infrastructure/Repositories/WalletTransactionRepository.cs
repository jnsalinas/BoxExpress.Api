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

    public async Task<List<WalletTransaction>> GetFilteredAsync(WalletTransactionFilter filter)
    {
        return await _context.WalletTransactions
            .Include(w => w.Wallet)
                .ThenInclude(w => w.Store)
            .Include(w => w.Creator)
            .ToListAsync();
    }
}