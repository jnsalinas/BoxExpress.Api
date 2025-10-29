using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BoxExpress.Application.Dtos;
using BoxExpress.Application.Dtos.Common;
using BoxExpress.Application.Interfaces;
using BoxExpress.Domain.Constants;
using BoxExpress.Domain.Interfaces;
using BoxExpress.Utilities;

namespace BoxExpress.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokens;

        public AuthService(
             IUserRepository userRepository,
             ITokenService tokens
            )
        {
            _userRepository = userRepository;
            _tokens = tokens;
        }
        public async Task<ApiResponse<AuthResponseDto>> AuthenticateAsync(LoginDto loginDto)
        {
            var user = await _userRepository.GetByEmailAsync(loginDto.UserName);
            if (user == null)
                return ApiResponse<AuthResponseDto>.Fail("User not found");

            if (!BcryptHelper.Verify(loginDto.Password, user.PasswordHash))
                return ApiResponse<AuthResponseDto>.Fail("Invalid credentials");

            var claimsExtras = new List<Claim>
            {
                new Claim(ClaimTypes.Role, user.Role.Name.ToLower()),
                new Claim(ClaimTypes.Country, user.CountryId.ToString()),
            };

            if (user.WarehouseId.HasValue)
            {
                claimsExtras.Add(new Claim("WarehouseId", user.WarehouseId.Value.ToString()));
                claimsExtras.Add(new Claim("WarehouseName", user.Warehouse?.Name ?? string.Empty));
            }

            if (user.StoreId.HasValue)
            {
                claimsExtras.Add(new Claim("StoreId", user.StoreId.Value.ToString()));
            }

            var authResponseDto = _tokens.CreateToken(user.Id.ToString(), user.Email, claimsExtras);
            return ApiResponse<AuthResponseDto>.Success(authResponseDto);
        }
    }

}
