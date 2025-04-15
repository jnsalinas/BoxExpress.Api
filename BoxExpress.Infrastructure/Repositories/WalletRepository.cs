using BoxExpress.Domain.Entities;
using BoxExpress.Domain.Interfaces;
using BoxExpress.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using BoxExpress.Domain.Filters;

namespace BoxExpress.Infrastructure.Repositories;

public class WalletRepository : Repository<Wallet>, IWalletRepository
{
    private readonly BoxExpressDbContext _context;

    public WalletRepository(BoxExpressDbContext context) : base(context)
    {
        _context = context;
    }
}