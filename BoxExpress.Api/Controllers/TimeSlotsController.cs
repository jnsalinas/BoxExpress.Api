// BoxExpress.Api/Controllers/OrdersController.cs
using BoxExpress.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using BoxExpress.Application.Dtos;
using System.Linq;

namespace BoxExpress.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TimeSlotsController : ControllerBase
{
    private readonly ITimeSlotService _timeSlotService;

    public TimeSlotsController(ITimeSlotService timeSlotService)
    {
        _timeSlotService = timeSlotService;
    }

    [HttpPost("search")]
    public async Task<IActionResult> Search([FromBody] TimeSlotFilterDto filter)
    {
        var result = await _timeSlotService.GetAllAsync(filter);
        return Ok(result);
    }
}
