using Microsoft.AspNetCore.Http;
using System.Security.Claims;
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
} 