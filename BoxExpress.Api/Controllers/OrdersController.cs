// BoxExpress.Api/Controllers/OrdersController.cs
using System.Security.Claims;
using BoxExpress.Api.Dtos.Upload;
using BoxExpress.Application.Dtos;
using BoxExpress.Application.Dtos.Common;
using BoxExpress.Application.Interfaces;
using BoxExpress.Application.Services;
using BoxExpress.Domain.Constants;
using BoxExpress.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BoxExpress.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IExcelExporter<OrderDto> _excelExporter;
    private readonly IOrderService _orderService;
    private readonly IFileService _fileService;

    public OrdersController(
        IOrderService orderService,
        IExcelExporter<OrderDto> excelStatusExporter,
        IFileService fileService
    )
    {
        _orderService = orderService;
        _excelExporter = excelStatusExporter;
        _fileService = fileService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _orderService.GetByIdAsync(id);
        if (!result.IsSuccess)
            return NotFound(result.Message);
        return Ok(result);
    }

    [HttpPost("search")]
    public async Task<IActionResult> Search([FromBody] OrderFilterDto filter)
    {
        var role = User.FindFirst(ClaimTypes.Role)?.Value;
        if (role?.ToLower() == RolConstants.Warehouse)
        {
            filter.WarehouseId = int.Parse(User.FindFirst("WarehouseId")?.Value ?? "0");
        }
        else if (role?.ToLower() == RolConstants.Store)
        {
            var storeId = User.FindFirst("StoreId")?.Value;
            if (storeId == null)
            {
                return BadRequest("StoreId is required for store role.");
            }

            filter.StoreId = int.Parse(storeId);
        }

        var result = await _orderService.GetAllAsync(filter);
        return Ok(result);
    }

    [HttpPost("summary")]
    public async Task<IActionResult> Summary([FromBody] OrderFilterDto filter)
    {
        var role = User.FindFirst(ClaimTypes.Role)?.Value;
        if (role?.ToLower() == RolConstants.Warehouse)
        {
            filter.WarehouseId = int.Parse(User.FindFirst("WarehouseId")?.Value ?? "0");
        }
        else if (role?.ToLower() == RolConstants.Store)
        {
            var storeId = User.FindFirst("StoreId")?.Value;
            if (storeId == null)
            {
                return BadRequest("StoreId is required for store role.");
            }

            filter.StoreId = int.Parse(storeId);
        }

        var result = await _orderService.GetSummaryAsync(filter);
        return Ok(result);
    }

    [HttpPatch("{orderId}/warehouse/{warehouseId}")]
    public async Task<IActionResult> UpdateWarehouse(int orderId, int warehouseId)
    {
        return Ok(await _orderService.UpdateWarehouseAsync(orderId, warehouseId));
    }

    [HttpPatch("{orderId}/status/{statusId}")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UpdateStatus(
        int orderId,
        int statusId,
        [FromForm] ChangeStatusDto changeStatusDto
    )
    {
        return Ok(await _orderService.UpdateStatusAsync(orderId, statusId, changeStatusDto));
    }

    [HttpPatch("{orderId}/schedule")]
    public async Task<IActionResult> UpdateSchedule(
        int orderId,
        [FromBody] OrderScheduleUpdateDto dto
    )
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

    [HttpPost("export")]
    public async Task<IActionResult> ExportToExcel([FromBody] OrderFilterDto filter)
    {
        filter.IsAll = true;
        var role = User.FindFirst(ClaimTypes.Role)?.Value;
        if (role?.ToLower() == RolConstants.Warehouse)
        {
            filter.WarehouseId = int.Parse(User.FindFirst("WarehouseId")?.Value ?? "0");
        }
        else if (role?.ToLower() == RolConstants.Store)
        {
            var storeId = User.FindFirst("StoreId")?.Value;
            if (storeId == null)
            {
                return BadRequest("StoreId is required for store role.");
            }

            filter.StoreId = int.Parse(storeId);
        }

        var result = await _orderService.GetAllAsync(filter);
        var bytes = _excelExporter.ExportToExcel(result?.Data?.ToList() ?? new List<OrderDto>());
        return File(
            bytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"Orders_{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx"
        );
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrderDto createOrderDto)
    {
        var result = await _orderService.AddOrderAsync(createOrderDto);
        return Ok(result);
    }

    [HttpPost("bulk-upload")]
    public async Task<IActionResult> BulkUpload([FromForm] UploadFileRequestDto dto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var role = User.FindFirst(ClaimTypes.Role)?.Value;
        int storeId = 0;
        if (role?.ToLower() == RolConstants.Store)
        {
            storeId = int.Parse(User.FindFirst("StoreId")?.Value ?? "0");
        }
        else
        {
            storeId = dto.StoreId ?? 0;
        }

        if (dto.File == null || dto.File.Length == 0)
        {
            return BadRequest("No file uploaded.");
        }

        using var memoryStream = new MemoryStream();
        await dto.File.CopyToAsync(memoryStream);

        var fileDto = new OrderExcelUploadDto
        {
            FileName = dto.File.FileName,
            Content = memoryStream.ToArray(),
            ContentType = dto.File.ContentType,
            StoreId = storeId,
            CreatorId = userId != null ? int.Parse(userId) : 0,
        };

        var result = await _orderService.AddOrdersFromExcelAsync(fileDto);

        if (result.IsSuccess)
        {
            return Ok(result);
        }

        return BadRequest(result.Message ?? "Upload failed.");
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateOrderDto createOrderDto)
    {
        var result = await _orderService.UpdateOrderAsync(id, createOrderDto);
        return Ok(result);
    }

    [HttpPost("bulk-change-status")]
    public async Task<IActionResult> BulkChangeStatus([FromBody] BulkChangeOrdersStatusDto bulkChangeStatusDto)
    {
        var result = await _orderService.BulkChangeStatusAsync(bulkChangeStatusDto);
        return Ok(result);
    }

    // [HttpPost("delivery-provider")]
    // public async Task<IActionResult> CreateDeliveryProvider([FromBody] OrderDeliveryProviderDto orderDeliveryProviderDto)
    // {
    //     var result = await _orderService.AssignDeliveryProviderAsync(orderDeliveryProviderDto);
    //     return Ok(result);
    // }
}
