// BoxExpress.Api/Controllers/OrdersController.cs
using BoxExpress.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using BoxExpress.Application.Dtos;
using System.Linq;

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


    // [HttpGet("autocomplete")]
    // public async Task<IActionResult> Search([FromQuery] string query, [FromQuery] int warehouseOriginId)

    // {
    //     if (string.IsNullOrWhiteSpace(query))
    //         return BadRequest("Query is required.");

    //     var result = await _productVariantService.GetVariantsAutocompleteAsync(query, warehouseOriginId);
    //     return Ok(result);
    // }
}
