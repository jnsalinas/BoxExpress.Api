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
    public class CityRepository : Repository<City>, ICityRepository
    {
        private readonly BoxExpressDbContext _context;
        public CityRepository(BoxExpressDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<City?> GetByNameAsync(string name)
        {
            return await _context.Cities.FirstOrDefaultAsync(x => x.Name.ToLower() == name.ToLower());
        }
    }
}
