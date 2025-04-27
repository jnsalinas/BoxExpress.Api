// BoxExpress.Api/Controllers/OrdersController.cs
using BoxExpress.Application.Dtos;
using BoxExpress.Application.Interfaces;
using BoxExpress.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace BoxExpress.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    // private readonly IExcelExporter<OrderStatusHistoryDto> _excelStatusExporter;
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService
    //, IExcelExporter<OrderStatusHistoryDto> excelStatusExporter
    )
    {
        _orderService = orderService;
        // _excelStatusExporter = excelStatusExporter;
    }

    [HttpPost("search")]
    public async Task<IActionResult> Search([FromBody] OrderFilterDto filter)
    {
        var result = await _orderService.GetAllAsync(filter);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _orderService.GetByIdAsync(id);
        if (result.Equals(null)) return NotFound();
        return Ok(result);
    }

    [HttpPatch("{orderId}/warehouse/{warehouseId}")]
    public async Task<IActionResult> UpdateWarehouse(int orderId, int warehouseId)
    {
        return Ok(await _orderService.UpdateWarehouseAsync(orderId, warehouseId));
    }

    [HttpPatch("{orderId}/status/{statusId}")]
    public async Task<IActionResult> UpdateStatus(int orderId, int statusId)
    {
        return Ok(await _orderService.UpdateStatusAsync(orderId, statusId));
    }

    [HttpPatch("{orderId}/schedule")]
    public async Task<IActionResult> UpdateSchedule(int orderId, [FromBody] OrderScheduleUpdateDto dto)
    {
        var updated = await _orderService.UpdateScheduleAsync(orderId, dto);
        return Ok(updated);
    }

    [HttpGet("{orderId}/status-history")]
    public async Task<IActionResult> GetStatusHistory(int orderId)
    {
        var result = await _orderService.GetStatusHistoryAsync(orderId);
        return Ok(result);
    }

    [HttpGet("{orderId}/category-history")]
    public async Task<IActionResult> GetCategoryHistory(int orderId)
    {
        var result = await _orderService.GetCategoryHistoryAsync(orderId);
        return Ok(result);
    }

    [HttpGet("{orderId}/products")]
    public async Task<IActionResult> GetProducts(int orderId)
    {
        var result = await _orderService.GetProductsAsync(orderId);
        return Ok(result);
    }

    // [HttpGet("export/{orderId}")]
    // public async Task<IActionResult> ExportToExcel(int orderId)
    // {
    //     var result = await _orderService.GetStatusHistoryAsync(orderId);
    //     if (result.Data == null || !result.Data.Any())
    //     {
    //         return NotFound("No data found to export.");
    //     }

    //     var bytes = _excelStatusExporter.ExportToExcel(result.Data.ToList());
    //     return File(
    //         bytes,
    //         "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
    //         $"StatusHistory_{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx"
    //     );
    // }
}
