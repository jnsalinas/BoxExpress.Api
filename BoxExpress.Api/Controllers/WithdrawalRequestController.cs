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
}
