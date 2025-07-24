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
    public class ClientAddressRepository : Repository<ClientAddress>, IClientAddressRepository
    {
        private readonly BoxExpressDbContext _context;
        public ClientAddressRepository(BoxExpressDbContext context) : base(context)
        {
            _context = context;
        }

        public new async Task<ClientAddress?> GetByIdAsync(int id)
        {
            return await _context.Set<ClientAddress>()
                .Include(c => c.City)
                .FirstOrDefaultAsync(x => x.Id.Equals(id));
        }

        public async Task<ClientAddress?> GetByClientIdAsync(int clientId, string address)
        {
            return await _context.Set<ClientAddress>()
                .Include(c => c.City)
                .FirstOrDefaultAsync(x => x.ClientId.Equals(clientId) && x.Address.Equals(address));
        }

    }
}
