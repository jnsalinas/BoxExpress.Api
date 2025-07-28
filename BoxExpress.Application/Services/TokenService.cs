using System;
using System.Security.Claims;
using System.Text;
using BoxExpress.Application.Configurations;
using BoxExpress.Application.Interfaces;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using BoxExpress.Application.Dtos;

namespace BoxExpress.Application.Services
{
    public class TokenService : ITokenService
    {
        private readonly JwtOptions _jwt;

        public TokenService(IOptions<JwtOptions> opts)
        {
            _jwt = opts.Value;
                          
        }

        public AuthResponseDto CreateToken(string userId, string email, IEnumerable<Claim>? extraClaims = null)
        {
            var now = DateTime.UtcNow;
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            var expires = now.AddMinutes(_jwt.ExpiryMinutes);

            var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub,   userId),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(JwtRegisteredClaimNames.Jti,   Guid.NewGuid().ToString()),
        };

            if (extraClaims != null)
                claims.AddRange(extraClaims);

            var token = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                notBefore: now,
                expires: expires,
                signingCredentials: creds
            );

            return new AuthResponseDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expires,
                Role = extraClaims?.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value,
                StoreId = extraClaims?.FirstOrDefault(c => c.Type == "StoreId")?.Value,
                WarehouseName = extraClaims?.FirstOrDefault(c => c.Type == "WarehouseName")?.Value,
            };
        }
    }
}
