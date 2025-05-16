// BoxExpress.Api/Controllers/OrdersController.cs
using BoxExpress.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using BoxExpress.Application.Dtos;
using System.Linq;

namespace BoxExpress.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WarehouseInventoriesController : ControllerBase
{
    private readonly IWarehouseInventoryService _service;

    public WarehouseInventoriesController(IWarehouseInventoryService service)
    {
        _service = service;
    }

    [HttpGet("autocomplete")]
    public async Task<IActionResult> Search([FromQuery] string query, [FromQuery] int warehouseOriginId)

    {
        if (string.IsNullOrWhiteSpace(query))
            return BadRequest("Query is required.");

        var result = await _service.GetVariantsAutocompleteAsync(query, warehouseOriginId);
        return Ok(result);
    }
}
