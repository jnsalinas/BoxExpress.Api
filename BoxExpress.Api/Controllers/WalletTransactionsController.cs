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
    private readonly IExcelExporter<WalletTransactionDto> _excelExporter;

    public WalletTransactionsController(IWalletTransactionService walletTransactionService, IExcelExporter<WalletTransactionDto> excelExporter)
    {
        _excelExporter = excelExporter;
        _walletTransactionService = walletTransactionService;
    }

    [HttpPost("search")]
    public async Task<IActionResult> Search([FromBody] WalletTransactionFilterDto filter)
    {
        var result = await _walletTransactionService.GetAllAsync(filter);
        return Ok(result);
    }

     [HttpPost("export")]
    public async Task<IActionResult> ExportToExcel([FromBody] WalletTransactionFilterDto filter)
    {
        var result = await _walletTransactionService.GetAllAsync(filter);
        if (result.Data == null || !result.Data.Any())
        {
            return NotFound("No data found to export.");
        }

        var bytes = _excelExporter.ExportToExcel(result.Data.ToList());
        return File(
            bytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"Bodegas_{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx"
        );
    }
}
