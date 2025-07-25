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

    public int UserId
    {
        get
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null)
                throw new Exception("No hay usuario autenticado en el contexto actual.");
            var claim = user.FindFirst(ClaimTypes.NameIdentifier) ?? user.FindFirst("sub");
            if (claim == null)
                throw new Exception("No se encontró el claim de UserId en el usuario actual.");
            return int.Parse(claim.Value);
        }
    }
} 