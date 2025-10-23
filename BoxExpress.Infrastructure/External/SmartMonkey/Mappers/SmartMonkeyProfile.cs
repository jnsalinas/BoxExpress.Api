using AutoMapper;
using BoxExpress.Application.Dtos.Integrations.Routing;
using BoxExpress.Infrastructure.External.SmartMonkey.Models;

namespace BoxExpress.Infrastructure.External.SmartMonkey.Mappers;
    public class SmartMonkeyProfile : AutoMapper.Profile
{
    public SmartMonkeyProfile()
    {
        CreateMap<RoutingLocationDto, SmartMonkeyLocationPayload>();
        CreateMap<SmartMonkeyCreatePlanResponse, RoutingResponseCreatePlanDto>();
        CreateMap<RoutingCreatePlanDto, SmartMonkeyCreatePlanPayload>();
        CreateMap<RoutingStopDto, SmartMonkeyStopPayload>();
    }
}