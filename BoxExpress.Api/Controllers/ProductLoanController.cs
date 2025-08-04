using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BoxExpress.Application.Dtos;
using BoxExpress.Application.Interfaces;
using System.Security.Claims;
using BoxExpress.Domain.Constants;

namespace BoxExpress.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductLoanController : ControllerBase
{
    private readonly IProductLoanService _productLoanService;

    public ProductLoanController(IProductLoanService productLoanService)
    {
        _productLoanService = productLoanService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductLoanDto dto)
    {
        var result = await _productLoanService.CreateAsync(dto);
        if (!result.IsSuccess)
            return BadRequest(result.Message);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _productLoanService.GetByIdAsync(id);
        if (!result.IsSuccess)
            return NotFound(result.Message);
        return Ok(result);
    }

    [HttpPost("search")]
    public async Task<IActionResult> Search([FromBody] ProductLoanFilterDto filter)
    {
        var role = User.FindFirst(ClaimTypes.Role)?.Value;
        if (role?.ToLower() == RolConstants.Warehouse)
        {
            filter.WarehouseId = int.Parse(User.FindFirst("WarehouseId")?.Value ?? "0");
        }

        var result = await _productLoanService.GetFilteredAsync(filter);
        if (!result.IsSuccess)
            return BadRequest(result.Message);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProductLoanDto dto)
    {
        var result = await _productLoanService.UpdateAsync(id, dto);
        if (!result.IsSuccess)
            return BadRequest(result.Message);
        return Ok(result);
    }

    [HttpGet("{productLoanId}/details")]
    public async Task<IActionResult> GetDetails(int productLoanId)
    {
        var result = await _productLoanService.GetDetailsAsync(productLoanId);
        if (!result.IsSuccess)
            return BadRequest(result.Message);
        return Ok(result);
    }
} 