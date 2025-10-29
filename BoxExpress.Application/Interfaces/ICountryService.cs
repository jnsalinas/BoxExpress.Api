using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoxExpress.Application.Dtos.Common;
using BoxExpress.Application.Dtos;

namespace BoxExpress.Application.Interfaces
{
    public interface ICountryService
    {
        Task<ApiResponse<IEnumerable<CountryDto>>> GetAllAsync();

    }
}
