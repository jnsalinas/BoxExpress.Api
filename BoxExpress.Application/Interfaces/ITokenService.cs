using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BoxExpress.Application.Dtos;

namespace BoxExpress.Application.Interfaces
{
    public interface ITokenService
    {
        AuthResponseDto CreateToken(string userId, string email, IEnumerable<Claim>? extraClaims = null);

    }
}
