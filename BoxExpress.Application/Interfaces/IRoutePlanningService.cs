using BoxExpress.Application.Dtos.Common;
using BoxExpress.Application.Dtos.Integrations.Routing;

public interface IRoutePlanningService
{
    Task<ApiResponse<RoutingResponseCreatePlanDto>> CreatePlanAsync();
    Task<ApiResponse<bool>> UpdateStatusAsync(RoutingUpdateStatusDto dto);
}