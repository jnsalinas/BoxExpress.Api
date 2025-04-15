using BoxExpress.Domain.Entities;
using BoxExpress.Domain.Interfaces;
using BoxExpress.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using BoxExpress.Domain.Filters;

namespace BoxExpress.Infrastructure.Repositories;

public class TransactionTypeRepository : Repository<TransactionType>, ITransactionTypeRepository
{
    private readonly BoxExpressDbContext _context;

    public TransactionTypeRepository(BoxExpressDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<TransactionType?> GetByNameAsync(string name)
    {
        return await _context.Set<TransactionType>().FirstOrDefaultAsync(w => w.Name.Equals(name));
    }
}