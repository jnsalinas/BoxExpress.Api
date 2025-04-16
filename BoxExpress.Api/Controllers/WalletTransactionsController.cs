// BoxExpress.Api/Controllers/OrdersController.cs
using BoxExpress.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using BoxExpress.Application.Dtos;
using System.Linq;

namespace BoxExpress.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WalletTransactionsController : ControllerBase
{
    private readonly IWalletTransactionService _walletTransactionService;

    public WalletTransactionsController(IWalletTransactionService walletTransactionService)
    {
        _walletTransactionService = walletTransactionService;
    }

    [HttpPost("search")]
    public async Task<IActionResult> Search([FromBody] WalletTransactionFilterDto filter)
    {
        var result = await _walletTransactionService.GetAllAsync(filter);
        return Ok(result);
    }
}
