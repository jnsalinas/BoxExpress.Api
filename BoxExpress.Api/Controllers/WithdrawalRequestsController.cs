// BoxExpress.Api/Controllers/OrdersController.cs
using BoxExpress.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using BoxExpress.Application.Dtos;
using System.Linq;
using System.Security.Claims;
using BoxExpress.Domain.Constants;
using Microsoft.AspNetCore.Http.HttpResults;

namespace BoxExpress.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WithdrawalRequestsController : ControllerBase
{
    private readonly IWithdrawalRequestService _withdrawalRequestService;

    public WithdrawalRequestsController(IWithdrawalRequestService withdrawalRequestService)
    {
        _withdrawalRequestService = withdrawalRequestService;
    }

    [HttpPost("search")]
    public async Task<IActionResult> Search([FromBody] WithdrawalRequestFilterDto filter)
    {
        var role = User.FindFirst(ClaimTypes.Role)?.Value;
        if (role?.ToLower() == RolConstants.Store)
        {
            filter.StoreId = int.Parse(User.FindFirst("StoreId")?.Value ?? "0"); 
        }

        var result = await _withdrawalRequestService.GetAllAsync(filter);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] WithdrawalRequestCreateDto dto)
    {
        var role = User.FindFirst(ClaimTypes.Role)?.Value;
        if (role?.ToLower() == RolConstants.Store)
        {
            var storeIdValue = User.FindFirst("StoreId")?.Value;
            if (string.IsNullOrEmpty(storeIdValue))
            {
                return BadRequest("StoreId is required for store users");
            }
            
            dto.StoreId = int.Parse(storeIdValue);
        }

        var result = await _withdrawalRequestService.AddAsync(dto);
        return Ok(result);
    }

    [HttpPost("{withdrawalrequestid}/approve")]
    public async Task<IActionResult> ApproveAsync(int withdrawalrequestid, [FromBody] WithdrawalRequestApproveDto warehouseInventoryTransferRejectDto)
    {
        var result = await _withdrawalRequestService.ApproveAsync(withdrawalrequestid, 2, warehouseInventoryTransferRejectDto);
        return Ok(result);
    }

    [HttpPost("{withdrawalrequestid}/reject")]
    public async Task<IActionResult> RejectAsync(int withdrawalrequestid, [FromBody] WithdrawalRequestRejectDto warehouseInventoryTransferRejectDto)
    {
        var result = await _withdrawalRequestService.RejectAsync(withdrawalrequestid, 2, warehouseInventoryTransferRejectDto);
        return Ok(result);
    }
}
