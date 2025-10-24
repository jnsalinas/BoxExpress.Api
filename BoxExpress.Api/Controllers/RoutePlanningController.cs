using Microsoft.AspNetCore.Mvc;
using BoxExpress.Application.Interfaces;
using BoxExpress.Application.Dtos.Integrations.Routing;
using BoxExpress.Application.Services;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/[controller]")]
public class RoutePlanningController : ControllerBase
{
    private readonly IRoutePlanningService _routePlanningService;
    public RoutePlanningController(IRoutePlanningService routePlanningService)
    {
        _routePlanningService = routePlanningService;
    }

    [Authorize]
    [HttpPost()]
    public async Task<IActionResult> CreatePlan()
    {
        var result = await _routePlanningService.CreatePlanAsync();
        return Ok(result);
    }

    [HttpPost("update-status")]
    public async Task<IActionResult> UpdateStatus([FromBody] RoutingUpdateStatusDto dto)
    {
        var result = await _routePlanningService.UpdateStatusAsync(dto);
        return Ok(result);
    }
}