// BoxExpress.Api/Controllers/OrdersController.cs
using BoxExpress.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using BoxExpress.Application.Dtos;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace BoxExpress.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StoresController : ControllerBase
{
    private readonly IStoreService _storeService;

    public StoresController(IStoreService storeService)
    {
        _storeService = storeService;
    }

    [HttpPost("search")]
    public async Task<IActionResult> Search([FromBody] StoreFilterDto filter)
    {
        var result = await _storeService.GetAllAsync(filter);
        return Ok(result);
    }
    
    [Authorize(Roles = "Administrador")]
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateStoreDto createStoreDto)
    {
        var result = await _storeService.AddStoreAsync(createStoreDto);
        return Ok(result);
    }
}
