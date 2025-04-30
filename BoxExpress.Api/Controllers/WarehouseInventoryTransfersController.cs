// BoxExpress.Api/Controllers/OrdersController.cs
using BoxExpress.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using BoxExpress.Application.Dtos;
using System.Linq;

namespace BoxExpress.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WarehouseInventoryTransfersController : ControllerBase
{
    private readonly IWarehouseInventoryTransferService _warehouseService;

    public WarehouseInventoryTransfersController(IWarehouseInventoryTransferService warehouseService)
    {
        _warehouseService = warehouseService;
    }

    [HttpPost("search")]
    public async Task<IActionResult> Search([FromBody] WarehouseInventoryTransferFilterDto filter)
    {
        var result = await _warehouseService.GetAllAsync(filter);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _warehouseService.GetByIdAsync(id);
        if (result.Equals(null)) return NotFound();
        return Ok(result);
    }

    [HttpPost("{transferId}/accept")]
    public async Task<IActionResult> AcceptTransferAsync(int transferId)
    {
        var result = await _warehouseService.AcceptTransferAsync(transferId, 2);
        return Ok(result);
    }

    [HttpPost("{transferId}/reject")]
    public async Task<IActionResult> RejectTransferAsync(int transferId, [FromBody] WarehouseInventoryTransferRejectDto warehouseInventoryTransferRejectDto)
    {
        var result = await _warehouseService.RejectTransferAsync(transferId, 2, warehouseInventoryTransferRejectDto.Reason);
        return Ok(result);
    }
}

