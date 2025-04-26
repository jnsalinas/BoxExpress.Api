using BoxExpress.Application.Dtos;
using BoxExpress.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BoxExpress.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }
    
    [Authorize(Roles = "Administrador")]
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateUserDto createUserDto)
    {
        var result = await _userService.AddUserAsync(createUserDto);
        return Ok(result);
    }
}