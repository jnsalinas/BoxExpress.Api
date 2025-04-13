// BoxExpress.Api/Controllers/OrdersController.cs
using BoxExpress.Application.Dtos;
using BoxExpress.Application.Interfaces;
using BoxExpress.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace BoxExpress.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost("search")]
    public async Task<IActionResult> Search([FromBody] OrderFilterDto filter)
    {
        var result = await _orderService.GetAllAsync(filter);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _orderService.GetByIdAsync(id);
        if (result.Equals(null)) return NotFound();
        return Ok(result);
    }

    // [HttpPost]
    // public async Task<IActionResult> Create(Order order)
    // {
    //     var created = await _orderService.AddAsync(order);
    //     return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    // }
}
