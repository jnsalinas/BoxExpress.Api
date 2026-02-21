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
public class ProductVariantsController : ControllerBase
{
    private readonly IProductVariantService _productVariantService;

    public ProductVariantsController(IProductVariantService productVariantService)
    {
        _productVariantService = productVariantService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _productVariantService.GetByIdAsync(id);
        if (result.Equals(null)) return NotFound();
        return Ok(result);
    }

    [HttpPost("search")]
    public async Task<IActionResult> Search([FromBody] ProductVariantFilterDto filter)
    {
        var result = await _productVariantService.GetAllAsync(filter);
        return Ok(result);
    }


    [HttpGet("autocomplete")]
    public async Task<IActionResult> Autocomplete([FromQuery] string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return BadRequest("Query is required.");

        AutocompleteVariantFilterDto filter = new AutocompleteVariantFilterDto
        {
            Query = query
        };

        var role = User.FindFirst(ClaimTypes.Role)?.Value;
        if (role?.ToLower() == RolConstants.Store)
        {
            var storeId = User.FindFirst("StoreId")?.Value;
            if (storeId == null)
            {
                return BadRequest("StoreId is required for store role.");
            }

            filter.StoreId = int.Parse(storeId);
        }

        var result = await _productVariantService.GetVariantsAutocompleteAsync(filter);
        return Ok(result);
    }

    [HttpGet("name/{name}")]
    public async Task<IActionResult> GetByName(string name)
    {
        var storeId = User.FindFirst("StoreId")?.Value;
        if (storeId == null)
        {
            return BadRequest("StoreId is required for store role.");
        }

        var result = await _productVariantService.GetByNameAsync(name, int.Parse(storeId));
        return Ok(result);
    }
}
