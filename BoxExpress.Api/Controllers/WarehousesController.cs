// BoxExpress.Api/Controllers/OrdersController.cs
using BoxExpress.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using BoxExpress.Application.Dtos;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace BoxExpress.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WarehousesController : ControllerBase
{
    private readonly IWarehouseService _warehouseService;
    private readonly IWarehouseInventoryTransferService _warehouseInventoryTransferService;

    private readonly IExcelExporter<WarehouseDto> _excelExporter;

    public WarehousesController(
        IWarehouseService warehouseService,
        IExcelExporter<WarehouseDto> excelExporter,
        IWarehouseInventoryTransferService warehouseInventoryTransferService)
    {
        _excelExporter = excelExporter;
        _warehouseService = warehouseService;
        _warehouseInventoryTransferService = warehouseInventoryTransferService;
    }

    [HttpPost("search")]
    public async Task<IActionResult> Search([FromBody] WarehouseFilterDto filter)
    {
        var role = User.FindFirst(ClaimTypes.Role)?.Value;
        if (role?.ToLower() == "bodega")
        {
            filter.Id = int.Parse(User.FindFirst("WarehouseId")?.Value ?? "0"); 
        }

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

    [HttpPost("{warehouseId}/transfers")]
    public async Task<IActionResult> CreateTransferAsync(int warehouseId, [FromBody] WarehouseInventoryTransferDto transferDto)
    {
        transferDto.FromWarehouseId = warehouseId;
        var result = await _warehouseInventoryTransferService.CreateTransferAsync(transferDto);
        return Ok(result);
    }
}
