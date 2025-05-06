// BoxExpress.Api/Controllers/OrdersController.cs
using BoxExpress.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using BoxExpress.Application.Dtos;
using System.Linq;

namespace BoxExpress.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BanksController : ControllerBase
{
    private readonly IBankService _BankService;

    public BanksController(IBankService BankService)
    {
        _BankService = BankService;
    }

    [HttpPost("search")]
    public async Task<IActionResult> Search([FromBody] BankFilterDto filter)
    {
        var result = await _BankService.GetAllAsync(filter);
        return Ok(result);
    }
}
