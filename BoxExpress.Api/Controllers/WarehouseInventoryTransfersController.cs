// BoxExpress.Api/Controllers/OrdersController.cs
using BoxExpress.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using BoxExpress.Application.Dtos;
using System.Linq;
using System.Security.Claims;
using BoxExpress.Domain.Constants;

namespace BoxExpress.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WarehouseInventoryTransfersController : ControllerBase
{
    private readonly IWarehouseInventoryTransferService _warehouseService;
    private readonly IExcelExporter<WarehouseInventoryTransferDto> _excelExporter;

    public WarehouseInventoryTransfersController(IWarehouseInventoryTransferService warehouseService, IExcelExporter<WarehouseInventoryTransferDto> excelStatusExporter)
    {
        _warehouseService = warehouseService;
        _excelExporter = excelStatusExporter;

    }

    [HttpPost("search")]
    public async Task<IActionResult> Search([FromBody] WarehouseInventoryTransferFilterDto filter)
    {
        var role = User.FindFirst(ClaimTypes.Role)?.Value;
        if (role?.ToLower() == RolConstants.Warehouse)
        {
            var warehouseId = User.FindFirst("WarehouseId")?.Value;
            if (warehouseId == null)
            {
                return BadRequest("Warehouse is required for warehouse role.");
            }
            filter.ToWarehouseId = int.Parse(warehouseId);
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

    [HttpPost("export")]
    public async Task<IActionResult> ExportToExcel([FromBody] WarehouseInventoryTransferFilterDto filter)
    {
        filter.IsAll = true;
        var result = await _warehouseService.GetAllAsync(filter);
        if (result.Data == null || !result.Data.Any())
        {
            return NotFound("No data found to export.");
        }

        var bytes = _excelExporter.ExportToExcel(result.Data.ToList());
        return File(
            bytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"InventoryTransfer_{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx"
        );
    }

    [HttpPost("pending-transfers")]
    public async Task<IActionResult> GetPendingTransfers(WarehouseInventoryTransferFilterDto filter)
    {
        var role = User.FindFirst(ClaimTypes.Role)?.Value;
        if (role?.ToLower() == RolConstants.Warehouse)
        {
            var warehouseId = User.FindFirst("WarehouseId")?.Value;
            if (warehouseId == null)
            {
                return BadRequest("Warehouse is required for warehouse role.");
            }
            filter.ToWarehouseId = int.Parse(warehouseId);
        }

        var result = await _warehouseService.GetPendingTransfersAsync(filter);
        return Ok(result);
    }

}

