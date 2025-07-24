using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoxExpress.Domain.Entities;

namespace BoxExpress.Domain.Interfaces
{
    public interface IClientAddressRepository : IRepository<ClientAddress>
    {
        Task<ClientAddress?> GetByClientIdAsync(int clientId, string address);
    }
}
