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
    public class ClientRepository : Repository<Client>, IClientRepository
    {
        private readonly BoxExpressDbContext _context;
        public ClientRepository(BoxExpressDbContext context) : base(context)
        {
            _context = context;
        }

        public new async Task<List<Client>> GetAllAsync()
        {
            return await _context.Set<Client>()
                .Include(c => c.Addresses)
                .ToListAsync();
        }

        public async Task<Client?> GetByDocumentAsync(string document)
        {
            return await _context.Set<Client>()
                .Include(c => c.Addresses)
                .FirstOrDefaultAsync(c => c.Document == document);
        }
    }
}
