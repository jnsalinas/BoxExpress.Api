// BoxExpress.Api/Controllers/OrdersController.cs
using BoxExpress.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using BoxExpress.Application.Dtos;

namespace BoxExpress.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WarehousesController : ControllerBase
{
    private readonly IWarehouseService _warehouseService;

    public WarehousesController(IWarehouseService warehouseService)
    {
        _warehouseService = warehouseService;
    }

    [HttpPost("search")]
    public async Task<IActionResult> Search([FromBody] WarehouseFilterDto filter)
    {
        var result = await _warehouseService.GetAllAsync(filter);
        return Ok(result);
    }

    // Si también quieres obtener por ID
    // [HttpGet("{id}")]
    // public async Task<IActionResult> GetById(int id)
    // {
    //     var result = await _warehouseService.GetByIdAsync(id); // Necesitarías tener este método en el servicio
    //     if (result == null) return NotFound();
    //     return Ok(result);
    // }
}
