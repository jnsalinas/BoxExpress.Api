using AutoMapper;
using BoxExpress.Application.Dtos;
using BoxExpress.Domain.Entities;
using BoxExpress.Domain.Filters;

namespace BoxExpress.Application.Mappings;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        // Entity ➜ DTO para mostrar
        CreateMap<Warehouse, WarehouseDto>()
            .ForMember(dest => dest.CityName, opt => opt.MapFrom(src => src.City.Name))
            .ForMember(dest => dest.CountryName, opt => opt.MapFrom(src => src.Country.Name));

        // Filtros
        CreateMap<WarehouseFilterDto, WarehouseFilter>();

        // DTOs de creación / actualización
        // CreateMap<WarehouseCreateDto, Warehouse>();
        // CreateMap<WarehouseUpdateDto, Warehouse>();
    }

}