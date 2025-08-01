// BoxExpress.Api/Controllers/OrdersController.cs
using BoxExpress.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using BoxExpress.Application.Dtos;
using System.Linq;
using System.Security.Claims;
using BoxExpress.Domain.Constants;
using Microsoft.AspNetCore.Authorization;

namespace BoxExpress.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WarehouseInventoriesController : ControllerBase
{
    private readonly IExcelExporter<ProductDto> _excelExporter;
    private readonly IWarehouseInventoryService _service;

    public WarehouseInventoriesController(IWarehouseInventoryService service, IExcelExporter<ProductDto> excelExporter)
    {
        _service = service;
        _excelExporter = excelExporter;
    }

    [HttpPost("search")]
    public async Task<IActionResult> Search([FromBody] WarehouseInventoryFilterDto filter)
    {
        var role = User.FindFirst(ClaimTypes.Role)?.Value;
        if (role?.ToLower() == RolConstants.Store)
        {
            var storeId = User.FindFirst("StoreId")?.Value;
            if (storeId == null)
            {
                return BadRequest("StoreId is required for store role.");
            }

            filter.StoreId = int.Parse(storeId);

            return Ok(await _service.GetWarehouseProductSummaryGroupAsync(filter));
        }
        else if (role?.ToLower() == RolConstants.Warehouse)
        {
            var warehouseId = User.FindFirst("WarehouseId")?.Value;
            if (warehouseId == null)
            {
                return BadRequest("WarehouseId is required for store role.");
            }

            filter.WarehouseId = int.Parse(warehouseId);

        }
        var result = await _service.GetWarehouseProductSummaryAsync(filter);
        return Ok(result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateWarehouseInventoryDto dto)
    {
        var result = await _service.UpdateAsync(id, dto);
        return Ok(result);
    }


    [HttpGet("autocomplete")]
    public async Task<IActionResult> Autocomplete([FromQuery] string query, [FromQuery] int warehouseOriginId)

    {
        if (string.IsNullOrWhiteSpace(query))
            return BadRequest("Query is required.");

        var result = await _service.GetVariantsAutocompleteAsync(query, warehouseOriginId);
        return Ok(result);
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result.Equals(null)) return NotFound();
        return Ok(result);
    }


    [HttpPost("export")]
    public async Task<IActionResult> ExportToExcel()
    {
        var filter = new WarehouseInventoryFilterDto();
        filter.IsAll = true;
        var role = User.FindFirst(ClaimTypes.Role)?.Value;
        if (role?.ToLower() == RolConstants.Store)
        {
            var storeId = User.FindFirst("StoreId")?.Value;
            if (storeId == null)
            {
                return BadRequest("StoreId is required for store role.");
            }

            filter.StoreId = int.Parse(storeId);
        }

        var result = await _service.GetWarehouseProductSummaryAsync(filter);
        var bytes = _excelExporter.ExportToExcel(result?.Data?.ToList() ?? new List<ProductDto>());
        return File(
            bytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"Orders_{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx"
        );
    }
}
