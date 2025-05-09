using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoxExpress.Domain.Entities;
using BoxExpress.Domain.Interfaces;
using BoxExpress.Infrastructure.Persistence;

namespace BoxExpress.Infrastructure.Repositories
{
    public class CityRepository : Repository<City>, ICityRepository
    {
        private readonly BoxExpressDbContext _context;
        public CityRepository(BoxExpressDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
