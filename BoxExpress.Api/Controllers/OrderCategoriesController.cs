// BoxExpress.Api/Controllers/OrdersController.cs
using BoxExpress.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using BoxExpress.Application.Dtos;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace BoxExpress.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
// [Authorize]
public class OrderCategoriesController : ControllerBase
{
    private readonly IOrderCategoryService _orderCategoryService;

    public OrderCategoriesController(IOrderCategoryService orderCategoryService)
    {
        _orderCategoryService = orderCategoryService;
    }

    [HttpPost("search")]
    public async Task<IActionResult> Search([FromBody] OrderCategoryFilterDto filter)
    {
        var result = await _orderCategoryService.GetAllAsync(filter);
        return Ok(result);
    }
}
