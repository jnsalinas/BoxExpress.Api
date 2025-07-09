// BoxExpress.Api/Controllers/OrdersController.cs
using BoxExpress.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using BoxExpress.Application.Dtos;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using BoxExpress.Domain.Constants;

namespace BoxExpress.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StoresController : ControllerBase
{
    private readonly IStoreService _storeService;
    private readonly IExcelExporter<StoreDto> _excelExporter;

    public StoresController(IStoreService storeService, IExcelExporter<StoreDto> excelStatusExporter)
    {
        _storeService = storeService;
        _excelExporter = excelStatusExporter;
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var role = User.FindFirst(ClaimTypes.Role)?.Value;
        if (role?.ToLower() == RolConstants.Store)
        {
            var storeValue = User.FindFirst("StoreId")?.Value;
            if (string.IsNullOrEmpty(storeValue))
            {
                return BadRequest("StoreId is required for store users");
            }

            id = int.Parse(storeValue); 
        }

        var result = await _storeService.GetByIdAsync(id);
        if (result.Equals(null)) return NotFound();
        return Ok(result);
    }

    [HttpPost("search")]
    public async Task<IActionResult> Search([FromBody] StoreFilterDto filter)
    {
        var result = await _storeService.GetAllAsync(filter);
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateStoreDto createStoreDto)
    {
        var result = await _storeService.AddStoreAsync(createStoreDto);
        return Ok(result);
    }

    [HttpPost("export")]
    public async Task<IActionResult> ExportToExcel([FromBody] StoreFilterDto filter)
    {
        filter.IsAll = true;
        var result = await _storeService.GetAllAsync(filter);
        if (result.Data == null || !result.Data.Any())
        {
            return NotFound("No data found to export.");
        }

        var bytes = _excelExporter.ExportToExcel(result.Data.ToList());
        return File(
            bytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"Orders_{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx"
        );
    }
}
