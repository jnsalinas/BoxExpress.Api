using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoxExpress.Domain.Entities;
using BoxExpress.Domain.Interfaces;
using BoxExpress.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BoxExpress.Infrastructure.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        private readonly BoxExpressDbContext _context;

        public UserRepository(BoxExpressDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Set<User>()
                .AsNoTracking()
                .Include(w => w.Role)
                .Include(w => w.Warehouse)
                .FirstOrDefaultAsync(w => w.Email.ToLower().Equals(email.ToLower()));
        }

        public async Task<User?> GetByStoreIdAsync(int id)
        {
            return await _context.Set<User>()
                .AsNoTracking()
                .FirstOrDefaultAsync(w => w.StoreId == id);
        }
    }
}
