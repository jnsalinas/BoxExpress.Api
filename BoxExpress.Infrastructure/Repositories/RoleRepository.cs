using BoxExpress.Domain.Entities;
using BoxExpress.Domain.Interfaces;
using BoxExpress.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using BoxExpress.Domain.Filters;

namespace BoxExpress.Infrastructure.Repositories;

public class RoleRepository : Repository<Role>, IRoleRepository
{
    private readonly BoxExpressDbContext _context;

    public RoleRepository(BoxExpressDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<Role?> GetByNameAsync(string roleName)
    {
        return await _context.Roles
            .FirstOrDefaultAsync(w => w.Name.ToLower().Equals(roleName.ToLower()));
    }
}