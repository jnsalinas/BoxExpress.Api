// BoxExpress.Api/Controllers/OrdersController.cs
using BoxExpress.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using BoxExpress.Application.Dtos;
using System.Linq;

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

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _warehouseService.GetByIdAsync(id);
        if (result.Equals(null)) return NotFound();
        return Ok(result);
    }

    [HttpPost("{warehouseId}/inventory")]
    public async Task<IActionResult> AddInventoryToWarehouse(int warehouseId, [FromBody] List<CreateProductWithVariantsDto> products)
    {
        var result = await _warehouseService.AddInventoryToWarehouseAsync(warehouseId, products);
        if (result.Equals(null)) return NotFound();
        return Ok(result);
    }

    [HttpPost("transfer")]
    public async Task<IActionResult> TransferInventory([FromBody] WarehouseInventoryTransferDto transferDto)
    {
        var result = await _warehouseService.TransferInventoryAsync(transferDto);
        if (!result.Data)
            return BadRequest(result);

        return Ok(result);
    }

}
