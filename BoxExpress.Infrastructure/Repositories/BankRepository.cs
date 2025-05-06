using BoxExpress.Domain.Entities;
using BoxExpress.Domain.Interfaces;
using BoxExpress.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using BoxExpress.Domain.Filters;

namespace BoxExpress.Infrastructure.Repositories;

public class BankRepository : Repository<Bank>, IBankRepository
{
    private readonly BoxExpressDbContext _context;

    public BankRepository(BoxExpressDbContext context) : base(context)
    {
        _context = context;
    }
}