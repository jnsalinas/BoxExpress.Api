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
public class WalletTransactionsController : ControllerBase
{
    private readonly IWalletTransactionService _walletTransactionService;
    private readonly IExcelExporter<WalletTransactionDto> _excelExporter;

    public WalletTransactionsController(IWalletTransactionService walletTransactionService, IExcelExporter<WalletTransactionDto> excelExporter)
    {
        _excelExporter = excelExporter;
        _walletTransactionService = walletTransactionService;
    }

    [HttpPost("search")]
    public async Task<IActionResult> Search([FromBody] WalletTransactionFilterDto filter)
    {
        var role = User.FindFirst(ClaimTypes.Role)?.Value;
        if (role?.ToLower() == RolConstants.Store)
        {
            var storeIdvalue = User.FindFirst("StoreId")?.Value;
            if (storeIdvalue == null)
            {
                return BadRequest("StoreId is required for warehouse role.");
            }

            filter.StoreId = int.Parse(storeIdvalue);
        }

        var result = await _walletTransactionService.GetAllAsync(filter);
        return Ok(result);
    }

    [HttpPost("export")]
    public async Task<IActionResult> ExportToExcel([FromBody] WalletTransactionFilterDto filter)
    {
        var role = User.FindFirst(ClaimTypes.Role)?.Value;
        if (role?.ToLower() == RolConstants.Store)
        {
            var storeIdvalue = User.FindFirst("StoreId")?.Value;
            if (storeIdvalue == null)
            {
                return BadRequest("StoreId is required for warehouse role.");
            }

            filter.StoreId = int.Parse(storeIdvalue);
        }

        filter.IsAll = true;
        var result = await _walletTransactionService.GetAllAsync(filter);
        var bytes = _excelExporter.ExportToExcel(result?.Data?.ToList() ?? new List<WalletTransactionDto>());
        return File(
            bytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"WalletTransaction_{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx"
        );
    }
}
