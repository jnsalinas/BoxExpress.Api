using BoxExpress.Domain.Entities;
using BoxExpress.Domain.Interfaces;
using BoxExpress.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using BoxExpress.Domain.Filters;
using DocumentFormat.OpenXml.Office2010.Excel;

namespace BoxExpress.Infrastructure.Repositories;

public class WalletRepository : Repository<Wallet>, IWalletRepository
{
    private readonly BoxExpressDbContext _context;

    public WalletRepository(BoxExpressDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<Wallet?> GetByStoreIdAsync(int storeId)
    {
        return await _context.Wallets.FirstOrDefaultAsync(x => x.Store.Id == storeId);
    }
}