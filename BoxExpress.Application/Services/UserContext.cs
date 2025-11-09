using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using BoxExpress.Application.Interfaces;

namespace BoxExpress.Application.Services;

public class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public UserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
}
    public int? UserId
    {
        get
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null)
                return null;
            var claim = user.FindFirst(ClaimTypes.NameIdentifier) ?? user.FindFirst("sub");
            if (claim == null)
                return null;
            return int.Parse(claim.Value);
        }
    }

    public int? CountryId
    {
        get
        {
            var headers = _httpContextAccessor.HttpContext?.Request?.Headers;
            if (headers == null)
                return null;
            if (!headers.TryGetValue("X-Country-Id", out var values))
                return null;
            var value = values.ToString();
            if (string.IsNullOrWhiteSpace(value))
                return null;
            if (int.TryParse(value, out var countryId))
                return countryId;
            return null;
        }
    }
} 