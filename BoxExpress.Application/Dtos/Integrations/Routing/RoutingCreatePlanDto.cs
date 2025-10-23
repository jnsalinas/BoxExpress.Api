namespace BoxExpress.Application.Dtos.Integrations.Routing;

public class RoutingCreatePlanDto
{
    public List<RoutingStopDto> Stops { get; set; } = new();
    public string? Label { get; set; }
}