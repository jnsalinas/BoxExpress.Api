// BoxExpress.Api/Controllers/OrdersController.cs
using BoxExpress.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using BoxExpress.Application.Dtos;
using System.Linq;

namespace BoxExpress.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InventoryMovementsController : ControllerBase
{
    private readonly IInventoryMovementService _inventoryMovementService;
    
    public InventoryMovementsController(IInventoryMovementService inventoryMovementService)
    {
        _inventoryMovementService = inventoryMovementService;
    }

    [HttpPost("search")]
    public async Task<IActionResult> Search([FromBody] InventoryMovementFilterDto filter)
    {
        var result = await _inventoryMovementService.GetAllAsync(filter);
        return Ok(result);
    }
}
