// BoxExpress.Api/Controllers/OrdersController.cs
using BoxExpress.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using BoxExpress.Application.Dtos;
using System.Linq;

namespace BoxExpress.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DeliveryProvidersController : ControllerBase
{
    private readonly IDeliveryProviderService _DeliveryProviderService;

    public DeliveryProvidersController(IDeliveryProviderService DeliveryProviderService)
    {
        _DeliveryProviderService = DeliveryProviderService;
    }

    [HttpPost("search")]
    public async Task<IActionResult> Search([FromBody] DeliveryProviderFilterDto filter)
    {
        var result = await _DeliveryProviderService.GetAllAsync(filter);
        return Ok(result);
    }
}
