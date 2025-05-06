// BoxExpress.Api/Controllers/OrdersController.cs
using BoxExpress.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using BoxExpress.Application.Dtos;
using System.Linq;

namespace BoxExpress.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WithdrawalRequestController : ControllerBase
{
    private readonly IWithdrawalRequestService _withdrawalRequestService;

    public WithdrawalRequestController(IWithdrawalRequestService withdrawalRequestService)
    {
        _withdrawalRequestService = withdrawalRequestService;
    }

    [HttpPost("search")]
    public async Task<IActionResult> Search([FromBody] WithdrawalRequestFilterDto filter)
    {
        var result = await _withdrawalRequestService.GetAllAsync(filter);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] WithdrawalRequestCreateDto dto)
    {
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
