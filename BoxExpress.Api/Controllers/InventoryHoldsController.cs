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
        var role = User.FindFirst(ClaimTypes.Role)?.Value;
        if (role?.ToLower() == RolConstants.Warehouse)
        {
            filter.WarehouseId = int.Parse(User.FindFirst("WarehouseId")?.Value ?? "0");
        }

        var result = await _inventoryHoldservice.GetAllAsync(filter);
        return Ok(result);
    }

    [HttpPost("acceptreturn")]
    public async Task<IActionResult> AcceptReturn([FromForm] InventoryHoldResolutionDto dto)
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

    [HttpPost("bulk-acceptreturn")]
    public async Task<IActionResult> BulkAcceptReturn([FromBody] InventoryHoldMassiveResolutionDto dto)
    {
        var result = await _inventoryHoldservice.BulkAcceptReturnAsync(dto.InventoryHoldResolutions);
        return Ok(result);
    }

    [HttpPost("bulk-rejectreturn")]
    public async Task<IActionResult> BulkRejectReturn([FromBody] InventoryHoldMassiveResolutionDto dto)
    {
        var result = await _inventoryHoldservice.BulkRejectReturnAsync(dto.InventoryHoldResolutions);
        return Ok(result);
    }
}
