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

        CreateMap<Warehouse, WarehouseDetailDto>()
            .ForMember(dest => dest.CityName, opt => opt.MapFrom(src => src.City.Name))
            .ForMember(dest => dest.CountryName, opt => opt.MapFrom(src => src.Country.Name))
            .ForMember(dest => dest.Products, opt => opt.MapFrom(src =>
                src.Inventories
                .GroupBy(wi => wi.ProductVariant.Product)
                .Select(g => new ProductInventoryDto
                {
                    ShopifyId = g.Key.ShopifyProductId,
                    Id = g.Key.Id,
                    Name = g.Key.Name,
                    Variants = g.Select(vi => new VariantInventoryDto
                    {
                        ShopifyId = vi.ProductVariant.ShopifyVariantId,
                        Id = vi.ProductVariant.Id,
                        Name = !string.IsNullOrEmpty(vi.ProductVariant.Name) ? vi.ProductVariant.Name : string.Empty,
                        Quantity = vi.Quantity
                 }).ToList()
             }).ToList()
     ));


        // Filtros
        CreateMap<WarehouseFilterDto, WarehouseFilter>();

        // DTOs de creación / actualización
        // CreateMap<WarehouseCreateDto, Warehouse>();
        // CreateMap<WarehouseUpdateDto, Warehouse>();
    }

}