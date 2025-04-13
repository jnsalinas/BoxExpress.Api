// BoxExpress.Api/Controllers/OrdersController.cs
using BoxExpress.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using BoxExpress.Application.Dtos;
using System.Linq;

namespace BoxExpress.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderStatusesController : ControllerBase
{
    private readonly IOrderStatusService _orderStatusService;

    public OrderStatusesController(IOrderStatusService orderStatusService)
    {
        _orderStatusService = orderStatusService;
    }

    [HttpPost("search")]
    public async Task<IActionResult> Search([FromBody] OrderStatusFilterDto filter)
    {
        var result = await _orderStatusService.GetAllAsync(filter);
        return Ok(result);
    }
}
