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
}