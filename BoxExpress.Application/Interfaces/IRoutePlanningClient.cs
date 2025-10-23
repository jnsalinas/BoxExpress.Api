using BoxExpress.Application.Dtos.Integrations.Routing;
using BoxExpress.Application.Dtos.Common;

namespace BoxExpress.Application.Interfaces;
    public interface IRoutePlanningClient
{
    Task<RoutingResponseCreatePlanDto> CreatePlanAsync(RoutingCreatePlanDto request);
}