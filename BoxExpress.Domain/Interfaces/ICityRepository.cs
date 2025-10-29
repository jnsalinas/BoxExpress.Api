using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoxExpress.Domain.Entities;
using BoxExpress.Domain.Filters;

namespace BoxExpress.Domain.Interfaces
{
    public interface ICityRepository : IRepository<City>
    {
        Task<City?> GetByNameAsync(string name);
        Task<List<City>> GetFilteredAsync(CityFilter filter);
    }
}
