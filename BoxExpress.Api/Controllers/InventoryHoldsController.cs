// BoxExpress.Api/Controllers/OrdersController.cs
using BoxExpress.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using BoxExpress.Application.Dtos;
using System.Linq;

namespace BoxExpress.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InventoryHoldsController : ControllerBase
{
    private readonly IInventoryHoldService _inventoryHoldservice;

    public InventoryHoldsController(IInventoryHoldService inventoryHoldservice)
    {
        _inventoryHoldservice = inventoryHoldservice;
    }

    [HttpPost("search")]
    public async Task<IActionResult> Search([FromBody] InventoryHoldFilterDto filter)
    {
        var result = await _inventoryHoldservice.GetAllAsync(filter);
        return Ok(result);
    }
}
