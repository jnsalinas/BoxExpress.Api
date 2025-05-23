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

    [HttpPost("acceptreturn")]
    public async Task<IActionResult> AcceptReturn([FromBody] InventoryHoldResolutionDto dto)
    {
        var result = await _inventoryHoldservice.AcceptReturnAsync(dto);
        return Ok(result);
    }

    [HttpPost("rejectreturn")]
    public async Task<IActionResult> RejectReturn([FromBody] InventoryHoldResolutionDto dto)
    {
        var result = await _inventoryHoldservice.RejectReturnAsync(dto);
        return Ok(result);
    }
}
